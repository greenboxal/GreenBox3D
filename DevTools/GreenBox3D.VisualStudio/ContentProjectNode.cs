// ContentProjectNode.cs
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
using Microsoft.VisualStudio.Shell.Interop;

namespace GreenBox3D.VisualStudio
{
    public class ContentProjectNode : HierarchyProjectItem
    {
        private IServiceProvider serviceProvider;

        public ContentProjectNode(IVsUIHierarchy innerHierarchy, IServiceProvider serviceProvider)
            : base(innerHierarchy)
        {
            this._innerVsUIHierarchy = _innerVsUIHierarchy;
            this.serviceProvider = serviceProvider;
        }
    }
}
