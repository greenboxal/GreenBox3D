// SettingsPage.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Windows.Forms;
using Microsoft.VisualStudio.Designer.Interfaces;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
    /// <summary>
    ///     The base class for property pages.
    /// </summary>
    [CLSCompliant(false), ComVisible(true)]
    public abstract class SettingsPage :
        LocalizableProperties,
        IPropertyPage,
        IDisposable
    {
        #region fields

        private static volatile object Mutex = new object();
        private bool active;
        private bool dirty;
        private IVSMDPropertyGrid grid;
        private bool isDisposed;
        private string name;
        private Panel panel;
        private ProjectNode project;
        private ProjectConfig[] projectConfigs;
        private IPropertyPageSite site;

        #endregion

        #region properties

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [Browsable(false)]
        [AutomationBrowsable(false)]
        public ProjectNode ProjectMgr
        {
            get { return project; }
        }

        protected IVSMDPropertyGrid Grid
        {
            get { return grid; }
        }

        protected bool IsDirty
        {
            get { return dirty; }
            set
            {
                if (dirty != value)
                {
                    dirty = value;
                    if (site != null)
                        site.OnStatusChange((uint)(dirty ? PropPageStatus.Dirty : PropPageStatus.Clean));
                }
            }
        }

        protected Panel ThePanel
        {
            get { return panel; }
        }

        #endregion

        #region abstract methods

        protected abstract void BindProperties();
        protected abstract int ApplyChanges();

        #endregion

        #region public methods

        public object GetTypedConfigProperty(string name, Type type)
        {
            string value = GetConfigProperty(name);
            if (string.IsNullOrEmpty(value))
                return null;

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            return tc.ConvertFromInvariantString(value);
        }

        public object GetTypedProperty(string name, Type type)
        {
            string value = GetProperty(name);
            if (string.IsNullOrEmpty(value))
                return null;

            TypeConverter tc = TypeDescriptor.GetConverter(type);
            return tc.ConvertFromInvariantString(value);
        }

        public string GetProperty(string propertyName)
        {
            if (ProjectMgr != null)
            {
                string property;
                bool found = ProjectMgr.BuildProject.GlobalProperties.TryGetValue(propertyName, out property);

                if (found)
                {
                    return property;
                }
            }

            return String.Empty;
        }

        // relative to active configuration.
        public string GetConfigProperty(string propertyName)
        {
            if (ProjectMgr != null)
            {
                string unifiedResult = null;
                bool cacheNeedReset = true;

                for (int i = 0; i < projectConfigs.Length; i++)
                {
                    ProjectConfig config = projectConfigs[i];
                    string property = config.GetConfigurationProperty(propertyName, cacheNeedReset);
                    cacheNeedReset = false;

                    if (property != null)
                    {
                        string text = property.Trim();

                        if (i == 0)
                            unifiedResult = text;
                        else if (unifiedResult != text)
                            return ""; // tristate value is blank then
                    }
                }

                return unifiedResult;
            }

            return String.Empty;
        }

        /// <summary>
        ///     Sets the value of a configuration dependent property.
        ///     If the attribute does not exist it is created.
        ///     If value is null it will be set to an empty string.
        /// </summary>
        /// <param name="name">property name.</param>
        /// <param name="value">value of property</param>
        public void SetConfigProperty(string name, string value)
        {
            CCITracing.TraceCall();
            if (value == null)
            {
                value = String.Empty;
            }

            if (ProjectMgr != null)
            {
                for (int i = 0, n = projectConfigs.Length; i < n; i++)
                {
                    ProjectConfig config = projectConfigs[i];

                    config.SetConfigurationProperty(name, value);
                }

                ProjectMgr.SetProjectFileDirty(true);
            }
        }

        #endregion

        #region IPropertyPage methods.

        public virtual void Activate(IntPtr parent, RECT[] pRect, int bModal)
        {
            if (panel == null)
            {
                if (pRect == null)
                {
                    throw new ArgumentNullException("pRect");
                }

                panel = new Panel();
                panel.Size = new Size(pRect[0].right - pRect[0].left, pRect[0].bottom - pRect[0].top);
                panel.Text = SR.GetString(SR.Settings, CultureInfo.CurrentUICulture);
                panel.Visible = false;
                panel.Size = new Size(550, 300);
                panel.CreateControl();
                NativeMethods.SetParent(panel.Handle, parent);
            }

            if (grid == null && project != null && project.Site != null)
            {
                IVSMDPropertyBrowser pb = project.Site.GetService(typeof(IVSMDPropertyBrowser)) as IVSMDPropertyBrowser;
                grid = pb.CreatePropertyGrid();
            }

            if (grid != null)
            {
                active = true;

                Control cGrid = Control.FromHandle(new IntPtr(grid.Handle));

                cGrid.Parent = Control.FromHandle(parent); //this.panel;
                cGrid.Size = new Size(544, 294);
                cGrid.Location = new Point(3, 3);
                cGrid.Visible = true;
                grid.SetOption(_PROPERTYGRIDOPTION.PGOPT_TOOLBAR, false);
                grid.GridSort = _PROPERTYGRIDSORT.PGSORT_CATEGORIZED | _PROPERTYGRIDSORT.PGSORT_ALPHABETICAL;
                NativeMethods.SetParent(new IntPtr(grid.Handle), panel.Handle);
                UpdateObjects();
            }
        }

        public virtual int Apply()
        {
            if (IsDirty)
            {
                return ApplyChanges();
            }
            return VSConstants.S_OK;
        }

        public virtual void Deactivate()
        {
            if (null != panel)
            {
                panel.Dispose();
                panel = null;
            }
            active = false;
        }

        public virtual void GetPageInfo(PROPPAGEINFO[] arrInfo)
        {
            if (arrInfo == null)
            {
                throw new ArgumentNullException("arrInfo");
            }

            PROPPAGEINFO info = new PROPPAGEINFO();

            info.cb = (uint)Marshal.SizeOf(typeof(PROPPAGEINFO));
            info.dwHelpContext = 0;
            info.pszDocString = null;
            info.pszHelpFile = null;
            info.pszTitle = name;
            info.SIZE.cx = 550;
            info.SIZE.cy = 300;
            arrInfo[0] = info;
        }

        public virtual void Help(string helpDir)
        {
        }

        public virtual int IsPageDirty()
        {
            // Note this returns an HRESULT not a Bool.
            return (IsDirty ? VSConstants.S_OK : VSConstants.S_FALSE);
        }

        public virtual void Move(RECT[] arrRect)
        {
            if (arrRect == null)
            {
                throw new ArgumentNullException("arrRect");
            }

            RECT r = arrRect[0];

            panel.Location = new Point(r.left, r.top);
            panel.Size = new Size(r.right - r.left, r.bottom - r.top);
        }

        public virtual void SetObjects(uint count, object[] punk)
        {
            if (punk == null)
            {
                return;
            }

            if (count > 0)
            {
                if (punk[0] is ProjectConfig)
                {
                    ArrayList configs = new ArrayList();

                    for (int i = 0; i < count; i++)
                    {
                        ProjectConfig config = (ProjectConfig)punk[i];

                        if (project == null || (project != (punk[0] as ProjectConfig).ProjectMgr))
                        {
                            project = config.ProjectMgr;
                        }

                        configs.Add(config);
                    }

                    projectConfigs = (ProjectConfig[])configs.ToArray(typeof(ProjectConfig));
                }
                else if (punk[0] is NodeProperties)
                {
                    if (project == null || (project != (punk[0] as NodeProperties).Node.ProjectMgr))
                    {
                        project = (punk[0] as NodeProperties).Node.ProjectMgr;
                    }

                    Dictionary<string, ProjectConfig> configsMap = new Dictionary<string, ProjectConfig>();

                    for (int i = 0; i < count; i++)
                    {
                        NodeProperties property = (NodeProperties)punk[i];
                        IVsCfgProvider provider;
                        ErrorHandler.ThrowOnFailure(property.Node.ProjectMgr.GetCfgProvider(out provider));
                        uint[] expected = new uint[1];
                        ErrorHandler.ThrowOnFailure(provider.GetCfgs(0, null, expected, null));
                        if (expected[0] > 0)
                        {
                            ProjectConfig[] configs = new ProjectConfig[expected[0]];
                            uint[] actual = new uint[1];
                            ErrorHandler.ThrowOnFailure(provider.GetCfgs(expected[0], configs, actual, null));

                            foreach (ProjectConfig config in configs)
                            {
                                if (!configsMap.ContainsKey(config.ConfigName))
                                {
                                    configsMap.Add(config.ConfigName, config);
                                }
                            }
                        }
                    }

                    if (configsMap.Count > 0)
                    {
                        if (projectConfigs == null)
                        {
                            projectConfigs = new ProjectConfig[configsMap.Keys.Count];
                        }
                        configsMap.Values.CopyTo(projectConfigs, 0);
                    }
                }
            }
            else
            {
                project = null;
            }

            if (active && project != null)
            {
                UpdateObjects();
            }
        }

        public virtual void SetPageSite(IPropertyPageSite theSite)
        {
            site = theSite;
        }

        public virtual void Show(uint cmd)
        {
            panel.Visible = true; // TODO: pass SW_SHOW* flags through      
            panel.Show();
        }

        public virtual int TranslateAccelerator(MSG[] arrMsg)
        {
            if (arrMsg == null)
            {
                throw new ArgumentNullException("arrMsg");
            }

            MSG msg = arrMsg[0];

            if ((msg.message < NativeMethods.WM_KEYFIRST || msg.message > NativeMethods.WM_KEYLAST) &&
                (msg.message < NativeMethods.WM_MOUSEFIRST || msg.message > NativeMethods.WM_MOUSELAST))
                return 1;

            return (NativeMethods.IsDialogMessageA(panel.Handle, ref msg)) ? 0 : 1;
        }

        #endregion

        #region helper methods

        protected ProjectConfig[] GetProjectConfigurations()
        {
            return projectConfigs;
        }

        protected void UpdateObjects()
        {
            if (projectConfigs != null && project != null)
            {
                // Demand unmanaged permissions in order to access unmanaged memory.
                new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

                IntPtr p = Marshal.GetIUnknownForObject(this);
                IntPtr ppUnk = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(IntPtr)));
                try
                {
                    Marshal.WriteIntPtr(ppUnk, p);
                    BindProperties();
                    // BUGBUG -- this is really bad casting a pointer to "int"...
                    grid.SetSelectedObjects(1, ppUnk.ToInt32());
                    grid.Refresh();
                }
                finally
                {
                    if (ppUnk != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem(ppUnk);
                    }
                    if (p != IntPtr.Zero)
                    {
                        Marshal.Release(p);
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                lock (Mutex)
                {
                    if (disposing)
                    {
                        panel.Dispose();
                    }

                    isDisposed = true;
                }
            }
        }
    }
}
