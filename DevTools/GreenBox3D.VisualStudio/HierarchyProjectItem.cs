// HierarchyProjectItem.cs
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
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace GreenBox3D.VisualStudio
{
    public class HierarchyProjectItem : HierarchyItem, IDisposable
    {
        /*private ImageList _projectImageList;

        protected ImageList IconImageList
        {
            get
            {
                if (_projectImageList == null)
                {
                    object pvar = (object)null;
                    ErrorHandler.ThrowOnFailure(InnerHierarchy.GetProperty((uint)VSConstants.VSITEMID.Nil, (int)__VSHPROPID.VSHPROPID_IconImgList, out pvar));
                    _projectImageList = ProjectUtilities.GetImageList(pvar);
                }

                return _projectImageList;
            }
        }

        public HierarchyProjectItem(IVsUIHierarchy innerIVsUIHierarchy)
            : base(innerIVsUIHierarchy)
        {
            OverrideProperty(-2095, (object)false);
            OverrideProperty(-2094, (object)false);
        }

        ~HierarchyProjectItem()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _projectImageList == null)
                return;

            _projectImageList.Dispose();
            _projectImageList = null;
        }

        protected int AddImageListIcon(Image image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            ImageList iconImageList = IconImageList;
            int count = iconImageList.Images.Count;
            iconImageList.Images.Add(image, Color.Magenta);

            return count;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize((object)this);
        }*/
    }
}
