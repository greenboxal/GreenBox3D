// GLUtils.cs
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
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

using OPFormat = OpenTK.Graphics.OpenGL.PixelFormat;
using OPType = OpenTK.Graphics.OpenGL.PixelType;
using OBAccess = OpenTK.Graphics.OpenGL.BufferAccess;

namespace GreenBox3D.Graphics
{
    internal static class GLUtils
    {
        public static TextureWrapMode GetWrapMode(TextureAddressMode value)
        {
            switch (value)
            {
                case TextureAddressMode.Wrap:
                    return TextureWrapMode.Repeat;
                case TextureAddressMode.Mirror:
                    return TextureWrapMode.MirroredRepeat;
                case TextureAddressMode.Clamp:
                    return TextureWrapMode.Clamp;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static TextureMinFilter GetMinFilter(TextureFilter value)
        {
            switch (value)
            {
                case TextureFilter.Point:
                    return TextureMinFilter.Nearest;
                case TextureFilter.Linear:
                    return TextureMinFilter.Linear;
                case TextureFilter.LinearMipPoint:
                    return TextureMinFilter.LinearMipmapNearest;
                case TextureFilter.PointMipLinear:
                    return TextureMinFilter.NearestMipmapLinear;
                case TextureFilter.MinLinearMagPointMipLinear:
                    return TextureMinFilter.LinearMipmapLinear;
                case TextureFilter.MinLinearMagPointMipPoint:
                    return TextureMinFilter.LinearMipmapNearest;
                case TextureFilter.MinPointMagLinearMipLinear:
                    return TextureMinFilter.NearestMipmapLinear;
                case TextureFilter.MinPointMagLinearMipPoint:
                    return TextureMinFilter.NearestMipmapNearest;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static TextureMagFilter GetMagFilter(TextureFilter value)
        {
            switch (value)
            {
                case TextureFilter.Point:
                    return TextureMagFilter.Nearest;
                case TextureFilter.Linear:
                    return TextureMagFilter.Linear;
                case TextureFilter.LinearMipPoint:
                    return TextureMagFilter.Linear;
                case TextureFilter.PointMipLinear:
                    return TextureMagFilter.Nearest;
                case TextureFilter.MinLinearMagPointMipLinear:
                    return TextureMagFilter.Nearest;
                case TextureFilter.MinLinearMagPointMipPoint:
                    return TextureMagFilter.Nearest;
                case TextureFilter.MinPointMagLinearMipLinear:
                    return TextureMagFilter.Linear;
                case TextureFilter.MinPointMagLinearMipPoint:
                    return TextureMagFilter.Linear;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static BlendEquationMode GetBlendEquationMode(BlendFunction value)
        {
            switch (value)
            {
                case BlendFunction.Add:
                    return BlendEquationMode.FuncAdd;
                case BlendFunction.Subtract:
                    return BlendEquationMode.FuncSubtract;
                case BlendFunction.ReverseSubtract:
                    return BlendEquationMode.FuncReverseSubtract;
                case BlendFunction.Min:
                    return BlendEquationMode.Min;
                case BlendFunction.Max:
                    return BlendEquationMode.Max;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static BlendingFactorSrc GetBlendingFactorSrc(Blend value)
        {
            switch (value)
            {
                case Blend.One:
                    return BlendingFactorSrc.One;
                case Blend.Zero:
                    return BlendingFactorSrc.Zero;
                case Blend.BlendFactor:
                    return BlendingFactorSrc.ConstantColor;
                case Blend.DestinationAlpha:
                    return BlendingFactorSrc.DstAlpha;
                case Blend.DestinationColor:
                    return BlendingFactorSrc.DstColor;
                case Blend.InverseBlendFactor:
                    return BlendingFactorSrc.OneMinusConstantColor;
                case Blend.InverseDestinationAlpha:
                    return BlendingFactorSrc.OneMinusDstAlpha;
                case Blend.InverseDestinationColor:
                    return BlendingFactorSrc.OneMinusDstColor;
                case Blend.InverseSourceAlpha:
                    return BlendingFactorSrc.OneMinusSrcAlpha;
                case Blend.InverseSourceColor:
                    return (BlendingFactorSrc)All.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return BlendingFactorSrc.SrcAlpha;
                case Blend.SourceAlphaSaturation:
                    return BlendingFactorSrc.SrcAlphaSaturate;
                case Blend.SourceColor:
                    return (BlendingFactorSrc)All.SrcColor;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static BlendingFactorDest GetBlendingFactorDest(Blend value)
        {
            switch (value)
            {
                case Blend.One:
                    return BlendingFactorDest.One;
                case Blend.Zero:
                    return BlendingFactorDest.Zero;
                case Blend.BlendFactor:
                    return BlendingFactorDest.ConstantColor;
                case Blend.DestinationAlpha:
                    return BlendingFactorDest.DstAlpha;
                case Blend.InverseBlendFactor:
                    return BlendingFactorDest.OneMinusConstantColor;
                case Blend.InverseDestinationAlpha:
                    return BlendingFactorDest.OneMinusDstAlpha;
                case Blend.InverseSourceAlpha:
                    return BlendingFactorDest.OneMinusSrcAlpha;
                case Blend.InverseSourceColor:
                    return (BlendingFactorDest)All.OneMinusSrcColor;
                case Blend.SourceAlpha:
                    return BlendingFactorDest.SrcAlpha;
                case Blend.SourceColor:
				    return (BlendingFactorDest)All.SrcColor;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static FrontFaceDirection GetFrontFaceDirection(CullMode value)
        {
            switch (value)
            {
                case CullMode.CullClockwiseFace:
                    return FrontFaceDirection.Cw;
                case CullMode.CullCounterClockwiseFace:
                    return FrontFaceDirection.Ccw;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static PolygonMode GetPolygonMode(FillMode value)
        {
            switch (value)
            {
                case FillMode.Solid:
                    return PolygonMode.Fill;
                case FillMode.WireFrame:
                    return PolygonMode.Line;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static PixelInternalFormat GetPixelInternalFormat(SurfaceFormat value)
        {
            switch (value)
            {
                case SurfaceFormat.R:
                    return PixelInternalFormat.R8;
                case SurfaceFormat.Rg:
                    return PixelInternalFormat.Rg8;
                case SurfaceFormat.Rgb:
                    return PixelInternalFormat.Rgb8;
                case SurfaceFormat.Color:
                    return PixelInternalFormat.Rgba;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static OPFormat GetPixelFormat(PixelFormat value)
        {
            switch (value)
            {
                case PixelFormat.R:
                    return OPFormat.Red;
                case PixelFormat.Rg:
                    return OPFormat.Rg;
                case PixelFormat.Rgb:
                    return OPFormat.Rgb;
                case PixelFormat.Rgba:
                    return OPFormat.Rgba;
                case PixelFormat.Bgr:
                    return OPFormat.Bgr;
                case PixelFormat.Bgra:
                    return OPFormat.Bgra;
                case PixelFormat.Depth:
                    return OPFormat.DepthComponent;
                case PixelFormat.DepthStencil:
                    return OPFormat.DepthStencil;
                default:
                    throw new ArgumentException("value");
            }
        }

        public static OPType GetPixelType(PixelType value)
        {
            switch (value)
            {
                case PixelType.Byte:
                    return OPType.Byte;
                case PixelType.UnsignedByte:
                    return OPType.UnsignedByte;
                case PixelType.Short:
                    return OPType.Short;
                case PixelType.UnsignedShort:
                    return OPType.UnsignedShort;
                case PixelType.Int:
                    return OPType.UnsignedInt;
                case PixelType.UnsignedInt8888:
                    return OPType.UnsignedInt8888;
                case PixelType.UnsignedInt8888Reversed:
                    return OPType.UnsignedInt8888Reversed;
                case PixelType.Float:
                    return OPType.Float;
                case PixelType.HalfFloat:
                    return OPType.HalfFloat;
                default:
                    throw new ArgumentException("value");
            }
        }

        internal static OBAccess GetBufferAccess(BufferAccess value)
        {
            switch (value)
            {
                case BufferAccess.ReadOnly:
                    return OBAccess.ReadOnly;
                case BufferAccess.ReadWrite:
                    return OBAccess.ReadWrite;
                case BufferAccess.WriteOnly:
                    return OBAccess.ReadWrite;
                default:
                    throw new ArgumentException("value");
            }
        }
    }
}

#endif
