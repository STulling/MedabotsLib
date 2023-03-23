using GBALib.ImageProcessing;
using GBALib.ImageProcessing.Ditherers;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomLib.ImageProcessing.Ditherers
{
    internal class BayerDitherer : Ditherer
    {
        private readonly int _matrixSize;

        public BayerDitherer(int matrixSize)
        {
            _matrixSize = matrixSize;
        }

        public override string ToString()
        {
            return base.ToString() + " (" + _matrixSize + ")";
        }

        public override SKBitmap Dither(SKBitmap originalBitmap, List<SKColor> palette)
        {
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            SKBitmap bitmap = originalBitmap.Copy();
            SKBitmap ditheredBitmap = new SKBitmap(width, height);

            float[,] bayerMatrix = GenerateBayerMatrix(_matrixSize);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Get closest 2 colors:
                    SKColor originalColor = bitmap.GetPixel(x, y);
                    var closestColors = GetClosestColorsWithDistances(palette, originalColor, 2);
                    SKColor[] closestColor = closestColors.Item1;
                    float[] distancesToClosestColors = closestColors.Item2;

                    float error = distancesToClosestColors[0] / distancesToClosestColors[1];

                    // get threshold:
                    float threshold = bayerMatrix[y % _matrixSize, x % _matrixSize] / (_matrixSize * _matrixSize);

                    // Set quantized color:
                    ditheredBitmap.SetPixel(x, y, error < threshold ? closestColor[0] : closestColor[1]);
                }
            }

            return ditheredBitmap;
        }

        private Tuple<SKColor[], float[]> GetClosestColorsWithDistances(List<SKColor> palette, SKColor origin, int count)
        {
            List<Tuple<SKColor, float>> distances = new List<Tuple<SKColor, float>>();
            foreach (SKColor color in palette)
            {
                float distance = ColorHelper.Instance.ColorDistance(origin, color);
                distances.Add(new Tuple<SKColor, float>(color, distance));
            }
            distances = distances.OrderBy(x => x.Item2).ToList();
            SKColor[] closestColors = new SKColor[count];
            float[] distancesToClosestColors = new float[count];
            for (int i = 0; i < count; i++)
            {
                closestColors[i] = distances[i].Item1;
                distancesToClosestColors[i] = distances[i].Item2;
            }
            return new Tuple<SKColor[], float[]>(closestColors, distancesToClosestColors);
        }

        private float[,] Bayer2x2 = new float[2, 2]
        {
            { 0.0f, 2.0f },
            { 3.0f, 1.0f }
        };

        private float[,] Bayer4x4 = new float[4, 4]
        {
            { 0.0f,  8.0f,  2.0f, 10.0f },
            { 12.0f, 4.0f, 14.0f,  6.0f },
            { 3.0f, 11.0f,  1.0f,  9.0f },
            { 15.0f, 7.0f, 13.0f,  5.0f }
        };

        private float[,] Bayer8x8 = new float[8, 8]
        {
            {  0.0f, 32.0f,  8.0f, 40.0f,  2.0f, 34.0f, 10.0f, 42.0f },
            { 48.0f, 16.0f, 56.0f, 24.0f, 50.0f, 18.0f, 58.0f, 26.0f },
            { 12.0f, 44.0f,  4.0f, 36.0f, 14.0f, 46.0f,  6.0f, 38.0f },
            { 60.0f, 28.0f, 52.0f, 20.0f, 62.0f, 30.0f, 54.0f, 22.0f },
            {  3.0f, 35.0f, 11.0f, 43.0f,  1.0f, 33.0f,  9.0f, 41.0f },
            { 51.0f, 19.0f, 59.0f, 27.0f, 49.0f, 17.0f, 57.0f, 25.0f },
            { 15.0f, 47.0f,  7.0f, 39.0f, 13.0f, 45.0f,  5.0f, 37.0f },
            { 63.0f, 31.0f, 55.0f, 23.0f, 61.0f, 29.0f, 53.0f, 21.0f }
        };

        private float[,] GenerateBayerMatrix(int size)
        {
            switch (size)
            {
                case 2:
                    return Bayer2x2;
                case 4:
                    return Bayer4x4;
                case 8:
                    return Bayer8x8;
                default:
                    throw new Exception("Invalid matrix size");
            }
        }
    }
}