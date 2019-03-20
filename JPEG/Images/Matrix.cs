using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;

namespace JPEG.Images
{
    class Matrix
    {
        public readonly Pixel[,] Pixels;
        public readonly int Height;
        public readonly int Width;
        public readonly int RealHeight;
        public readonly int RealWidth;
				
        public Matrix(int height, int width, int realHeight, int realWidth)
        {
            Height = height;
            Width = width;
            RealHeight = realHeight;
            RealWidth = realWidth;

            Pixels = new Pixel[height,width];
        }

        public static explicit operator Matrix(Bitmap bmp)
        {
            var height = bmp.Height + (8 - bmp.Height % 8);
            var width = bmp.Width + (8 - bmp.Width % 8);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var matrix = new Matrix(height, width, data.Height, data.Width);
            var stride = data.Stride;

            unsafe
            {
                var ptr = (byte*)data.Scan0;
                var ptr1 = (BytePixel*)data.Scan0;

                BytePixel* PixelAt(int x, int y)
                {
                    return ptr1 + y * stride + x * sizeof(BytePixel);
                }

                for (var j = 0; j < data.Height; j++)
                {
                    for (var i = 0; i < data.Width; i++)
                    {
                        var offset = i * 3 + j * stride;
                        var b = ptr[offset];
                        var g = ptr[offset + 1];
                        var r = ptr[offset + 2];
                        var pixel = PixelAt(i, j);

                        matrix.Pixels[j, i] = new Pixel(r, g, b, PixelFormat.RGB);
                    }
                };
            }

            bmp.UnlockBits(data);

            return matrix;
        }

        public static explicit operator Bitmap(Matrix matrix)
        {
            var bmp = new Bitmap(matrix.RealWidth, matrix.RealHeight);
            var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            var stride = data.Stride;
            var bmpHeight = bmp.Height;
            var bmpWidth = bmp.Width;

            unsafe
            {
                var ptr = (byte*) data.Scan0;
                Parallel.For(0, bmpHeight, j =>
                {
                    for (var i = 0; i < bmpWidth; i++)
                    {
                        var pixel = matrix.Pixels[j, i];
                        var offset = i * 3 + j * stride;
                        ptr[offset] = (byte) ToByte(pixel.B);
                        ptr[offset + 1] = (byte) ToByte(pixel.G);
                        ptr[offset + 2] = (byte) ToByte(pixel.R);
                    }
                });

            }

            bmp.UnlockBits(data);

            return bmp;
        }

        public static int ToByte(double d)
        {
            var val = (int) d;
            if (val > byte.MaxValue)
                return byte.MaxValue;
            if (val < byte.MinValue)
                return byte.MinValue;
            return val;
        }
    }
}