using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public sealed class GLRasterizerState : RasterizerState
    {
        private bool _bound;
        private CullMode _cullMode;
        private FillMode _fillMode;
        private bool _multiSampleAntiAlias, _scissorTestEnable;

        public override CullMode CullMode
        {
            get { return _cullMode; }
            set
            {
                ThrowIfBound();
                _cullMode = value;
            }
        }

        public override FillMode FillMode
        {
            get { return _fillMode; }
            set
            {
                ThrowIfBound();
                _fillMode = value;
            }
        }

        public override bool MultiSampleAntiAlias
        {
            get { return _multiSampleAntiAlias; }
            set
            {
                ThrowIfBound();
                _multiSampleAntiAlias = value;
            }
        }

        public override bool ScissorTestEnable
        {
            get { return _scissorTestEnable; }
            set
            {
                ThrowIfBound();
                _scissorTestEnable = value;
            }
        }

        public GLRasterizerState(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _cullMode = CullMode.CullCounterClockwiseFace;
            _fillMode = FillMode.Solid;
            _multiSampleAntiAlias = true;
            _scissorTestEnable = false;
        }

        public void Bond(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice != GraphicsDevice)
                throw new InvalidOperationException("A RasterizerState can only be bound to the GraphicsDevice that created it");

            _bound = true;
        }

        public void ApplyState()
        {
            if (_cullMode != CullMode.None)
            {
                GL.Enable(EnableCap.CullFace);
                GL.FrontFace(GLUtils.GetFrontFaceDirection(_cullMode));
            }
            else
            {
                GL.Disable(EnableCap.CullFace);
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, GLUtils.GetPolygonMode(_fillMode));

            if (_multiSampleAntiAlias)
                GL.Enable(EnableCap.Multisample);
            else
                GL.Disable(EnableCap.Multisample);

            if (_scissorTestEnable)
                GL.Enable(EnableCap.ScissorTest);
            else
                GL.Disable(EnableCap.ScissorTest);
        }

        private void ThrowIfBound()
        {
            if (_bound)
                throw new InvalidOperationException("You can't modify a RasterizerState after it's bound to a GraphicsDevice");
        }
    }
}
