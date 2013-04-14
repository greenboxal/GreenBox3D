// ContentProject.cs
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
    public class ContentProject : FlavoredProjectBase
    {
        private readonly GreenBox3DPackage _package;
        private IVsProjectBuildSystem _buildSystem;

        // Interfaces
        private IVsProjectFlavorCfgProvider _cfgProvider;
        private IVsBuildPropertyStorage _propertyStorage;
        private ContentProjectNode _root;

        public ContentProject(GreenBox3DPackage package)
        {
            _package = package;
        }

        protected override void InitializeForOuter(string fileName, string location, string name, uint flags,
                                                   ref Guid guidProject, out bool cancel)
        {
            base.InitializeForOuter(fileName, location, name, flags, ref guidProject, out cancel);
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            object unknownObject = Marshal.GetObjectForIUnknown(innerIUnknown);

            if (serviceProvider == null)
                serviceProvider = _package;

            base.SetInnerProject(innerIUnknown);

            _cfgProvider = unknownObject as IVsProjectFlavorCfgProvider;
            _buildSystem = unknownObject as IVsProjectBuildSystem;
            _root = new ContentProjectNode(_innerVsUIHierarchy, serviceProvider);
        }
    }
}
