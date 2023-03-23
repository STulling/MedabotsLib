using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace GBALib.ImageProcessing.Ditherers
{
    internal class RiemersmaDitherer : Ditherer
    {
        public enum CurveType
        {
            Hilbert,
            Z,
            Random
        }

        CurveType _curvetype;

        public RiemersmaDitherer(CurveType curveType)
        {
            this._curvetype = curveType;
        }

        public override SKBitmap Dither(SKBitmap bitmap, List<SKColor> palette)
        {
            switch (this._curvetype)
            {
                case CurveType.Hilbert:
                    return DitherWithCurve(bitmap, palette, GenerateHilbertCurve(bitmap.Width, bitmap.Height));
                case CurveType.Z:
                    return DitherWithCurve(bitmap, palette, GenerateZCurve(bitmap.Width, bitmap.Height));
                case CurveType.Random:
                    return DitherWithCurve(bitmap, palette, GenerateRandomCurve(bitmap.Width, bitmap.Height));
                default:
                    throw new ArgumentException("Invalid curve type");
            }
        }

        private IEnumerable<(int x, int y)> GenerateHilbertCurve(int width, int height)
        {
            int order = (int)Math.Ceiling(Math.Log(Math.Max(width, height), 2));
            int curveSize = 1 << order;

            for (int d = 0; d < curveSize * curveSize; d++)
            {
                int t = d;
                int x = 0;
                int y = 0;
                for (int s = 1; s < curveSize; s *= 2)
                {
                    int rx = 1 & (t / 2);
                    int ry = 1 & (t ^ rx);
                    (x, y) = Rotate(s, x, y, rx, ry);
                    x += s * rx;
                    y += s * ry;
                    t /= 4;
                }

                if (x < width && y < height)
                {
                    yield return (x, y);
                }
            }
        }

        private (int x, int y) Rotate(int n, int x, int y, int rx, int ry)
        {
            if (ry == 0)
            {
                if (rx == 1)
                {
                    x = n - 1 - x;
                    y = n - 1 - y;
                }

                return (y, x);
            }

            return (x, y);
        }

        private IEnumerable<(int x, int y)> GenerateZCurve(int width, int height)
        {
            int order = (int)Math.Ceiling(Math.Log(Math.Max(width, height), 2));
            int curveSize = 1 << order;
            var curvePoints = new List<(int x, int y)>();

            for (int d = 0; d < curveSize * curveSize; d++)
            {
                int t = d;
                int x = 0;
                int y = 0;
                for (int s = 1; s < curveSize; s *= 2)
                {
                    int rx = 1 & (t / 2);
                    int ry = 1 & (t ^ rx);
                    x += s * rx;
                    y += s * ry;
                    t /= 4;
                }

                if (x < width && y < height)
                {
                    curvePoints.Add((x, y));
                }
            }

            return curvePoints;
        }

        private IEnumerable<(int x, int y)> GenerateRandomCurve(int width, int height)
        {
            var curvePoints = new List<(int x, int y)>();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    curvePoints.Add((x, y));
                }
            }
            var random = new Random();
            int n = curvePoints.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (int x, int y) = curvePoints[k];
                curvePoints[k] = curvePoints[n];
                curvePoints[n] = (x, y);
                yield return (x, y);
            }
        }

        private SKBitmap DitherWithCurve(SKBitmap bitmap, List<SKColor> palette, IEnumerable<(int x, int y)> curvePoints, int n = 32, float R = 0.125f)
        {
            SKBitmap ditheredBitmap = new SKBitmap(bitmap.Width, bitmap.Height);
            var errorQueue = new Queue<(float rError, float gError, float bError)>(n);
            var diffusionSequence = new List<float>();

            // Calculate diffusion sequence
            float totalWeight = 0;
            for (int i = 0; i < n; i++)
            {
                float weight = (float)Math.Pow(R, i);
                diffusionSequence.Add(weight);
                totalWeight += weight;
            }

            foreach (var point in curvePoints)
            {
                int x = point.x;
                int y = point.y;

                SKColor originalColor = bitmap.GetPixel(x, y);

                // Apply previous errors from errorQueue
                float r = originalColor.Red;
                float g = originalColor.Green;
                float b = originalColor.Blue;
                int index = 0;
                foreach (var error in errorQueue.Reverse())
                {
                    float weight = diffusionSequence[index] / totalWeight;
                    r += error.rError * weight;
                    g += error.gError * weight;
                    b += error.bError * weight;
                    index++;
                }

                // Clamp color values
                r = Math.Clamp(r, 0, 255);
                g = Math.Clamp(g, 0, 255);
                b = Math.Clamp(b, 0, 255);

                SKColor quantizedColor = GetClosestColor(palette, new SKColor((byte)r, (byte)g, (byte)b));
                ditheredBitmap.SetPixel(x, y, quantizedColor);

                float rError = r - quantizedColor.Red;
                float gError = g - quantizedColor.Green;
                float bError = b - quantizedColor.Blue;

                errorQueue.Enqueue((rError, gError, bError));

                // Maintain errorQueue size
                if (errorQueue.Count > n)
                {
                    errorQueue.Dequeue();
                }
            }

            return ditheredBitmap;
        }

        public override string ToString()
        {
            return base.ToString() + "(" + this._curvetype.ToString() + ")";
        }
    }
}