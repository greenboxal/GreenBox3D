// HierarchyItem.cs
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
using Microsoft.VisualStudio.OLE.Interop;

namespace GreenBox3D.VisualStudio
{
    public class HierarchyItem
    {
        private readonly List<int> _disabledGuidProperties;
        private readonly List<int> _disabledProperties;
        private readonly IVsUIHierarchy _innerIVsUIHierarchy;
        private readonly Dictionary<int, GetGuidPropertyDelegate> _overrideGetGuidProperties;
        private readonly Dictionary<int, GetPropertyDelegate> _overrideGetProperties;
        private readonly Dictionary<int, Guid> _overrideGuidProperties;
        private readonly Dictionary<int, object> _overrideProperties;
        private readonly Dictionary<int, SetPropertyDelegate> _overrideSetProperties;

        internal HierarchyItem(IVsUIHierarchy innerIVsUIHierarchy)
        {
            _innerIVsUIHierarchy = innerIVsUIHierarchy;
            _disabledProperties = new List<int>();
            _overrideProperties = new Dictionary<int, object>();
            _overrideGetProperties = new Dictionary<int, GetPropertyDelegate>();
            _overrideSetProperties = new Dictionary<int, SetPropertyDelegate>();
            _disabledGuidProperties = new List<int>();
            _overrideGuidProperties = new Dictionary<int, Guid>();
            _overrideGetGuidProperties = new Dictionary<int, GetGuidPropertyDelegate>();
        }

        protected IVsHierarchy InnerHierarchy
        {
            get { return _innerIVsUIHierarchy; }
        }

        protected void DisableProperty(int propid)
        {
            DisableProperty(propid, _disabledProperties);
        }

        protected void ResetProperty(int propid)
        {
            ResetProperty(propid, _disabledProperties);

            if (_overrideProperties.ContainsKey(propid))
                _overrideProperties.Remove(propid);

            if (_overrideGetProperties.ContainsKey(propid))
                _overrideGetProperties.Remove(propid);

            if (!_overrideSetProperties.ContainsKey(propid))
                return;

            _overrideSetProperties.Remove(propid);
        }

        protected void OverrideProperty(int propid, object value)
        {
            ResetProperty(propid);
            _overrideProperties[propid] = value;
        }

        protected void OverrideProperty(int propid, GetPropertyDelegate getHandler)
        {
            OverrideProperty(propid, getHandler, null);
        }

        protected void OverrideProperty(int propid, GetPropertyDelegate getHandler, SetPropertyDelegate setHandler)
        {
            ResetProperty(propid);
            _overrideGetProperties[propid] = getHandler;
            _overrideSetProperties[propid] = setHandler;
        }

        protected void DisableGuidProperty(int propid)
        {
            DisableProperty(propid, _disabledGuidProperties);
        }

        protected void ResetGuidProperty(int propid)
        {
            ResetProperty(propid, _disabledGuidProperties);

            if (_overrideGuidProperties.ContainsKey(propid))
                _overrideGuidProperties.Remove(propid);

            if (!_overrideGetGuidProperties.ContainsKey(propid))
                return;

            _overrideGetGuidProperties.Remove(propid);
        }

        protected void OverrideGuidProperty(int propid, ref Guid guidValue)
        {
            ResetGuidProperty(propid);
            _overrideGuidProperties[propid] = guidValue;
        }

        protected void OverrideGuidProperty(int propid, GetGuidPropertyDelegate getHandler)
        {
            ResetGuidProperty(propid);
            _overrideGetGuidProperties[propid] = getHandler;
        }

        private static void DisableProperty(int propid, List<int> sortedList)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList");
            int num = sortedList.BinarySearch(propid);
            if (num >= 0)
                return;
            int index = ~num;
            sortedList.Insert(index, propid);
        }

        private static void ResetProperty(int propid, List<int> sortedList)
        {
            if (sortedList == null)
                throw new ArgumentNullException("sortedList");
            int index = sortedList.BinarySearch(propid);
            if (index < 0)
                return;
            sortedList.RemoveAt(index);
        }

        internal virtual int GetGuidProperty(uint itemid, int propid, out Guid pguid)
        {
            pguid = Guid.Empty;

            if (_disabledGuidProperties.BinarySearch(propid) >= 0)
                return -2147467263;

            if (_overrideGuidProperties.TryGetValue(propid, out pguid))
                return 0;

            GetGuidPropertyDelegate propertyDelegate = null;

            if (_overrideGetGuidProperties.TryGetValue(propid, out propertyDelegate))
                return propertyDelegate(itemid, propid, out pguid);

            if (_innerIVsUIHierarchy != null)
                return _innerIVsUIHierarchy.GetGuidProperty(itemid, propid, out pguid);
            else
                return -2147467263;
        }

        internal virtual int SetGuidProperty(uint itemid, int propid, ref Guid rguid)
        {
            if (_disabledGuidProperties.BinarySearch(propid) >= 0)
                return -2147467263;

            if (_overrideGuidProperties.ContainsKey(propid))
            {
                _overrideGuidProperties[propid] = rguid;
                return 0;
            }
            else if (_innerIVsUIHierarchy != null)
            {
                return _innerIVsUIHierarchy.SetGuidProperty(itemid, propid, ref rguid);
            }
            else
            {
                return -2147467263;
            }
        }

        internal virtual int GetProperty(uint itemid, int propid, out object pvar)
        {
            pvar = null;

            if (_disabledProperties.BinarySearch(propid) >= 0)
                return -2147467263;

            if (_overrideProperties.TryGetValue(propid, out pvar))
                return 0;

            GetPropertyDelegate propertyDelegate = null;

            if (_overrideGetProperties.TryGetValue(propid, out propertyDelegate))
                return propertyDelegate(itemid, propid, out pvar);

            if (_innerIVsUIHierarchy != null)
                return _innerIVsUIHierarchy.GetProperty(itemid, propid, out pvar);
            else
                return -2147467263;
        }

        internal virtual int SetProperty(uint itemid, int propid, object var)
        {
            if (_disabledProperties.BinarySearch(propid) >= 0)
                return -2147467263;

            if (_overrideProperties.ContainsKey(propid))
            {
                _overrideProperties[propid] = var;
                return 0;
            }
            else
            {
                SetPropertyDelegate propertyDelegate = null;

                if (_overrideSetProperties.TryGetValue(propid, out propertyDelegate))
                {
                    if (propertyDelegate == null)
                        return -2147467263;
                    else
                        return propertyDelegate(itemid, propid, var);
                }
                else if (_innerIVsUIHierarchy != null)
                {
                    return _innerIVsUIHierarchy.SetProperty(itemid, propid, var);
                }
                else
                {
                    return -2147467263;
                }
            }
        }

        internal virtual int QueryStatusCommand(uint itemid, ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds,
                                                IntPtr pCmdText)
        {
            if (_innerIVsUIHierarchy != null)
                return _innerIVsUIHierarchy.QueryStatusCommand(itemid, ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
            else
                return -2147467263;
        }

        internal virtual int ExecCommand(uint itemid, ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt,
                                         IntPtr pvaIn, IntPtr pvaOut)
        {
            if (_innerIVsUIHierarchy != null)
                return _innerIVsUIHierarchy.ExecCommand(itemid, ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
            else
                return -2147467263;
        }

        protected delegate int GetGuidPropertyDelegate(uint itemid, int propid, out Guid value);

        protected delegate int GetPropertyDelegate(uint itemid, int propid, out object value);

        protected delegate int SetPropertyDelegate(uint itemid, int propid, object value);
    }
}
