using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GreenBox3D.Graphics;

namespace GreenBox3D.ContentPipeline.Graphics
{
    public abstract class BitmapContent
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        public abstract void SetPixelData(byte[] sourceData);
        public abstract byte[] GetPixelData();
        public abstract bool TryGetFormat(out SurfaceFormat format);

        protected abstract bool TryCopyTo(BitmapContent destinationBitmap, Rectangle sourceRegion,
                                          Rectangle destinationRegion);

        protected abstract bool TryCopyFrom(BitmapContent sourceBitmap, Rectangle sourceRegion,
                                            Rectangle destinationRegion);

        public static void Copy(BitmapContent sourceBitmap, Rectangle sourceRegion, BitmapContent destinationBitmap,
                                Rectangle destinationRegion)
        {
            PixelBitmapContent<Vector4> copy1, copy2;
            Rectangle rect1, rect2;

            ValidateCopyArguments(sourceBitmap, sourceRegion, destinationBitmap, destinationRegion);

            // Try a direct copy
            if (sourceBitmap.TryCopyTo(destinationBitmap, sourceRegion, destinationRegion) || destinationBitmap.TryCopyFrom(sourceBitmap, sourceRegion, destinationRegion))
                return;

            // Try two step copy: src -> PixelBitmap, PixelBitmap -> dst
            copy1 = new PixelBitmapContent<Vector4>(destinationRegion.Width, destinationRegion.Height);
            rect1 = new Rectangle(0, 0, copy1.Width, copy1.Height);

            if (sourceBitmap.TryCopyTo(copy1, sourceRegion, rect1) && destinationBitmap.TryCopyFrom(copy1, rect1, destinationRegion))
                return;

            copy1 = new PixelBitmapContent<Vector4>(sourceBitmap.Width, sourceBitmap.Height);
            rect1 = new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height);

            copy2 = new PixelBitmapContent<Vector4>(destinationBitmap.Width, destinationBitmap.Height);
            rect2 = new Rectangle(0, 0, destinationBitmap.Width, destinationBitmap.Height);

            // Try four step copy: src -> PixelBitmap1, dst -> PixelBitmap2, PixelBitmap1 -> PixelBitmap2, PixelBitmap2 -> dst
            if (!sourceBitmap.TryCopyTo(copy1, rect1, rect1) || !destinationBitmap.TryCopyTo(copy2, rect2, rect2) || (!copy2.TryCopyFrom(copy1, sourceRegion, destinationRegion) || !destinationBitmap.TryCopyFrom(copy2, rect2, rect2)))
                throw new NotSupportedException();
        }

        public static void Copy(BitmapContent sourceBitmap, BitmapContent destinationBitmap)
        {
            Copy(sourceBitmap, new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), destinationBitmap,
                 new Rectangle(0, 0, destinationBitmap.Width, destinationBitmap.Height));
        }

        protected static void ValidateCopyArguments(BitmapContent sourceBitmap, Rectangle sourceRegion, BitmapContent destinationBitmap, Rectangle destinationRegion)
        {
            if (sourceBitmap == null)
                throw new ArgumentNullException("sourceBitmap");

            if (destinationBitmap == null)
                throw new ArgumentNullException("destinationBitmap");

            if (sourceRegion.Left < 0 || sourceRegion.Top < 0 || (sourceRegion.Width < 0 || sourceRegion.Height < 0) || (sourceRegion.Right > sourceBitmap.Width || sourceRegion.Bottom > sourceBitmap.Height))
                throw new ArgumentOutOfRangeException("sourceRegion");

            if (destinationRegion.Left < 0 || destinationRegion.Top < 0 || (destinationRegion.Width < 0 || destinationRegion.Height < 0) || (destinationRegion.Right > destinationBitmap.Width || destinationRegion.Bottom > destinationBitmap.Height))
                throw new ArgumentOutOfRangeException("destinationRegion");
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}x{2}", GetType().Name, Width, Height);
        }

        protected static bool ResizeCopy(BitmapContent source, Rectangle sourceRegion,
                                         BitmapContent destination, Rectangle destinationRegion)
        {
            Bitmap bitmap = ImageHelper.BitmapContentToBitmap(source, sourceRegion);
            Bitmap dest = new Bitmap(destinationRegion.Width, destinationRegion.Height);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(dest);

            g.DrawImage(bitmap, new System.Drawing.Rectangle(0, 0, dest.Width, dest.Height), 0, 0, bitmap.Width,
                        bitmap.Height, GraphicsUnit.Pixel);
                
            try
            {
                Copy(ImageHelper.BitmapContentFromBitmap(dest), new Rectangle(0, 0, dest.Width, dest.Height),
                     destination, destinationRegion);
            }
            catch
            {
                return false;
            }

            bitmap.Dispose();
            dest.Dispose();
            g.Dispose();

            return true;
        }
    }
}
