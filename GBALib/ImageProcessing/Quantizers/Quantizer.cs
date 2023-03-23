using System.Collections.Generic;
using SkiaSharp;

namespace GBALib.ImageProcessing.Quantizers
{
    internal abstract class Quantizer
    {
        public abstract List<SKColor> Quantize(SKBitmap image, int colors);
    }
}
