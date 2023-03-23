using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class StuckiDitherer : Ditherer
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

            int[][] stuckiMatrix = new int[][]
            {
                new int[] {0, 0, 0, 8, 4},
                new int[] {2, 4, 8, 4, 2},
                new int[] {1, 2, 4, 2, 1}
            };

            int stuckiDivisor = 42;

            for (int row = 0; row < stuckiMatrix.Length; row++)
            {
                for (int col = 0; col < stuckiMatrix[row].Length; col++)
                {
                    int xOffset = col - 2;
                    int yOffset = row - 1;

                    if (x + xOffset >= 0 && x + xOffset < width && y + yOffset >= 0 && y + yOffset < height)
                    {
                        errorMatrix[y + yOffset][3 * (x + xOffset)] += rError * stuckiMatrix[row][col] / stuckiDivisor;
                        errorMatrix[y + yOffset][3 * (x + xOffset) + 1] += gError * stuckiMatrix[row][col] / stuckiDivisor;
                        errorMatrix[y + yOffset][3 * (x + xOffset) + 2] += bError * stuckiMatrix[row][col] / stuckiDivisor;
                    }
                }
            }
        }
    }
}