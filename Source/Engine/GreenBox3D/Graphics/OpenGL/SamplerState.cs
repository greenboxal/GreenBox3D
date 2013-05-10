// SamplerState.cs
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
    public sealed class SamplerState : GraphicsResource
    {
        public static SamplerState PointWrap { get { return GraphicsDevice.ActiveDevice.SamplerPointWrap; } }
        public static SamplerState PointClamp { get { return GraphicsDevice.ActiveDevice.SamplerPointClamp; } }
        public static SamplerState LinearWrap { get { return GraphicsDevice.ActiveDevice.SamplerLinearWrap; } }
        public static SamplerState LinearClamp { get { return GraphicsDevice.ActiveDevice.SamplerLinearClamp; } }

        internal int SamplerID;

        private bool _bound;
        private TextureAddressMode _addressU, _addressV, _addressW;
        private TextureFilter _filter;
        private int _maxAnisotropy, _maxMipLevel;
        private float _mipMapLevelOfDetailBias;

        public SamplerState()
            : base(GraphicsDevice.ActiveDevice)
        {
            SamplerID = GL.GenSampler();

            AddressU = TextureAddressMode.Wrap;
            AddressV = TextureAddressMode.Wrap;
            AddressW = TextureAddressMode.Wrap;
            Filter = TextureFilter.Point;
            MaxAnisotropy = 4;
            MaxMipLevel = 1000;
            MipMapLevelOfDetailBias = 0;
        }

        public TextureAddressMode AddressU
        {
            get { return _addressU; }
            set
            {
                ThrowIfBound();
                if (_addressU != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureWrapS, (int)GLUtils.GetWrapMode(value));
                    _addressU = value;
                }
            }
        }

        public TextureAddressMode AddressV
        {
            get { return _addressV; }
            set
            {
                ThrowIfBound();
                if (_addressV != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureWrapT, (int)GLUtils.GetWrapMode(value));
                    _addressV = value;
                }
            }
        }

        public TextureAddressMode AddressW
        {
            get { return _addressW; }
            set
            {
                ThrowIfBound();
                if (_addressW != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureWrapR, (int)GLUtils.GetWrapMode(value));
                    _addressW = value;
                }
            }
        }

        public TextureFilter Filter
        {
            get { return _filter; }
            set
            {
                ThrowIfBound();
                if (_filter != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureMagFilter, (int)GLUtils.GetMagFilter(value));
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureMinFilter, (int)GLUtils.GetMinFilter(value));
                    _filter = value;
                }
            }
        }

        public int MaxAnisotropy
        {
            get { return _maxAnisotropy; }
            set
            {
                ThrowIfBound();
                if (_maxAnisotropy != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureMaxAnisotropyExt, value);
                    _maxAnisotropy = value;
                }
            }
        }

        public int MaxMipLevel
        {
            get { return _maxMipLevel; }
            set
            {
                ThrowIfBound();
                if (_maxMipLevel != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureMaxLod, value);
                    _maxMipLevel = value;
                }
            }
        }

        public float MipMapLevelOfDetailBias
        {
            get { return _mipMapLevelOfDetailBias; }
            set
            {
                ThrowIfBound();
                if (_mipMapLevelOfDetailBias != value)
                {
                    GL.SamplerParameter(SamplerID, SamplerParameter.TextureLodBias, value);
                    _mipMapLevelOfDetailBias = value;
                }
            }
        }

        public void Bond(GraphicsDevice graphicsDevice)
        {
            if (graphicsDevice != GraphicsDevice)
                throw new InvalidOperationException("A SamplerState can only be bound to the GraphicsDevice that created it");

            _bound = true;
        }

        protected override void Dispose(bool disposing)
        {
            if (SamplerID != -1)
            {
                GL.DeleteSampler(SamplerID);
                SamplerID = -1;
            }

            base.Dispose(disposing);
        }

        private void ThrowIfBound()
        {
            if (_bound)
                throw new InvalidOperationException("You can't modify a SamplerState after it's bound to a GraphicsDevice");
        }
    }
}

#endif
