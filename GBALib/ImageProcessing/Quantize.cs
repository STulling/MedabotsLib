using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace GBALib.ImageProcessing
{
    enum QuantizationMethod
    {
        Octree,
        MedianCut
    }
    internal class Quantize
    {
        public static void QuantizeImage(SKBitmap image, int colors, QuantizationMethod quantizationMethod)
        {
            Octree<Color> octree = new Octree<Color>(null, Color.Black);
            foreach (Color color in image.Palette)
            {
                octree.Add(color, color);
            }
            octree.Reduce(colors);
            image.Palette = octree.GetPalette();
            for (int i = 0; i < image.Data.Length; i++)
            {
                image.Data[i] = (byte)octree.GetIndex(image.Data[i]);
            }
        }
    }
}
