using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
{
    public sealed class GLBlendState : BlendState
    {
        private bool _bound;
        private BlendFunction _alphaBlendFunction;
        private Blend _alphaDestinationBlend;
        private Blend _alphaSourceBlend;
        private Color _blendFactor;
        private BlendFunction _colorBlendFunction;
        private Blend _colorDestinationBlend;
        private Blend _colorSourceBlend;
        private ColorWriteChannels _colorWriteChannels;
        private int _multiSampleMask;

        public override BlendFunction AlphaBlendFunction
        {
            get { return _alphaBlendFunction; }
            set
            {
                ThrowIfBound();
                _alphaBlendFunction = value;
            }
        }

        public override Blend AlphaDestinationBlend
        {
            get { return _alphaDestinationBlend; }
            set
            {
                ThrowIfBound();
                _alphaDestinationBlend = value;
            }
        }

        public override Blend AlphaSourceBlend
        {
            get { return _alphaSourceBlend; }
            set
            {
                ThrowIfBound();
                _alphaSourceBlend = value;
            }
        }

        public override Color BlendFactor
        {
            get { return _blendFactor; }
            set
            {
                ThrowIfBound();
                _blendFactor = value;
            }
        }

        public override BlendFunction ColorBlendFunction
        {
            get { return _colorBlendFunction; }
            set
            {
                ThrowIfBound();
                _colorBlendFunction = value;
            }
        }

        public override Blend ColorDestinationBlend
        {
            get { return _colorDestinationBlend; }
            set
            {
                ThrowIfBound();
                _colorDestinationBlend = value;
            }
        }

        public override Blend ColorSourceBlend
        {
            get { return _colorSourceBlend; }
            set
            {
                ThrowIfBound();
                _colorSourceBlend = value;
            }
        }

        public override ColorWriteChannels ColorWriteChannels
        {
            get { return _colorWriteChannels; }
            set
            {
                ThrowIfBound();
                _colorWriteChannels = value;
            }
        }

        public override int MultiSampleMask
        {
            get { return _multiSampleMask; }
            set
            {
                ThrowIfBound();
                _multiSampleMask = value;
            }
        }

        public GLBlendState(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
            _alphaBlendFunction = BlendFunction.Add;
            _alphaDestinationBlend = Blend.One;
            _alphaSourceBlend = Blend.One;
            _blendFactor = Color.White;
            _colorBlendFunction = BlendFunction.Add;
            _colorDestinationBlend = Blend.One;
            _colorSourceBlend = Blend.One;
            _colorWriteChannels = ColorWriteChannels.All;
            _multiSampleMask = -1;
        }

        public void Bond(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice != GraphicsDevice)
                throw new InvalidOperationException("A BlendState can only be bound to the GraphicsDevice that created it");

            _bound = true;
        }

        public void ApplyState()
        {
            GL.BlendFuncSeparate(GLUtils.GetBlendingFactorSrc(_colorSourceBlend),
                                 GLUtils.GetBlendingFactorDest(_colorDestinationBlend),
                                 GLUtils.GetBlendingFactorSrc(_alphaSourceBlend),
                                 GLUtils.GetBlendingFactorDest(_alphaDestinationBlend));
            GL.BlendEquationSeparate(GLUtils.GetBlendEquationMode(_colorBlendFunction),
                                     GLUtils.GetBlendEquationMode(_alphaBlendFunction));
            GL.BlendColor(_blendFactor.R / 255.0f, _blendFactor.G / 255.0f, _blendFactor.B / 255.0f, _blendFactor.A / 255.0f);
            GL.ColorMask((_colorWriteChannels & ColorWriteChannels.Red) != ColorWriteChannels.None,
                         (_colorWriteChannels & ColorWriteChannels.Green) != ColorWriteChannels.None,
                         (_colorWriteChannels & ColorWriteChannels.Blue) != ColorWriteChannels.None,
                         (_colorWriteChannels & ColorWriteChannels.Alpha) != ColorWriteChannels.None);
            GL.SampleMask(0, _multiSampleMask);
        }

        private void ThrowIfBound()
        {
            if (_bound)
                throw new InvalidOperationException("You can't modify a BlendState after it's bound to a GraphicsDevice");
        }
    }
}
