using System;
using System.Xml;

using MonoDevelop.Projects;

namespace GreenBox3D.MDAddin
{
	public class ContentProjectBinding : IProjectBinding
	{
		public Project CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			return new ContentProject(info, projectOptions);
		}
		
		public Project CreateSingleFileProject(string sourceFile)
		{
			return null;
		}
		
		public bool CanCreateSingleFileProject(string sourceFile)
		{
			return false;
		}
		
		public string Name 
		{
			get { return "GBContentProject"; }
		}
	}
}
