using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GreenBox3D.Platform.Windows.Graphics
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

        internal static TextureMinFilter GetMinFilter(TextureFilter value)
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

        public static void GetOpenGLTextureFormat(SurfaceFormat format, out PixelInternalFormat glInternalFormat,
                                                  out PixelFormat glFormat, out PixelType glType)
        {
            glInternalFormat = PixelInternalFormat.Rgba;
            glFormat = PixelFormat.Rgba;
            glType = PixelType.UnsignedByte;

            switch (format)
            {
                case SurfaceFormat.Color:
                    glInternalFormat = PixelInternalFormat.Rgba;
                    glFormat = PixelFormat.Rgba;
                    glType = PixelType.UnsignedByte;
                    break;
                case SurfaceFormat.Bgr565:
                    glInternalFormat = PixelInternalFormat.Rgb;
                    glFormat = PixelFormat.Rgb;
                    glType = PixelType.UnsignedShort565;
                    break;
                case SurfaceFormat.Bgra4444:
                    glInternalFormat = PixelInternalFormat.Rgba4;
                    glFormat = PixelFormat.Rgba;
                    glType = PixelType.UnsignedShort4444;
                    break;
                case SurfaceFormat.Bgra5551:
                    glInternalFormat = PixelInternalFormat.Rgba;
                    glFormat = PixelFormat.Rgba;
                    glType = PixelType.UnsignedShort5551;
                    break;
                case SurfaceFormat.Alpha8:
                    glInternalFormat = PixelInternalFormat.Luminance;
                    glFormat = PixelFormat.Luminance;
                    glType = PixelType.UnsignedByte;
                    break;
                case SurfaceFormat.Dxt1:
                    glInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt1Ext;
                    glFormat = (PixelFormat)All.CompressedTextureFormats;
                    break;
                case SurfaceFormat.Dxt3:
                    glInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    glFormat = (PixelFormat)All.CompressedTextureFormats;
                    break;
                case SurfaceFormat.Dxt5:
                    glInternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    glFormat = (PixelFormat)All.CompressedTextureFormats;
                    break;

                case SurfaceFormat.Single:
                    glInternalFormat = PixelInternalFormat.R32f;
                    glFormat = PixelFormat.Red;
                    glType = PixelType.Float;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
