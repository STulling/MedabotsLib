using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class JarvisJudiceNinkeDitherer : Ditherer
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

            int[] dx = { 1, -2, -1, 0, 1, 2, -2, -1, 0, 1, 2 };
            int[] dy = { 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2 };
            int[] weights = { 7, 3, 5, 7, 5, 3, 1, 3, 5, 3, 1 };

            int totalWeight = weights.Sum();

            for (int i = 0; i < dx.Length; i++)
            {
                int newX = x + dx[i];
                int newY = y + dy[i];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    errorMatrix[newY][newX * 3] += rError * weights[i] / totalWeight;
                    errorMatrix[newY][newX * 3 + 1] += gError * weights[i] / totalWeight;
                    errorMatrix[newY][newX * 3 + 2] += bError * weights[i] / totalWeight;
                }
            }
        }
    }
}