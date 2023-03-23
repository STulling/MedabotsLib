using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace GBALib.ImageProcessing.Quantizers
{
    internal class UniformQuantizer : Quantizer
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

            int bits = GetBitsPerChannel(max_colors);
            List<SKColor> colors = GetColors(bitmap);
            Dictionary<SKColor, int> colorFrequency = CalculateColorFrequency(colors, bits);
            List<SKColor> quantizedColors = GetMostFrequentColors(colorFrequency, max_colors);

            return quantizedColors;
        }

        private int GetBitsPerChannel(int max_colors)
        {
            int bits = (int)Math.Ceiling(Math.Pow(max_colors, 1.0 / 3.0));
            return Math.Min(bits, 8); // Limit to 8 bits per channel
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

        private Dictionary<SKColor, int> CalculateColorFrequency(List<SKColor> colors, int bits)
        {
            Dictionary<SKColor, int> colorFrequency = new Dictionary<SKColor, int>();
            int step = 256 / bits;

            foreach (SKColor color in colors)
            {
                SKColor quantizedColor = QuantizeColor(color, step);
                if (colorFrequency.ContainsKey(quantizedColor))
                {
                    colorFrequency[quantizedColor]++;
                }
                else
                {
                    colorFrequency[quantizedColor] = 1;
                }
            }

            return colorFrequency;
        }

        private SKColor QuantizeColor(SKColor color, int step)
        {
            byte r = (byte)((color.Red / step) * step);
            byte g = (byte)((color.Green / step) * step);
            byte b = (byte)((color.Blue / step) * step);

            return new SKColor(r, g, b);
        }

        private List<SKColor> GetMostFrequentColors(Dictionary<SKColor, int> colorFrequency, int max_colors)
        {
            return colorFrequency.OrderByDescending(pair => pair.Value)
                                 .Take(max_colors)
                                 .Select(pair => pair.Key)
                                 .ToList();
        }
    }

}
