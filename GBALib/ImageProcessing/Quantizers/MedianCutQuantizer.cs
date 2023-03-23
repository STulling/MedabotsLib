using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

namespace GBALib.ImageProcessing.Quantizers
{
    internal class MedianCutQuantizer: Quantizer
    {
        public override List<SKColor> Quantize(SKBitmap bitmap, int colors)
        {
            if (colors < 1 || colors > 256)
                throw new ArgumentOutOfRangeException("colors", "Number of colors should be between 1 and 256");

            return GeneratePalette(bitmap, colors);
        }

        private static List<SKColor> GeneratePalette(SKBitmap bitmap, int colors)
        {
            List<SKColor> colorList = new List<SKColor>(bitmap.Width * bitmap.Height);

            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    colorList.Add(bitmap.GetPixel(x, y));
                }
            }

            List<Box> boxList = new List<Box> { new Box(colorList) };

            while (boxList.Count < colors)
            {
                Box largestBox = boxList.OrderByDescending(b => b.Colors.Count).FirstOrDefault();
                if (largestBox == null || largestBox.Colors.Count <= 1)
                    break;

                boxList.Remove(largestBox);
                boxList.AddRange(largestBox.Split());
            }

            return boxList.Select(b => b.AverageColor()).ToList();
        }

        private class Box
        {
            public List<SKColor> Colors { get; }

            public Box(List<SKColor> colors)
            {
                Colors = colors;
            }

            public IEnumerable<Box> Split()
            {
                int rRange = Colors.Max(c => c.Red) - Colors.Min(c => c.Red);
                int gRange = Colors.Max(c => c.Green) - Colors.Min(c => c.Green);
                int bRange = Colors.Max(c => c.Blue) - Colors.Min(c => c.Blue);

                int maxRange = Math.Max(rRange, Math.Max(gRange, bRange));

                List<SKColor> sortedColors;

                if (maxRange == rRange)
                    sortedColors = Colors.OrderBy(c => c.Red).ToList();
                else if (maxRange == gRange)
                    sortedColors = Colors.OrderBy(c => c.Green).ToList();
                else
                    sortedColors = Colors.OrderBy(c => c.Blue).ToList();

                int median = sortedColors.Count / 2;

                return new[]
                {
                new Box(sortedColors.Take(median).ToList()),
                new Box(sortedColors.Skip(median).ToList())
            };
            }

            public SKColor AverageColor()
            {
                int totalR = 0;
                int totalG = 0;
                int totalB = 0;

                foreach (SKColor color in Colors)
                {
                    totalR += color.Red;
                    totalG += color.Green;
                    totalB += color.Blue;
                }

                int count = Colors.Count;
                return new SKColor((byte)(totalR / count), (byte)(totalG / count), (byte)(totalB / count));
            }
        }
    }
}
