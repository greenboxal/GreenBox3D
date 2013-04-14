// ProjectPackage.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     Defines abstract package.
    /// </summary>
    [ComVisible(true)]
    [CLSCompliant(false)]
    public abstract class ProjectPackage : Package
    {
        #region fields

        /// <summary>
        ///     This is the place to register all the solution listeners.
        /// </summary>
        private readonly List<SolutionListener> solutionListeners = new List<SolutionListener>();

        #endregion

        #region properties

        /// <summary>
        ///     Add your listener to this list. They should be added in the overridden Initialize befaore calling the base.
        /// </summary>
        protected internal IList<SolutionListener> SolutionListeners
        {
            get { return solutionListeners; }
        }

        public abstract string ProductUserContext { get; }

        #endregion

        #region methods

        protected override void Initialize()
        {
            base.Initialize();

            // Subscribe to the solution events
            solutionListeners.Add(new SolutionListenerForProjectReferenceUpdate(this));
            solutionListeners.Add(new SolutionListenerForProjectOpen(this));
            solutionListeners.Add(new SolutionListenerForBuildDependencyUpdate(this));
            solutionListeners.Add(new SolutionListenerForProjectEvents(this));

            foreach (SolutionListener solutionListener in solutionListeners)
            {
                solutionListener.Init();
            }
        }

        protected override void Dispose(bool disposing)
        {
            // Unadvise solution listeners.
            try
            {
                if (disposing)
                {
                    foreach (SolutionListener solutionListener in solutionListeners)
                    {
                        solutionListener.Dispose();
                    }

                    // Dispose the UIThread singleton.
                    UIThread.Instance.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion
    }
}
