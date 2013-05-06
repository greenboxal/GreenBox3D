#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class RasterizerState
    {
        private static readonly RasterizerState _cullClockwise, _cullCounterClockwise, _cullNone;

        public static RasterizerState CullClockwise { get { return _cullClockwise; } }
        public static RasterizerState CullCounterClockwise { get { return _cullCounterClockwise; } }
        public static RasterizerState CullNone { get { return _cullNone; } }

        static RasterizerState()
        {
            _cullClockwise = new RasterizerState()
            {
                CullMode = CullMode.CullClockwiseFace
            };
            _cullClockwise.Bond();

            _cullCounterClockwise = new RasterizerState()
            {
                CullMode = CullMode.CullCounterClockwiseFace
            };
            _cullCounterClockwise.Bond();

            _cullNone = new RasterizerState()
            {
                CullMode = CullMode.None
            };
            _cullNone.Bond();
        }

        private bool _bound;
        private CullMode _cullMode;
        private FillMode _fillMode;
        private bool _multiSampleAntiAlias, _scissorTestEnable;

        public CullMode CullMode
        {
            get { return _cullMode; }
            set
            {
                ThrowIfBound();
                _cullMode = value;
            }
        }

        public FillMode FillMode
        {
            get { return _fillMode; }
            set
            {
                ThrowIfBound();
                _fillMode = value;
            }
        }

        public bool MultiSampleAntiAlias
        {
            get { return _multiSampleAntiAlias; }
            set
            {
                ThrowIfBound();
                _multiSampleAntiAlias = value;
            }
        }

        public bool ScissorTestEnable
        {
            get { return _scissorTestEnable; }
            set
            {
                ThrowIfBound();
                _scissorTestEnable = value;
            }
        }

        public RasterizerState()
        {
            _cullMode = CullMode.CullCounterClockwiseFace;
            _fillMode = FillMode.Solid;
            _multiSampleAntiAlias = true;
            _scissorTestEnable = false;
        }

        internal void Bond()
        {
            _bound = true;
        }

        internal void ApplyState()
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

#endif
