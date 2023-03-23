using SkiaSharp;
using System;
using System.Collections.Generic;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class SierraLiteDitherer : Ditherer
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
                    float r = Math.Clamp(originalColor.Red + errorMatrix[y][x * 3], 0, 255);
                    float g = Math.Clamp(originalColor.Green + errorMatrix[y][x * 3 + 1], 0, 255);
                    float b = Math.Clamp(originalColor.Blue + errorMatrix[y][x * 3 + 2], 0, 255);

                    SKColor quantizedColor = GetClosestColor(palette, new SKColor((byte)r, (byte)g, (byte)b));
                    ditheredBitmap.SetPixel(x, y, quantizedColor);

                    float rError = r - quantizedColor.Red;
                    float gError = g - quantizedColor.Green;
                    float bError = b - quantizedColor.Blue;

                    DistributeError(errorMatrix, x, y, rError, gError, bError);
                }
            }

            return ditheredBitmap;
        }

        private void DistributeError(float[][] errorMatrix, int x, int y, float rError, float gError, float bError)
        {
            int width = errorMatrix[0].Length / 3;
            int height = errorMatrix.Length;

            if (x + 1 < width)
            {
                errorMatrix[y][x * 3 + 3] += rError * 2 / 4;
                errorMatrix[y][x * 3 + 4] += gError * 2 / 4;
                errorMatrix[y][x * 3 + 5] += bError * 2 / 4;
            }

            if (y + 1 < height)
            {
                errorMatrix[y + 1][x * 3] += rError * 1 / 4;
                errorMatrix[y + 1][x * 3 + 1] += gError * 1 / 4;
                errorMatrix[y + 1][x * 3 + 2] += bError * 1 / 4;

                if (x + 1 < width)
                {
                    errorMatrix[y + 1][x * 3 + 3] += rError * 1 / 4;
                    errorMatrix[y + 1][x * 3 + 4] += gError * 1 / 4;
                    errorMatrix[y + 1][x * 3 + 5] += bError * 1 / 4;
                }
            }
        }
    }
}