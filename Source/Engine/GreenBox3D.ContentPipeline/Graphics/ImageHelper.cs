using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.ContentPipeline.CompilerServices;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public static class ImageHelper
    {
        public unsafe static BitmapContent BitmapContentFromBitmap(Bitmap bitmap)
        {
            BitmapContent pbmp;
            bool direct = false;

            switch (bitmap.PixelFormat)
            {
                case PixelFormat.Format32bppArgb:
                    pbmp = new PixelBitmapContent<Color>(bitmap.Width, bitmap.Height);
                    direct = true;
                    break;
                case PixelFormat.Format24bppRgb:
                    pbmp = new PixelBitmapContent<Color>(bitmap.Width, bitmap.Height);
                    break;
                default:
                    return null;
            }

            BitmapData bd = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                            ImageLockMode.ReadOnly,
                                            bitmap.PixelFormat);

            if (direct)
            {
                byte[] data = new byte[bd.Stride * bd.Height];
                Marshal.Copy(bd.Scan0, data, 0, data.Length);
                pbmp.SetPixelData(data);
            }
            else
            {
                switch (bitmap.PixelFormat)
                {
                    case PixelFormat.Format24bppRgb:
                        byte[] data = new byte[pbmp.Width * pbmp.Height * 4];
                        byte* ptr = (byte*)bd.Scan0;

                        for (int x = 0; x < pbmp.Width; x++)
                        {
                            for (int y = 0; y < pbmp.Height; y++)
                            {
                                int offset = (y * pbmp.Width + x) * 3;
                                int boffset = (y * pbmp.Width + x) * 4;

                                data[boffset] = ptr[offset + 2];
                                data[boffset + 1] = ptr[offset + 1];
                                data[boffset + 2] = ptr[offset];
                                data[boffset + 3] = 255;
                            }
                        }

                        pbmp.SetPixelData(data);
                        break;
                }
            }

            bitmap.UnlockBits(bd);

            return pbmp;
        }

        public static Bitmap BitmapContentToBitmap(BitmapContent source, Rectangle sourceRegion)
        {
            Bitmap bmp = new Bitmap(sourceRegion.Width, sourceRegion.Height);
            PixelBitmapContent<Color> pbmp = source as PixelBitmapContent<Color>;
            Point p = sourceRegion.Location;

            if (pbmp == null)
            {
                pbmp = new PixelBitmapContent<Color>(sourceRegion.Width, sourceRegion.Height);
                BitmapContent.Copy(source, sourceRegion, pbmp,
                                   new Rectangle(0, 0, sourceRegion.Width, sourceRegion.Height));
                p = new Point(0, 0);
            }

            pbmp.CopyToBitmap(bmp, p);

            return bmp;
        }
    }
}
