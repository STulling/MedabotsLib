using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBALib.ImageProcessing.Quantizers
{
    internal class MinimumVarianceQuantizer : Quantizer
    {
        public override List<SKColor> Quantize(SKBitmap bitmap, int max_colors)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (max_colors < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(max_colors));
            }

            List<SKColor> colors = GetColors(bitmap);
            List<SKColor> palette = MinimumVariance(colors, max_colors);

            return palette;
        }

        private List<SKColor> GetColors(SKBitmap bitmap)
        {
            List<SKColor> colors = new List<SKColor>();

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    colors.Add(bitmap.GetPixel(x, y));
                }
            }

            return colors;
        }

        private List<SKColor> MinimumVariance(List<SKColor> colors, int max_colors)
        {
            List<SKColor> palette = new List<SKColor>();

            if (colors.Count <= max_colors)
            {
                palette.AddRange(colors);
            }
            else
            {
                List<SKColor> remainingColors = new List<SKColor>(colors);
                while (palette.Count < max_colors)
                {
                    SKColor selectedColor = GetColorWithMinimumVariance(remainingColors);
                    palette.Add(selectedColor);
                    remainingColors.Remove(selectedColor);
                }
            }

            return palette;
        }

        private SKColor GetColorWithMinimumVariance(List<SKColor> colors)
        {
            Dictionary<SKColor, double> colorVariances = new Dictionary<SKColor, double>();

            foreach (SKColor color in colors)
            {
                double variance = CalculateColorVariance(color, colors);
                colorVariances[color] = variance;
            }

            return colorVariances.OrderBy(kvp => kvp.Value).First().Key;
        }

        private double CalculateColorVariance(SKColor color, List<SKColor> colors)
        {
            double sum = 0;
            int count = colors.Count;

            foreach (SKColor otherColor in colors)
            {
                sum += ColorDistanceSquared(color, otherColor);
            }

            return sum / count;
        }

        private double ColorDistanceSquared(SKColor a, SKColor b)
        {
            int deltaR = a.Red - b.Red;
            int deltaG = a.Green - b.Green;
            int deltaB = a.Blue - b.Blue;

            return deltaR * deltaR + deltaG * deltaG + deltaB * deltaB;
        }
    }
}
