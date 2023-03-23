using CustomLib.ImageProcessing.Ditherers;
using GBALib.ImageProcessing.Ditherers;
using GBALib.ImageProcessing.Quantizers;
using SkiaSharp;
using System;
using System.Collections.Generic;

namespace GBALib.ImageProcessing
{
    public static class Pixelizer
    {
        static List<Quantizer> quantizers = new List<Quantizer> 
        { 
            new MedianCutQuantizer(),
            new KMeansQuantizer(),
            new UniformQuantizer(),
            new PopularityQuantizer(),
            //new MinimumVarianceQuantizer(),
        };

        static List<Ditherer> ditherers = new List<Ditherer>
        {
            new NoneDitherer(),
            new FloydSteinbergDitherer(),
            new BayerDitherer(2),
            new BayerDitherer(4),
            new BayerDitherer(8),
            new RiemersmaDitherer(RiemersmaDitherer.CurveType.Hilbert),
            new RiemersmaDitherer(RiemersmaDitherer.CurveType.Z),
            new RiemersmaDitherer(RiemersmaDitherer.CurveType.Random),
            new JarvisJudiceNinkeDitherer(),
            new StuckiDitherer(),
            new AtkinsonDitherer(),
            new Sierra2Ditherer(),
            new Sierra3Ditherer(),
            new SierraLiteDitherer(),
        };

        static List<ColorSpace> colorspace = new List<ColorSpace>
        { ColorSpace.sRGB, ColorSpace.CIELAB, ColorSpace.CIELCH};

        public static Dictionary<string, SKBitmap> All_Combinations(SKBitmap bitmap, int colors)
        {
            Dictionary<string, SKBitmap> results = new Dictionary<string, SKBitmap>();
            foreach (ColorSpace colorSpace in colorspace)
            {
                ColorHelper.Instance.Space = colorSpace;
                foreach (Quantizer quantizer in quantizers)
                {
                    Console.WriteLine(quantizer.ToString());
                    List<SKColor> palette = quantizer.Quantize(bitmap, colors);
                    Console.WriteLine(palette.Count);
                    foreach (Ditherer ditherer in ditherers)
                    {
                        string name = colorSpace.ToString() + " + " + quantizer.GetType().Name + " + " + ditherer.ToString();
                        Console.WriteLine(ditherer.ToString());
                        results.Add(name, ditherer.Dither(bitmap, palette));
                    }
                }
            }
            return results;
        }
    }
}
