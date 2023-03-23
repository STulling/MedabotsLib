using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GBALib.ImageProcessing;
using MedabotsLib;
using SkiaSharp;

namespace Libtest
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load image from file using Skiasharp
            SKBitmap bitmap = SKBitmap.Decode("test.png");
            Dictionary<string, SKBitmap> results = Pixelizer.All_Combinations(bitmap, 16);
            // create out folder
            Directory.CreateDirectory("out");
            foreach (KeyValuePair<string, SKBitmap> result in results)
            {
                FileStream outfile = File.Create("out/" + result.Key + ".png");
                result.Value.Encode(SKEncodedImageFormat.Png, 100).SaveTo(outfile);
                outfile.Close();
            }
        }
    }
}
