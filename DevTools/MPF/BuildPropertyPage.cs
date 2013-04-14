// BuildPropertyPage.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     Enumerated list of the properties shown on the build property page
    /// </summary>
    internal enum BuildPropertyPageTag
    {
        OutputPath
    }

    /// <summary>
    ///     Defines the properties on the build property page and the logic the binds the properties to project data (load and save)
    /// </summary>
    [CLSCompliant(false), ComVisible(true), Guid("9B3DEA40-7F29-4a17-87A4-00EE08E8241E")]
    public class BuildPropertyPage : SettingsPage
    {
        #region fields

        private string outputPath;

        public BuildPropertyPage()
        {
            Name = SR.GetString(SR.BuildCaption, CultureInfo.CurrentUICulture);
        }

        #endregion

        #region properties

        [SRCategory(SR.BuildCaption)]
        [LocDisplayName(SR.OutputPath)]
        [SRDescription(SR.OutputPathDescription)]
        public string OutputPath
        {
            get { return outputPath; }
            set
            {
                outputPath = value;
                IsDirty = true;
            }
        }

        #endregion

        #region overridden methods

        public override string GetClassName()
        {
            return GetType().FullName;
        }

        protected override void BindProperties()
        {
            if (ProjectMgr == null)
            {
                Debug.Assert(false);
                return;
            }

            outputPath = GetConfigProperty(BuildPropertyPageTag.OutputPath.ToString());
        }

        protected override int ApplyChanges()
        {
            if (ProjectMgr == null)
            {
                Debug.Assert(false);
                return VSConstants.E_INVALIDARG;
            }

            SetConfigProperty(BuildPropertyPageTag.OutputPath.ToString(), outputPath);
            IsDirty = false;
            return VSConstants.S_OK;
        }

        #endregion
    }
}
