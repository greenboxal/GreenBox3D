using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    public class GuiRenderProcessHandler : CefRenderProcessHandler
    {
        protected override void OnFocusedNodeChanged(CefBrowser browser, CefFrame frame, CefDomNode node)
        {
            if (node != null)
                ChromiumRuntime.SelectedNodeEditable = node.IsEditable;
            else
                ChromiumRuntime.SelectedNodeEditable = false;

            base.OnFocusedNodeChanged(browser, frame, node);
        }
    }
}
