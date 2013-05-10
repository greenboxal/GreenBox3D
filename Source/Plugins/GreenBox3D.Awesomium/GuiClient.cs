using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace GreenBox3D.Awesomium
{
    internal class GuiClient : CefClient
    {
        private readonly GuiLifeSpanHandler _lifeSpanHandler;
        private readonly GuiRenderHandler _renderHandler;

        public GuiClient(GuiManager owner)
        {
            _lifeSpanHandler = new GuiLifeSpanHandler(owner);
            _renderHandler = new GuiRenderHandler(owner);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        public void Resize(int width, int height)
        {
            _renderHandler.Resize(width, height);
        }

        public void Update()
        {
            _renderHandler.Update();
        }

        public void Render()
        {
            _renderHandler.Render();
        }

        public bool CheckMousePosition(int x, int y)
        {
            return _renderHandler.GetAlphaAt(x, y) >= 30;
        }
    }
}
