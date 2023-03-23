using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace GBALib.ImageProcessing.Ditherers
{
    internal abstract class Ditherer
    {
        public abstract SKBitmap Dither(SKBitmap bitmap, List<SKColor> palette);

        protected SKColor GetClosestColor(List<SKColor> palette, SKColor color)
        {
            return palette.OrderBy(c => ColorHelper.Instance.ColorDistance(c, color)).First();
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
