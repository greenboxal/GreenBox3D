// Guids.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;

namespace GreenBox3D.VisualStudio
{
    internal static class GuidList
    {
        public const string GreenBox3DPackage = "116E413F-0A1F-4ECB-AA46-3D421624B05D";
        
        // Project
        public const string GameProjectFactory = "3D751863-3F8C-4CC9-A251-17BF9CE46515";
        public const string GameFlavoredProject = "08742854-D5AD-48FC-AE87-70D54138640C";

        public static readonly Guid GreenBox3DPackageGuid = new Guid(GreenBox3DPackage);
        public static readonly Guid GameProjectFactoryGuid = new Guid(GameProjectFactory);
        public static readonly Guid GameFlavoredProjectGuid = new Guid(GameFlavoredProject);
    };
}
