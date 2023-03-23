using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GBALib.ImageProcessing
{
    internal enum ColorSpace
    {
        sRGB,
        CIELAB,
        CIELCH,
        HSV
    }
    internal class ColorHelper
    {
        private static ColorHelper instance;
        public ColorSpace Space { get; set; } = ColorSpace.sRGB;

        public static ColorHelper Instance{
            get
            {
                if (instance == null)
                {
                    instance = new ColorHelper();
                }
                return instance;
            }
        }
        private ColorHelper()
        {
        }

        public float ColorDistance(SKColor c1, SKColor c2)
        {
            return Space switch
            {
                ColorSpace.sRGB => Distance_sRGB(c1, c2),
                ColorSpace.CIELAB => Distance_CIELAB(c1, c2),
                ColorSpace.CIELCH => Distance_CIELCH(c1, c2),
                ColorSpace.HSV => Distance_HSV(c1, c2),
                _ => throw new ArgumentException("Invalid color space"),
            };
        }

        public static float Distance_sRGB(SKColor c1, SKColor c2)
        {
            float distance = (c1.Red - c2.Red) * (c1.Red - c2.Red)
                + (c1.Green - c2.Green) * (c1.Green - c2.Green)
                + (c1.Blue - c2.Blue) * (c1.Blue - c2.Blue);
            return ((float)Math.Sqrt(distance));
        }

        public float Distance_CIELAB(SKColor c1, SKColor c2)
        {
            return DistanceInColorSpace(c1, c2, RGBtoCIELAB);
        }

        public float Distance_CIELCH(SKColor c1, SKColor c2)
        {
            return DistanceInColorSpace(c1, c2, RGBtoCIELCH);
        }

        public float Distance_HSV(SKColor c1, SKColor c2)
        {
            return DistanceInColorSpace(c1, c2, RGBtoHSV);
        }

        private static float DistanceInColorSpace(SKColor c1, SKColor c2, Func<SKColor, Tuple<float, float, float>> converter)
        {
            var cs1 = converter(c1);
            var cs2 = converter(c2);
            float distance = (cs1.Item1 - cs2.Item1) * (cs1.Item1 - cs2.Item1)
                + (cs1.Item2 - cs2.Item2) * (cs1.Item2 - cs2.Item2)
                + (cs1.Item3 - cs2.Item3) * (cs1.Item3 - cs2.Item3);
            return ((float)Math.Sqrt(distance));
        }

        private Tuple<float, float, float> RGBtoCIELAB(SKColor color)
        {
            // RGB to XYZ
            float r = color.Red / 255.0f;
            float g = color.Green / 255.0f;
            float b = color.Blue / 255.0f;

            r = (r > 0.04045f) ? (float)Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92f;
            g = (g > 0.04045f) ? (float)Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92f;
            b = (b > 0.04045f) ? (float)Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92f;

            r *= 100;
            g *= 100;
            b *= 100;

            float x = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            float y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            float z = r * 0.0193f + g * 0.1192f + b * 0.9505f;

            // XYZ to LAB
            float xr = x / 95.047f;
            float yr = y / 100.0f;
            float zr = z / 108.883f;

            xr = (xr > 0.008856f) ? (float)Math.Pow(xr, 1.0 / 3.0) : (7.787f * xr) + 16.0f / 116.0f;
            yr = (yr > 0.008856f) ? (float)Math.Pow(yr, 1.0 / 3.0) : (7.787f * yr) + 16.0f / 116.0f;
            zr = (zr > 0.008856f) ? (float)Math.Pow(zr, 1.0 / 3.0) : (7.787f * zr) + 16.0f / 116.0f;

            float l = 116.0f * yr - 16;
            float a = 500.0f * (xr - yr);
            float bz = 200.0f * (yr - zr);

            return Tuple.Create(l, a, bz);
        }

        private Tuple<float, float, float> RGBtoCIELCH(SKColor color)
        {
            var lab = RGBtoCIELAB(color);
            float l = lab.Item1;
            float a = lab.Item2;
            float b = lab.Item3;

            float c = (float)Math.Sqrt(a * a + b * b);
            float h = (float)Math.Atan2(b, a);

            h = (float)(h * 180.0 / Math.PI);
            if (h < 0)
            {
                h += 360;
            }

            return Tuple.Create(l, c, h);
        }

        private Tuple<float, float, float> RGBtoHSV(SKColor color)
        {
            float r = color.Red / 255.0f;
            float g = color.Green / 255.0f;
            float b = color.Blue / 255.0f;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            float h;
            if (delta == 0) h = 0;
            else if (max == r) h = (g - b) / delta % 6;
            else if (max == g) h = (b - r) / delta + 2;
            else h = (r - g) / delta + 4;
            h = (float)(h * 60.0);
            if (h < 0) h += 360;

            float s = (max == 0) ? 0 : delta / max;
            float v = max;

            return Tuple.Create(h, s, v);
        }

    }
}
