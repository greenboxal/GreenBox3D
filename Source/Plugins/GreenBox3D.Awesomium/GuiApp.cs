using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    public class GuiApp : CefApp
    {
        private readonly GuiRenderProcessHandler _renderProcessHandler;

        public GuiApp()
        {
            _renderProcessHandler = new GuiRenderProcessHandler();
        }

        protected override CefRenderProcessHandler GetRenderProcessHandler()
        {
            return _renderProcessHandler;
        }
    }
}
