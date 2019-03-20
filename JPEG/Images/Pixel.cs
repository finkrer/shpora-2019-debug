using System;
using System.Linq;

namespace JPEG.Images
{
    public struct Pixel
    {
        private readonly PixelFormat format;

        public Pixel(double firstComponent, double secondComponent, double thirdComponent, PixelFormat pixelFormat)
        {
            if (pixelFormat != PixelFormat.RGB && pixelFormat != PixelFormat.YCbCr)
                throw new FormatException("Unknown pixel format: " + pixelFormat);
            format = pixelFormat;
            c1 = firstComponent;
            c2 = secondComponent;
            c3 = thirdComponent;
        }

        private readonly double c1;
        private readonly double c2;
        private readonly double c3;

        public double R => format == PixelFormat.RGB ? c1 : (298.082 * Y + 408.583 * Cr) / 256.0 - 222.921;
        public double G => format == PixelFormat.RGB ? c2 : (298.082 * Y - 100.291 * Cb - 208.120 * Cr) / 256.0 + 135.576;
        public double B => format == PixelFormat.RGB ? c3 : (298.082 * Y + 516.412 * Cb) / 256.0 - 276.836;

        public double Y => format == PixelFormat.YCbCr ? c1 : 16.0 + (65.738 * R + 129.057 * G + 24.064 * B) / 256.0;
        public double Cb => format == PixelFormat.YCbCr ? c2 : 128.0 + (-37.945 * R - 74.494 * G + 112.439 * B) / 256.0;
        public double Cr => format == PixelFormat.YCbCr ? c3 : 128.0 + (112.439 * R - 94.154 * G - 18.285 * B) / 256.0;
    }
}