// ContentProjectFactory.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace GreenBox3D.VisualStudio
{
    [Guid(GuidList.ContentProjectFactory)]
    public class ContentProjectFactory : FlavoredProjectFactoryBase
    {
        private readonly GreenBox3DPackage _package;
        private readonly IServiceProvider _provider;

        public ContentProjectFactory(GreenBox3DPackage package)
        {
            _package = package;
            _provider = _package;
        }

        protected override object PreCreateForOuter(IntPtr outerProjectIUnknown)
        {
            return new ContentProject(_package);
        }

        protected override void CreateProject(string fileName, string location, string name, uint flags,
                                              ref Guid projectGuid, out IntPtr project, out int canceled)
        {
            canceled = 0;

            IVsCreateAggregateProject creator =
                (IVsCreateAggregateProject)_provider.GetService(typeof(SVsCreateAggregateProject));
            creator.CreateAggregateProject(ProjectTypeGuids(fileName), fileName, location, name, flags, ref projectGuid,
                                           out project);
        }

        protected override string ProjectTypeGuids(string file)
        {
            return "{" + GuidList.ContentProjectFactory + "};{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}";
        }
    }
}
