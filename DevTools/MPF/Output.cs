// Output.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Build.Execution;

namespace Microsoft.VisualStudio.Project
{
    internal class Output : IVsOutput2
    {
        private readonly ProjectItemInstance output;
        private readonly ProjectNode project;

        /// <summary>
        ///     Constructor for IVSOutput2 implementation
        /// </summary>
        /// <param name="projectManager">Project that produce this output</param>
        /// <param name="outputAssembly">MSBuild generated item corresponding to the output assembly (by default, these would be of type MainAssembly</param>
        public Output(ProjectNode projectManager, ProjectItemInstance outputAssembly)
        {
            if (projectManager == null)
                throw new ArgumentNullException("projectManager");
            if (outputAssembly == null)
                throw new ArgumentNullException("outputAssembly");

            project = projectManager;
            output = outputAssembly;
        }

        #region IVsOutput2 Members

        public int get_CanonicalName(out string pbstrCanonicalName)
        {
            // Get the output assembly path (including the name)
            pbstrCanonicalName = output.GetMetadataValue(ProjectFileConstants.FinalOutputPath);
            Debug.Assert(!String.IsNullOrEmpty(pbstrCanonicalName), "Output Assembly not defined");

            // Make sure we have a full path
            if (!Path.IsPathRooted(pbstrCanonicalName))
            {
                pbstrCanonicalName = new Url(project.BaseURI, pbstrCanonicalName).AbsoluteUrl;
            }
            return VSConstants.S_OK;
        }

        /// <summary>
        ///     This path must start with file:/// if it wants other project
        ///     to be able to reference the output on disk.
        ///     If the output is not on disk, then this requirement does not
        ///     apply as other projects probably don't know how to access it.
        /// </summary>
        public virtual int get_DeploySourceURL(out string pbstrDeploySourceURL)
        {
            string path = output.GetMetadataValue(ProjectFileConstants.FinalOutputPath);
            if (string.IsNullOrEmpty(path))
            {
                throw new InvalidOperationException();
            }
            if (path.Length < 9 ||
                String.Compare(path.Substring(0, 8), "file:///", StringComparison.OrdinalIgnoreCase) != 0)
                path = "file:///" + path; // TODO: does not work with '#' char, see e.g. bug 641942
            pbstrDeploySourceURL = path;
            return VSConstants.S_OK;
        }

        public int get_DisplayName(out string pbstrDisplayName)
        {
            return get_CanonicalName(out pbstrDisplayName);
        }

        public virtual int get_Property(string szProperty, out object pvar)
        {
            if (string.IsNullOrEmpty(szProperty))
            {
                pvar = null;
                return VSConstants.E_INVALIDARG;
            }

            if (string.Equals(szProperty, "OUTPUTLOC", StringComparison.OrdinalIgnoreCase))
            {
                szProperty = ProjectFileConstants.FinalOutputPath;
            }

            string value = output.GetMetadataValue(szProperty);
            pvar = value;

            // If we don't have a value, we are expected to return unimplemented
            if (string.IsNullOrEmpty(value))
            {
                return VSConstants.E_NOTIMPL;
            }

            // Special hack for COM2REG property: it's a bool rather than a string, and always true, for some reason.
            if (string.Equals(szProperty, "COM2REG", StringComparison.OrdinalIgnoreCase))
            {
                pvar = true;
            }

            return VSConstants.S_OK;
        }

        public int get_RootRelativeURL(out string pbstrRelativePath)
        {
            pbstrRelativePath = String.Empty;
            object variant;
            // get the corresponding property

            if (ErrorHandler.Succeeded(get_Property("TargetPath", out variant)))
            {
                string var = variant as String;

                if (var != null)
                {
                    pbstrRelativePath = var;
                }
            }

            return VSConstants.S_OK;
        }

        public virtual int get_Type(out Guid pguidType)
        {
            pguidType = Guid.Empty;
            throw new NotImplementedException();
        }

        #endregion
    }
}
