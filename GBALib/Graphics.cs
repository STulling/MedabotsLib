using System;
using System.Collections.Generic;
using SkiaSharp;

/*
 * Joinked from: https://github.com/Normmatt/Medarot-Navi-GBA-Translation/blob/master/MedabotsMapEditor/MedabotsMapEditor/GBA/Graphics.cs
 * But we're unjoinking now
 */

namespace GBALib
{
    public enum GraphicsMode
    {
        Tile8bit,
        Tile4bit,
        BitmapTrueColour,
        Bitmap8bit
    }

    public static class GBAGraphics
    {
        static public SKBitmap ToBitmap(byte[] GBAGraphics, SKColor[] palette, int width, GraphicsMode mode, out int emptyGraphicBlocks)
        {
            SKBitmap result = null;
            emptyGraphicBlocks = 0;
            switch (mode)
            {
                case GraphicsMode.Tile8bit:
                    result = FromTile8bit(GBAGraphics, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.Tile4bit:
                    result = FromTile4bit(GBAGraphics, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.BitmapTrueColour:
                    result = FromBitmapTrueColour(GBAGraphics, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.Bitmap8bit:
                    result = FromBitmapIndexed(GBAGraphics, palette, width, out emptyGraphicBlocks);
                    break;
            }
            return result;
        }

        static private SKBitmap FromTile8bit(byte[] GBAGraphics, SKColor[] palette, int width, out int emptyGraphicBlocks)
        {
            int length = GBAGraphics.Length;
            int height = length / width;
            if (height % 8 != 0)
                height += 8 - (height % 8);

            while ((emptyGraphicBlocks = height * width - length) < 0)
                height += 8;
            emptyGraphicBlocks /= 64;

            SKBitmap result = new SKBitmap(width, height);

            for (int i = 0; i < length; i++)
            {
                byte paletteIndex = GBAGraphics[i];
                SKColor pixel = palette[paletteIndex];
                SKPoint tiledPosition = tiledCoordinate(i, width, 8);
                result.SetPixel((int)tiledPosition.X, (int)tiledPosition.Y, pixel);
            }

            return result;
        }

        static private SKBitmap FromTile4bit(byte[] GBAGraphics, SKColor[] palette, int width, out int emptyGraphicBlocks)
        {
            int length = GBAGraphics.Length;
            int height = length * 2 / width;

            if (height % 8 != 0 || height == 0)
                height += 8 - (height % 8);

            while ((emptyGraphicBlocks = height * width - length * 2) < 0)
                height += 8;
            emptyGraphicBlocks /= 64;
            SKBitmap result = new SKBitmap(width, height);

            for (int i = 0; i < length; i++)
            {
                byte pixel1 = (byte)(GBAGraphics[i] & 0xF);
                SKPoint position1 = tiledCoordinate(i * 2, width, 8);
                result.SetPixel((int)position1.X, (int)position1.Y, palette[pixel1]);

                byte pixel2 = (byte)(GBAGraphics[i] >> 4);
                SKPoint position2 = tiledCoordinate(i * 2 + 1, width, 8);
                result.SetPixel((int)position2.X, (int)position2.Y, palette[pixel2]);
            }
            return result;
        }

        static private SKColor RGB555toRGB888(short GBAColor, int index)
        {
            byte red = (byte)((GBAColor & 0x1F) << 3);
            byte green = (byte)(((GBAColor >> 5) & 0x1F) << 3);
            byte blue = (byte)(((GBAColor >> 10) & 0x1F) << 3);
            return new SKColor(red, green, blue);
        }

        static private SKBitmap FromBitmapTrueColour(byte[] GBAGraphics, SKColor[] palette, int width, out int emptyGraphicBlocks)
        {
            int length = GBAGraphics.Length;
            int height = (length / 2) / width;
            while (height * width < length / 2)
                height++;
            emptyGraphicBlocks = length / 2 - height * width;

            SKBitmap result = new SKBitmap(width, height);

            for (int i = 0; i < length; i+=2){
                short GBAColor = (short)(GBAGraphics[i] | (GBAGraphics[i + 1] << 8));
                SKColor pixel = RGB555toRGB888(GBAColor, i);
                SKPoint position = tiledCoordinate(i / 2, width, 1);
                result.SetPixel((int)position.X, (int)position.Y, pixel);
            }

            return result;
        }

        static private SKBitmap FromBitmapIndexed(byte[] GBAGraphics, SKColor[] palette, int width, out int emptyGraphicBlocks)
        {
            int length = GBAGraphics.Length;
            int height = length / width;
            while (height * width < length)
                height++;

            emptyGraphicBlocks = length - height * width;

            SKBitmap result = new SKBitmap(width, height);

            for (int i = 0; i < length; i++)
            {
                byte paletteIndex = GBAGraphics[i];
                SKColor pixel = palette[paletteIndex];
                SKPoint position = tiledCoordinate(i, width, 1);
                result.SetPixel((int)position.X, (int)position.Y, pixel);
            }

            emptyGraphicBlocks = 0;
            return result;
        }

        static public byte[] ToGBARaw(SKBitmap image, SKColor[] palette, GraphicsMode mode)
        {
            byte[] result = null;
            switch (mode)
            {
                case GraphicsMode.Tile8bit:
                    result = ToTile8bit(image, palette);
                    break;
                case GraphicsMode.Tile4bit:
                    result = ToTile4bit(image, palette);
                    break;
                case GraphicsMode.BitmapTrueColour:
                    result = ToBitmapTrueColour(image);
                    break;
                case GraphicsMode.Bitmap8bit:
                    result = ToBitmapIndexed(image, palette);
                    break;
            }
            return result;
        }

        static private int findPaletteIndex(SKColor color, SKColor[] palette)
        {
            int i = 0;
            while (i < palette.Length && palette[i] != color)
                i++;
            if (i == palette.Length)
                i = 0;
            return i;
        }

        static private byte[] ToTile8bit(SKBitmap image, SKColor[] palette)
        {
            byte[] result = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    SKColor color = image.GetPixel(x, y);
                    int i = findPaletteIndex(color, palette);
                    int position = tiledPosition(new SKPoint(x, y), image.Width, 8);
                    result[position] = (byte)i;
                }
            }
            return result;
        }

        static private byte[] ToTile4bit(SKBitmap image, SKColor[] palette)
        {
            byte[] result = new byte[image.Width * image.Height / 2];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    SKColor color = image.GetPixel(x, y);
                    int i = findPaletteIndex(color, palette);

                    int position = tiledPosition(new SKPoint(x, y), image.Width, 8);

                    if ((position & 1) == 1)
                        i <<= 4;

                    result[position / 2] |= (byte)i;
                }
            }
            return result;
        }

        static private byte[] ToBitmapTrueColour(SKBitmap image)
        {
            byte[] result = new byte[image.Width * image.Height * 2];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    SKColor color = image.GetPixel(x, y);

                    ushort colorValue = toGBAcolor(color);

                    int position = BitmapPosition(new SKPoint(x, y), image.Width);
                    result[position * 2] = (byte)(colorValue & 0xFF);
                    result[position * 2 + 1] = (byte)((colorValue >> 8) & 0xFF);
                }
            }
            return result;
        }

        static private byte[] ToBitmapIndexed(SKBitmap image, SKColor[] palette)
        {
            byte[] result = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    SKColor color = image.GetPixel(x, y);
                    int i = findPaletteIndex(color, palette);

                    result[y * image.Width + x] = (byte)i;
                }
            }
            return result;
        }

        static public int RawGraphicsLength(SKSize size, GraphicsMode mode)
        {
            return (int)(size.Width * size.Height * BitsPerPixel(mode) / 8);
        }

        static public int BitsPerPixel(GraphicsMode mode)
        {
            switch (mode)
            {
                case GraphicsMode.Tile8bit:
                    return 8;
                case GraphicsMode.Tile4bit:
                    return 4;
                case GraphicsMode.BitmapTrueColour:
                    return 16;
                case GraphicsMode.Bitmap8bit:
                    return 8;
                default:
                    return 0;
            }
        }

        static private SKColor[] toPalette(ushort[] GBAPalette, int amountOfColours)
        {
            SKColor[] palette = new SKColor[amountOfColours];

            for (int i = 0; i < palette.Length; i++)
            {
                palette[i] = toColor(GBAPalette[i]);
            }
            return palette;
        }

        static public byte[] toRawGBAPalette(SKColor[] palette)
        {
            byte[] result = new byte[palette.Length * 2];
            for (int i = 0; i < palette.Length; i++)
            {
                ushort color = toGBAcolor(palette[i]);
                result[i * 2] = (byte)(color & 0xFF);
                result[i * 2 + 1] = (byte)((color >> 8) & 0xFF);
            }
            return result;
        }

        static private SKColor toColor(ushort GBAColor)
        {
            int red = ((GBAColor) & 0x1F) * 8;
            int green = (((GBAColor) >> 5) & 0x1F) * 8;
            int blue = (((GBAColor) >> 10) & 0x1F) * 8;
            return new SKColor((byte)red, (byte)green, (byte)blue);
        }

        /// <summary>
        /// Converts a SKColor to a GBA color (RGB555)
        /// </summary>
        /// <param name="color">The SKColor to convert</param>
        static public ushort toGBAcolor(SKColor color)
        {
            byte red = (byte)(color.Red >> 3);
            byte green = (byte)(color.Green >> 3);
            byte blue = (byte)(color.Blue >> 3);
            return (ushort)(red + (green << 5) + (blue << 10));
        }

        static public ushort toGBAcolor(int red, int green, int blue)
        {
            byte GBAred = (byte)(red >> 3);
            byte GBAblue = (byte)(green >> 3);
            byte GBAgreen = (byte)(blue >> 3);
            return (ushort)(GBAred + (GBAgreen << 5) + (GBAblue << 10));
        }

        static private SKPoint BitmapCoordinate(int position, int width)
        {
            SKPoint point = new SKPoint();
            point.X = position / width;
            point.Y = position % width;
            return point;
        }

        static private SKPoint tiledCoordinate(int position, int width, int tileDimension)
        {
            if (width % tileDimension != 0)
                throw new ArgumentException("Bitmaps width needs to be multiple of tile's width.");

            SKPoint point = new SKPoint();
            point.X = (position % tileDimension) + ((position / (tileDimension * tileDimension)) % (width / tileDimension)) * tileDimension;
            point.Y = ((position % (tileDimension * tileDimension)) / tileDimension) + ((position / (tileDimension * tileDimension)) * tileDimension / width) * tileDimension;
            return point;
        }

        static private int BitmapPosition(SKPoint coordinate, int width)
        {
            return (int)(coordinate.X + coordinate.Y * width);
        }

        static private int tiledPosition(SKPoint coordinate, int width, int tileDimension)
        {
            if (width % tileDimension != 0)
                throw new ArgumentException("Bitmaps width needs to be multiple of tile's width.");

            return (int)((coordinate.X % tileDimension + (coordinate.Y % tileDimension) * tileDimension +
                   (coordinate.X / tileDimension) * (tileDimension * tileDimension) +
                   (coordinate.Y / tileDimension) * (tileDimension * width)));
        }

        static public (byte[], byte[]) ConvertToGBA(SKBitmap image, GraphicsMode mode)
        {
            if (mode == GraphicsMode.BitmapTrueColour) {
                // Convert colors to 16 bit
            }

            (SKBitmap, SKColor[]) quantized = Quantize(image);
            SKBitmap quantizedImage = quantized.Item1;
            SKColor[] palette = quantized.Item2;

            byte[] imageData = ToGBARaw(quantizedImage, palette, mode);
            byte[] paletteData = toRawGBAPalette(palette);

            return (imageData, paletteData);
        }

        static private (SKBitmap, SKColor[]) Quantize(SKBitmap bitmap, int colorlimit)
        {
            SKBitmap result = new SKBitmap(bitmap.Width, bitmap.Height);

            Octree<List<SKColor>> colors = new Octree<List<SKColor>>(5, 5);
            int* pointer = (int*)bmpData.Scan0.ToPointer();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < trueColorBitmap.Width; x++)
                {
                    Color color = Color.FromArgb(pointer[x]);
                    int[] position = new int[5];
                    for (int i = 0; i < position.Length; i++)
                    {
                        position[i] = ((color.R >> (8 - i)) & 1);
                        position[i] += ((color.G >> (8 - i)) & 1) * 2;
                        position[i] += ((color.B >> (8 - i)) & 1) * 4;
                    }
                    colors.GetItem(position).Add(color);
                }
                pointer += bmpData.Stride;
            }

            if (trueColorBitmap != bitmap)
                trueColorBitmap.Dispose();

            return result;
        }
    }
}