// ConfigurationProperties.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     Defines the config dependent properties exposed through automation
    /// </summary>
    [ComVisible(true)]
    [Guid("21f73a8f-91d7-4085-9d4f-c48ee235ee5b")]
    public interface IProjectConfigProperties
    {
        string OutputPath { get; set; }
    }

    /// <summary>
    ///     Implements the configuration dependent properties interface
    /// </summary>
    [CLSCompliant(false), ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    public class ProjectConfigProperties : IProjectConfigProperties
    {
        #region fields

        private readonly ProjectConfig projectConfig;

        #endregion

        #region ctors

        public ProjectConfigProperties(ProjectConfig projectConfig)
        {
            this.projectConfig = projectConfig;
        }

        #endregion

        #region IProjectConfigProperties Members

        public virtual string OutputPath
        {
            get { return projectConfig.GetConfigurationProperty(BuildPropertyPageTag.OutputPath.ToString(), true); }
            set { projectConfig.SetConfigurationProperty(BuildPropertyPageTag.OutputPath.ToString(), value); }
        }

        #endregion
    }
}
