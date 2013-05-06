using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenBox3D.Graphics
{
    public abstract class RasterizerState : GraphicsResource
    {
        public static RasterizerState CullClockwise { get { return GraphicsDevice.ActiveDevice.StateManager.RasterizerCullClockwise; } }
        public static RasterizerState CullCounterClockwise { get { return GraphicsDevice.ActiveDevice.StateManager.RasterizerCullCounterClockwise; } }
        public static RasterizerState CullNone { get { return GraphicsDevice.ActiveDevice.StateManager.RasterizerCullNone; } }

        public abstract CullMode CullMode { get; set; }
        public abstract FillMode FillMode { get; set; }
        public abstract bool MultiSampleAntiAlias { get; set; }
        public abstract bool ScissorTestEnable { get; set; }

        protected RasterizerState(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }
    }
}
