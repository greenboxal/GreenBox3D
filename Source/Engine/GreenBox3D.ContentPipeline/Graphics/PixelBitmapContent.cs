// PixelBitmapContent.cs
// 
// Copyright (c) 2013 The GreenBox Development LLC, all rights reserved
// 
// This file is a proprietary part of GreenBox3D, disclosing the content
// of this file without the owner consent may lead to legal actions
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public class PixelBitmapContent<T> : BitmapContent
    {
        private static Converter<Vector4, T> _packer;
        private static Converter<T, Vector4> _unpacker;

        private readonly T[][] _pixelData;

        public PixelBitmapContent(int width, int height)
        {
            StaticInit();

            Width = width;
            Height = height;

            _pixelData = new T[height][];

            for (int i = 0; i < height; i++)
                _pixelData[i] = new T[width];
        }

        private static void StaticInit()
        {
            try
            {
                if (_packer == null)
                    _packer = (Converter<Vector4, T>)VectorUtils.GetPacker(typeof(T));

                if (_unpacker == null)
                    _unpacker = (Converter<T, Vector4>)VectorUtils.GetUnpacker(typeof(T));
            }
            catch
            {
                throw new InvalidOperationException("This type can't be a Bitmap");
            }
        }

        public T[] GetRow(int y)
        {
            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            return _pixelData[y];
        }

        public T GetPixel(int x, int y)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            return _pixelData[y][x];
        }

        public void SetPixel(int x, int y, T value)
        {
            if (x < 0 || x >= Width)
                throw new ArgumentOutOfRangeException("x");

            if (y < 0 || y >= Height)
                throw new ArgumentOutOfRangeException("y");

            _pixelData[y][x] = value;
        }

        public override void SetPixelData(byte[] sourceData)
        {
            int stride = Marshal.SizeOf(typeof(T)) * Width;

            if (sourceData.Length != stride * Height)
                throw new ArgumentException("Source array is too small", "sourceData");

            for (int y = 0; y < Height; y++)
            {
                T[] row = GetRow(y);
                GCHandle handle = GCHandle.Alloc(row, GCHandleType.Pinned);
                IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);

                Marshal.Copy(sourceData, y * stride, address, stride);
                handle.Free();
            }
        }

        public override byte[] GetPixelData()
        {
            int stride = Marshal.SizeOf(typeof(T)) * Width;
            byte[] data = new byte[stride * Height];

            for (int y = 0; y < Height; y++)
            {
                T[] row = GetRow(y);
                GCHandle handle = GCHandle.Alloc(row, GCHandleType.Pinned);
                IntPtr address = Marshal.UnsafeAddrOfPinnedArrayElement(row, 0);

                Marshal.Copy(address, data, stride * y, stride);
                handle.Free();
            }

            return data;
        }

        public override bool TryGetFormat(out SurfaceFormat format)
        {
            return VectorUtils.TryGetSurfaceFormat(typeof(T), out format);
        }

        protected override bool TryCopyTo(BitmapContent destinationBitmap, Rectangle sourceRegion,
                                          Rectangle destinationRegion)
        {
            ValidateCopyArguments(this, sourceRegion, destinationBitmap, destinationRegion);

            if (sourceRegion.Width != destinationRegion.Width || sourceRegion.Height != destinationRegion.Height)
                return ResizeCopy(this, sourceRegion, destinationBitmap, destinationRegion);

            Point sourceLocation = new Point(sourceRegion.X, sourceRegion.Y);
            PixelBitmapContent<T> destinationBitmap1 = destinationBitmap as PixelBitmapContent<T>;

            if (destinationBitmap1 != null)
            {
                CopySameType(this, sourceLocation, destinationBitmap1, destinationRegion);
                return true;
            }
            else
            {
                PixelBitmapContent<Vector4> destinationBitmap2 = destinationBitmap as PixelBitmapContent<Vector4>;

                if (destinationBitmap2 == null)
                    return false;

                CopyToVector4(destinationBitmap2, sourceLocation, destinationRegion);

                return true;
            }
        }

        protected override bool TryCopyFrom(BitmapContent sourceBitmap, Rectangle sourceRegion,
                                            Rectangle destinationRegion)
        {
            ValidateCopyArguments(sourceBitmap, sourceRegion, this, destinationRegion);

            if (sourceRegion.Width != destinationRegion.Width || sourceRegion.Height != destinationRegion.Height)
                return ResizeCopy(sourceBitmap, sourceRegion, this, destinationRegion);

            Point sourceLocation = new Point(sourceRegion.X, sourceRegion.Y);
            PixelBitmapContent<T> sourceBitmap1 = sourceBitmap as PixelBitmapContent<T>;

            if (sourceBitmap1 != null)
            {
                CopySameType(sourceBitmap1, sourceLocation, this, destinationRegion);
                return true;
            }
            else
            {
                PixelBitmapContent<Vector4> sourceBitmap2 = sourceBitmap as PixelBitmapContent<Vector4>;

                if (sourceBitmap2 == null)
                    return false;

                CopyFromVector4(sourceBitmap2, sourceLocation, destinationRegion);

                return true;
            }
        }

        private static void CopySameType(PixelBitmapContent<T> sourceBitmap, Point sourceLocation,
                                         PixelBitmapContent<T> destinationBitmap, Rectangle destinationRegion)
        {
            int num1;
            int num2;
            int num3;

            if (ReferenceEquals(sourceBitmap, destinationBitmap) && sourceLocation.Y < destinationRegion.Top)
            {
                num1 = destinationRegion.Height - 1;
                num2 = -1;
                num3 = -1;
            }
            else
            {
                num1 = 0;
                num2 = destinationRegion.Height;
                num3 = 1;
            }

            int num4 = num1;

            while (num4 != num2)
            {
                T[] row1 = sourceBitmap.GetRow(sourceLocation.Y + num4);
                T[] row2 = destinationBitmap.GetRow(destinationRegion.Y + num4);

                Array.Copy(row1, sourceLocation.X, row2, destinationRegion.Left, destinationRegion.Width);
                num4 += num3;
            }
        }

        private void CopyFromVector4(PixelBitmapContent<Vector4> sourceBitmap, Point sourceLocation,
                                     Rectangle destinationRegion)
        {
            for (int index1 = 0; index1 < destinationRegion.Height; ++index1)
            {
                Vector4[] row1 = sourceBitmap.GetRow(sourceLocation.Y + index1);
                T[] row2 = GetRow(destinationRegion.Y + index1);

                for (int index2 = 0; index2 < destinationRegion.Width; ++index2)
                    row2[destinationRegion.Left + index2] = _packer(row1[sourceLocation.X + index2]);
            }
        }

        private void CopyToVector4(PixelBitmapContent<Vector4> destinationBitmap, Point sourceLocation,
                                   Rectangle destinationRegion)
        {
            for (int index1 = 0; index1 < destinationRegion.Height; ++index1)
            {
                T[] row1 = GetRow(sourceLocation.Y + index1);
                Vector4[] row2 = destinationBitmap.GetRow(destinationRegion.Y + index1);

                for (int index2 = 0; index2 < destinationRegion.Width; ++index2)
                    row2[destinationRegion.Left + index2] = _unpacker(row1[sourceLocation.X + index2]);
            }
        }

        public void ReplaceColor(T originalColor, T newColor)
        {
            foreach (T[] row in _pixelData)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    if (row[i].Equals(originalColor))
                        row[i] = newColor;
                }
            }
        }

        internal unsafe void CopyToBitmap(Bitmap bmp, Point point)
        {
            if (typeof(T) != typeof(Color))
                throw new NotSupportedException();

            BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                                         ImageLockMode.WriteOnly,
                                         PixelFormat.Format32bppArgb);
            byte* ptr = (byte*)bd.Scan0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    int offset = (y * bmp.Width + x) * 4;
                    Color c = (Color)(object)_pixelData[y][x];
                    ptr[offset] = c.B;
                    ptr[offset + 1] = c.G;
                    ptr[offset + 2] = c.R;
                    ptr[offset + 3] = c.A;
                }
            }

            bmp.UnlockBits(bd);
        }
    }
}
