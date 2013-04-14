// ContentProject.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Xml;
using MonoDevelop.Projects;

namespace GreenBox3D.MDAddin
{
    public class ContentProject : Project
    {
        public ContentProject(ProjectCreateInformation info, XmlElement projectOptions)
        {
        }

        public override string ProjectType
        {
            get { return "MSBuild"; }
        }
    }
}
