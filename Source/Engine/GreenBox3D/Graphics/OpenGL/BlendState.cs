// BlendState.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
#if DESKTOP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Graphics
{
    public sealed class BlendState
    {
        public static readonly BlendState Additive;
        public static readonly BlendState AlphaBlend;
        public static readonly BlendState NonPremultiplied;
        public static readonly BlendState Opaque;

        static BlendState()
        {
            Additive = new BlendState
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.One,
                AlphaDestinationBlend = Blend.One
            };
            Additive.Bond();

            AlphaBlend = new BlendState
            {
                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
            AlphaBlend.Bond();

            NonPremultiplied = new BlendState
            {
                ColorSourceBlend = Blend.SourceAlpha,
                AlphaSourceBlend = Blend.SourceAlpha,
                ColorDestinationBlend = Blend.InverseSourceAlpha,
                AlphaDestinationBlend = Blend.InverseSourceAlpha
            };
            NonPremultiplied.Bond();

            Opaque = new BlendState
            {
                ColorSourceBlend = Blend.One,
                AlphaSourceBlend = Blend.One,
                ColorDestinationBlend = Blend.Zero,
                AlphaDestinationBlend = Blend.Zero
            };
            Opaque.Bond();
        }

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

        public BlendFunction AlphaBlendFunction
        {
            get { return _alphaBlendFunction; }
            set
            {
                ThrowIfBound();
                _alphaBlendFunction = value;
            }
        }

        public Blend AlphaDestinationBlend
        {
            get { return _alphaDestinationBlend; }
            set
            {
                ThrowIfBound();
                _alphaDestinationBlend = value;
            }
        }

        public Blend AlphaSourceBlend
        {
            get { return _alphaSourceBlend; }
            set
            {
                ThrowIfBound();
                _alphaSourceBlend = value;
            }
        }

        public Color BlendFactor
        {
            get { return _blendFactor; }
            set
            {
                ThrowIfBound();
                _blendFactor = value;
            }
        }

        public BlendFunction ColorBlendFunction
        {
            get { return _colorBlendFunction; }
            set
            {
                ThrowIfBound();
                _colorBlendFunction = value;
            }
        }

        public Blend ColorDestinationBlend
        {
            get { return _colorDestinationBlend; }
            set
            {
                ThrowIfBound();
                _colorDestinationBlend = value;
            }
        }

        public Blend ColorSourceBlend
        {
            get { return _colorSourceBlend; }
            set
            {
                ThrowIfBound();
                _colorSourceBlend = value;
            }
        }

        public ColorWriteChannels ColorWriteChannels
        {
            get { return _colorWriteChannels; }
            set
            {
                ThrowIfBound();
                _colorWriteChannels = value;
            }
        }

        public int MultiSampleMask
        {
            get { return _multiSampleMask; }
            set
            {
                ThrowIfBound();
                _multiSampleMask = value;
            }
        }

        public BlendState()
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

        internal void Bond()
        {
            _bound = true;
        }

        internal void ApplyState()
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

#endif
