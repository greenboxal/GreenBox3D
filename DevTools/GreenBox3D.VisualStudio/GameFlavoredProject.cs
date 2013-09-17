using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Flavor;
using Microsoft.VisualStudio.Shell.Interop;

namespace GreenBox3D.VisualStudio
{
    [ComVisible(true)]
    [Guid(GuidList.GameFlavoredProject)]
    [ClassInterface(ClassInterfaceType.None)]
    public class GameFlavoredProject : FlavoredProjectBase
    {
        private readonly GreenBox3DPackage _package;

        public GameFlavoredProject(GreenBox3DPackage package)
        {
            _package = package;
        }

        protected override void SetInnerProject(IntPtr innerIUnknown)
        {
            if (serviceProvider == null)
                serviceProvider = _package;

            base.SetInnerProject(innerIUnknown);
        }

        protected override int GetProperty(uint itemId, int propId, out object property)
        {
            
            /*switch (propId)
            {
                case (int)__VSHPROPID.VSHPROPID_IconIndex:
                case (int)__VSHPROPID.VSHPROPID_OpenFolderIconIndex:
                    {
                        if (itemId == VSConstants.VSITEMID_ROOT)
                        {
                            property = null;
                            //Forward to __VSHPROPID.VSHPROPID_IconHandle and __VSHPROPID.VSHPROPID_OpenFolderIconHandle propIds
                            return VSConstants.E_NOTIMPL;
                        }

                        break;
                    }
                case (int)__VSHPROPID.VSHPROPID_IconHandle:
                case (int)__VSHPROPID.VSHPROPID_OpenFolderIconHandle:
                    {
                        if (itemId == VSConstants.VSITEMID_ROOT && projectIcon != null)
                        {
                            property = projectIcon.Handle;
                            return VSConstants.S_OK;
                        }

                        break;
                    }
            }*/

            return base.GetProperty(itemId, propId, out property);
        }
    }
}
