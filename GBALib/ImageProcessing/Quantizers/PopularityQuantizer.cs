using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBALib.ImageProcessing.Quantizers
{
    internal class PopularityQuantizer : Quantizer
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
            Dictionary<SKColor, int> colorFrequency = CalculateColorFrequency(colors);
            List<SKColor> quantizedColors = GetMostFrequentColors(colorFrequency, max_colors);

            return quantizedColors;
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

        private Dictionary<SKColor, int> CalculateColorFrequency(List<SKColor> colors)
        {
            Dictionary<SKColor, int> colorFrequency = new Dictionary<SKColor, int>();

            foreach (SKColor color in colors)
            {
                if (colorFrequency.ContainsKey(color))
                {
                    colorFrequency[color]++;
                }
                else
                {
                    colorFrequency[color] = 1;
                }
            }

            return colorFrequency;
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
