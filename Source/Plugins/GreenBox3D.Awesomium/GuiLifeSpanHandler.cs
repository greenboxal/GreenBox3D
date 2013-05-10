using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    internal class GuiLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly GuiManager _owner;

        public GuiLifeSpanHandler(GuiManager owner)
        {
            _owner = owner;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            base.OnAfterCreated(browser);

            _owner.OnCreated(browser);
        }

        protected override bool DoClose(CefBrowser browser)
        {
            return false;
        }
    }
}
