// SuspendFileChanges.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using IServiceProvider = System.IServiceProvider;
using ShellConstants = Microsoft.VisualStudio.Shell.Interop.Constants;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     helper to make the editor ignore external changes
    /// </summary>
    internal class SuspendFileChanges
    {
        private readonly string documentFileName;

        private readonly IServiceProvider site;

        private IVsDocDataFileChangeControl fileChangeControl;
        private bool isSuspending;

        public SuspendFileChanges(IServiceProvider site, string document)
        {
            this.site = site;
            documentFileName = document;
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        public void Suspend()
        {
            if (isSuspending)
                return;

            IntPtr docData = IntPtr.Zero;
            try
            {
                IVsRunningDocumentTable rdt =
                    site.GetService(typeof(SVsRunningDocumentTable)) as IVsRunningDocumentTable;

                IVsHierarchy hierarchy;
                uint itemId;
                uint docCookie;
                IVsFileChangeEx fileChange;

                if (rdt == null)
                    return;

                ErrorHandler.ThrowOnFailure(rdt.FindAndLockDocument((uint)_VSRDTFLAGS.RDT_NoLock, documentFileName,
                                                                    out hierarchy, out itemId, out docData,
                                                                    out docCookie));

                if ((docCookie == (uint)ShellConstants.VSDOCCOOKIE_NIL) || docData == IntPtr.Zero)
                    return;

                fileChange = site.GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;

                if (fileChange != null)
                {
                    isSuspending = true;
                    ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(0, documentFileName, 1));
                    if (docData != IntPtr.Zero)
                    {
                        IVsPersistDocData persistDocData = null;

                        // if interface is not supported, return null
                        object unknown = Marshal.GetObjectForIUnknown(docData);
                        if (unknown is IVsPersistDocData)
                        {
                            persistDocData = (IVsPersistDocData)unknown;
                            if (persistDocData is IVsDocDataFileChangeControl)
                            {
                                fileChangeControl = (IVsDocDataFileChangeControl)persistDocData;
                                if (fileChangeControl != null)
                                {
                                    ErrorHandler.ThrowOnFailure(fileChangeControl.IgnoreFileChanges(1));
                                }
                            }
                        }
                    }
                }
            }
            catch (InvalidCastException e)
            {
                Trace.WriteLine("Exception" + e.Message);
            }
            finally
            {
                if (docData != IntPtr.Zero)
                {
                    Marshal.Release(docData);
                }
            }
            return;
        }

        public void Resume()
        {
            if (!isSuspending)
                return;
            IVsFileChangeEx fileChange;
            fileChange = site.GetService(typeof(SVsFileChangeEx)) as IVsFileChangeEx;
            if (fileChange != null)
            {
                isSuspending = false;
                ErrorHandler.ThrowOnFailure(fileChange.IgnoreFile(0, documentFileName, 0));
                if (fileChangeControl != null)
                {
                    ErrorHandler.ThrowOnFailure(fileChangeControl.IgnoreFileChanges(0));
                }
            }
        }
    }
}
