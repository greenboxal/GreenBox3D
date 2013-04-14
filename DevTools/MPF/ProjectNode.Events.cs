// ProjectNode.Events.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Project
{
    public partial class ProjectNode
    {
        #region fields

        private EventHandler<ProjectPropertyChangedArgs> projectPropertiesListeners;

        #endregion

        #region events

        public event EventHandler<ProjectPropertyChangedArgs> OnProjectPropertyChanged
        {
            add { projectPropertiesListeners += value; }
            remove { projectPropertiesListeners -= value; }
        }

        #endregion

        #region methods

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
        protected void RaiseProjectPropertyChanged(string propertyName, string oldValue, string newValue)
        {
            if (null != projectPropertiesListeners)
            {
                projectPropertiesListeners(this, new ProjectPropertyChangedArgs(propertyName, oldValue, newValue));
            }
        }

        #endregion
    }
}
