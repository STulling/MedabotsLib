using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class FloydSteinbergDitherer : Ditherer
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
                errorMatrix[y][x * 3 + 3] += rError * 7 / 16;
                errorMatrix[y][x * 3 + 4] += gError * 7 / 16;
                errorMatrix[y][x * 3 + 5] += bError * 7 / 16;
            }

            if (y + 1 < height)
            {
                if (x - 1 >= 0)
                {
                    errorMatrix[y + 1][x * 3 - 3] += rError * 3 / 16;
                    errorMatrix[y + 1][x * 3 - 2] += gError * 3 / 16;
                    errorMatrix[y + 1][x * 3 - 1] += bError * 3 / 16;
                }

                errorMatrix[y + 1][x * 3] += rError * 5 / 16;
                errorMatrix[y + 1][x * 3 + 1] += gError * 5 / 16;
                errorMatrix[y + 1][x * 3 + 2] += bError * 5 / 16;

                if (x + 1 < width)
                {
                    errorMatrix[y + 1][x * 3 + 3] += rError * 1 / 16;
                    errorMatrix[y + 1][x * 3 + 4] += gError * 1 / 16;
                    errorMatrix[y + 1][x * 3 + 5] += bError * 1 / 16;
                }
            }
        }
    }
}
