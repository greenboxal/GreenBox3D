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

namespace GreenBox3D.VisualStudio
{
    [Guid(GuidList.GreenBox3DPackage)]
    [PackageRegistration(UseManagedResourcesOnly = true, RegisterUsing = RegistrationMethod.CodeBase)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideProjectFactory(typeof(GameProjectFactory), "GreenBox3D", "GreenBox3D Projects (*.csproj);*.csproj", null, null, "\\..\\NullPath", LanguageVsTemplate = "CSharp")]
    public sealed class GreenBox3DPackage : Package
    {
        protected override void Initialize()
        {
            base.Initialize();

            RegisterProjectFactory(new GameProjectFactory(this));
        }
    }
}
