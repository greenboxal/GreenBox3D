// GreenBox3DPackage.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Project;

namespace GreenBox3D.VisualStudio
{
    [Guid(GuidList.GreenBox3DPackage)]
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideProjectFactory(typeof(ContentProjectFactory), "Content Project",
        "GreenBox3D Content Project (*.gbcontentproj);*.gbcontentproj", "gbcontentproj", "gbcontentproj",
        @"Templates\Projects\ContentProject")]
    public sealed class GreenBox3DPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();
            RegisterProjectFactory(new ContentProjectFactory(this));
        }
    }
}
