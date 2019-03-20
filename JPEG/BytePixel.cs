using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace JPEG
{
    [StructLayout(LayoutKind.Sequential, Size = 4)]

    internal struct BytePixel
    {
        public readonly byte B;
        public readonly byte G;
        public readonly byte R;
    }
}
