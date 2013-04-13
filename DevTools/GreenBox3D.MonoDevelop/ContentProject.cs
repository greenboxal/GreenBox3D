// GreenBox3D
//
// Copyright (c) 2013 The GreenBox Development Inc.
// Copyright (c) 2013 Mono.Xna Team and Contributors
//
// Licensed under MIT license terms.

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
