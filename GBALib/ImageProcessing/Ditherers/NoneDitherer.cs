using SkiaSharp;
using System.Collections.Generic;
using System.Linq;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class NoneDitherer : Ditherer
    {
        public override SKBitmap Dither(SKBitmap bitmap, List<SKColor> palette)
        {
            SKBitmap ditheredBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            float[][] errorMatrix = new float[bitmap.Height][];
            for (int i = 0; i < bitmap.Height; i++)
            {
                errorMatrix[i] = new float[bitmap.Width * 3];
            }

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    SKColor originalColor = bitmap.GetPixel(x, y);

                    SKColor quantizedColor = GetClosestColor(palette, originalColor);
                    ditheredBitmap.SetPixel(x, y, quantizedColor);
                }
            }

            return ditheredBitmap;
        }
    }
}
