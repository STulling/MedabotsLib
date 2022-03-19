using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

/*
 * Joinked from: https://github.com/Normmatt/Medarot-Navi-GBA-Translation/blob/master/MedabotsMapEditor/MedabotsMapEditor/GBA/Graphics.cs
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

    public unsafe static class GBAGraphics
    {
        static public Bitmap ToBitmap(byte[] GBAGraphics, int length, int index, Color[] palette, int width, GraphicsMode mode, out int emptyGraphicBlocks)
        {
            fixed (byte* pointer = &GBAGraphics[index])
            {
                return ToBitmap(pointer, length, palette, width, mode, out emptyGraphicBlocks);
            }
        }

        static public Bitmap ToBitmap(byte* GBAGraphics, int length, Color[] palette, int width, GraphicsMode mode, out int emptyGraphicBlocks)
        {
            Bitmap result = null;
            emptyGraphicBlocks = 0;
            switch (mode)
            {
                case GraphicsMode.Tile8bit:
                    result = FromTile8bit(GBAGraphics, length, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.Tile4bit:
                    result = FromTile4bit(GBAGraphics, length, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.BitmapTrueColour:
                    result = FromBitmapTrueColour(GBAGraphics, length, palette, width, out emptyGraphicBlocks);
                    break;
                case GraphicsMode.Bitmap8bit:
                    result = FromBitmapIndexed(GBAGraphics, length, palette, width, out emptyGraphicBlocks);
                    break;
            }
            return result;
        }

        static private Bitmap FromTile8bit(byte* GBAGraphics, int length, Color[] palette, int width, out int emptyGraphicBlocks)
        {
            int height = length / width;
            if (height % 8 != 0)
                height += 8 - (height % 8);

            while ((emptyGraphicBlocks = height * width - length) < 0)
                height += 8;
            emptyGraphicBlocks /= 64;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            result.Palette = paletteMaker(palette, result.Palette);

            BitmapData bData = result.LockBits(new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly, result.PixelFormat);

            for (int i = 0; i < length; i++)
            {
                byte pixel = GBAGraphics[i];
                int position = bitmapPosition(tiledCoordinate(i, width, 8), width);
                ((byte*)bData.Scan0)[position] = pixel;
            }

            result.UnlockBits(bData);
            return result;
        }

        static private Bitmap FromTile4bit(byte* GBAGraphics, int length, Color[] palette, int width, out int emptyGraphicBlocks)
        {
            int height = length * 2 / width;

            if (height % 8 != 0 || height == 0)
                height += 8 - (height % 8);

            while ((emptyGraphicBlocks = height * width - length * 2) < 0)
                height += 8;
            emptyGraphicBlocks /= 64;
            Bitmap result = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            result.Palette = paletteMaker(palette, result.Palette);

            BitmapData bData = result.LockBits(new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly, result.PixelFormat);

            for (int i = 0; i < length; i++)
            {
                byte pixel1 = (byte)(GBAGraphics[i] & 0xF);
                byte pixel2 = (byte)(GBAGraphics[i] >> 4);
                int position = bitmapPosition(tiledCoordinate(i * 2, width, 8), width);
                ((byte*)bData.Scan0)[position] = pixel1;
                ((byte*)bData.Scan0)[position + 1] = pixel2;
            }

            result.UnlockBits(bData);

            return result;
        }

        static private Bitmap FromBitmapTrueColour(byte* GBAGraphics, int length, Color[] palette, int width, out int emptyGraphicBlocks)
        {
            int height = (length / 2) / width;
            while (height * width < length / 2)
                height++;
            emptyGraphicBlocks = length / 2 - height * width;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format16bppRgb555);

            BitmapData bData = result.LockBits(new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly, result.PixelFormat);

            byte* bitmap = (byte*)bData.Scan0;
            for (int i = 0; i < length; i++)
                bitmap[i] = GBAGraphics[i];

            result.UnlockBits(bData);
            return result;
        }

        static private Bitmap FromBitmapIndexed(byte* GBAGraphics, int length, Color[] palette, int width, out int emptyGraphicBlocks)
        {
            int height = length / width;
            while (height * width < length)
                height++;

            emptyGraphicBlocks = length - height * width;

            Bitmap result = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            result.Palette = paletteMaker(palette, result.Palette);

            BitmapData bData = result.LockBits(new Rectangle(new Point(), result.Size), ImageLockMode.WriteOnly, result.PixelFormat);

            byte* bitmap = (byte*)bData.Scan0;
            for (int i = 0; i < length; i++)
                bitmap[i] = GBAGraphics[i];

            result.UnlockBits(bData);
            emptyGraphicBlocks = 0;
            return result;
        }

        static public byte[] ToGBARaw(Bitmap image, Color[] palette, GraphicsMode mode)
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

        static private byte[] ToTile8bit(Bitmap image, Color[] palette)
        {
            byte[] result = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int i = 0;
                    while (i < palette.Length && palette[i] != color)
                        i++;
                    if (i == palette.Length)
                        i = 0;

                    int position = tiledPosition(new Point(x, y), image.Width, 8);
                    result[position] = (byte)i;
                }
            }
            return result;
        }

        static private byte[] ToTile4bit(Bitmap image, Color[] palette)
        {
            byte[] result = new byte[image.Width * image.Height / 2];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int i = 0;
                    while (i < palette.Length && palette[i] != color)
                        i++;
                    if (i == palette.Length)
                        i = 0;

                    int position = tiledPosition(new Point(x, y), image.Width, 8);

                    if ((position & 1) == 1)
                        i <<= 4;

                    result[position / 2] |= (byte)i;
                }
            }
            return result;
        }

        static private byte[] ToBitmapTrueColour(Bitmap image)
        {
            byte[] result = new byte[image.Width * image.Height * 2];

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);

                    ushort colorValue = toGBAcolor(color);

                    int position = bitmapPosition(new Point(x, y), image.Width);
                    result[position * 2] = (byte)(colorValue & 0xFF);
                    result[position * 2 + 1] = (byte)((colorValue >> 8) & 0xFF);
                }
            }
            return result;
        }

        static private byte[] ToBitmapIndexed(Bitmap image, Color[] palette)
        {
            byte[] result = new byte[image.Width * image.Height];
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color color = image.GetPixel(x, y);
                    int i = 0;
                    while (i < palette.Length && palette[i] != color)
                        i++;
                    if (i == palette.Length)
                        i = 0;

                    result[y * image.Width + x] = (byte)i;
                }
            }
            return result;
        }

        static private ColorPalette paletteMaker(Color[] palette, ColorPalette original)
        {
            if (palette == null)
                return original;

            for (int i = 0; i < palette.Length && i < original.Entries.Length; i++)
            {
                original.Entries[i] = palette[i];
            }
            for (int i = palette.Length; i < original.Entries.Length; i++)
            {
                original.Entries[i] = Color.FromArgb(0, 0, 0);
            }
            return original;
        }

        static public int RawGraphicsLength(Size size, GraphicsMode mode)
        {
            return size.Width * size.Height * BitsPerPixel(mode) / 8;
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

        static public Color[] toPalette(byte[] data, int offset, int amountOfColours)
        {
            fixed (byte* ptr = &data[offset])
            {
                return toPalette((ushort*)ptr, amountOfColours);
            }
        }

        static private Color[] toPalette(ushort* GBAPalette, int amountOfColours)
        {
            Color[] palette = new Color[amountOfColours];

            for (int i = 0; i < palette.Length; i++)
            {
                palette[i] = toColor(GBAPalette);
                GBAPalette++;
            }
            return palette;
        }

        static public byte[] toRawGBAPalette(Color[] palette)
        {
            byte[] result = new byte[palette.Length * 2];
            fixed (byte* pointer = &result[0])
            {
                ushort* upointer = (ushort*)pointer;
                for (int i = 0; i < palette.Length; i++)
                {
                    *upointer = toGBAcolor(palette[i]);
                    upointer++;
                }
            }
            return result;
        }

        static private Color toColor(ushort* GBAColor)
        {
            int red = ((*GBAColor) & 0x1F) * 8;
            int green = (((*GBAColor) >> 5) & 0x1F) * 8;
            int blue = (((*GBAColor) >> 10) & 0x1F) * 8;
            return Color.FromArgb(red, green, blue);
        }

        static public ushort toGBAcolor(Color color)
        {
            byte red = (byte)(color.R >> 3);
            byte blue = (byte)(color.B >> 3);
            byte green = (byte)(color.G >> 3);
            return (ushort)(red + (green << 5) + (blue << 10));
        }

        static public ushort toGBAcolor(int red, int green, int blue)
        {
            byte GBAred = (byte)(red >> 3);
            byte GBAblue = (byte)(green >> 3);
            byte GBAgreen = (byte)(blue >> 3);
            return (ushort)(GBAred + (GBAgreen << 5) + (GBAblue << 10));
        }

        static private Point bitmapCoordinate(int position, int width)
        {
            Point point = new Point();
            point.X = position / width;
            point.Y = position % width;
            return point;
        }

        static private Point tiledCoordinate(int position, int width, int tileDimension)
        {
            if (width % tileDimension != 0)
                throw new ArgumentException("Bitmaps width needs to be multiple of tile's width.");

            Point point = new Point();
            point.X = (position % tileDimension) + ((position / (tileDimension * tileDimension)) % (width / tileDimension)) * tileDimension;
            point.Y = ((position % (tileDimension * tileDimension)) / tileDimension) + ((position / (tileDimension * tileDimension)) * tileDimension / width) * tileDimension;
            return point;
        }

        static private int bitmapPosition(Point coordinate, int width)
        {
            return coordinate.X + coordinate.Y * width;
        }

        static private int tiledPosition(Point coordinate, int width, int tileDimension)
        {
            if (width % tileDimension != 0)
                throw new ArgumentException("Bitmaps width needs to be multiple of tile's width.");

            return (coordinate.X % tileDimension + (coordinate.Y % tileDimension) * tileDimension +
                   (coordinate.X / tileDimension) * (tileDimension * tileDimension) +
                   (coordinate.Y / tileDimension) * (tileDimension * width));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="mode"></param>
        /// <param name="TSAdata"></param>
        /// <param name="offset"></param>
        /// <param name="TSAsize"></param>
        /// <returns>Tile8bit raw GBA graphics</returns>
        static public byte[] TSAmap(byte[] graphics, GraphicsMode mode, byte[] TSAdata, int offset, Size TSAsize)
        {
            byte[] result;
            if (TSAsize.Height * TSAsize.Width > TSAdata.Length / 2)
            {
                TSAsize.Height = TSAdata.Length / (2 * TSAsize.Width);
            }
            fixed (byte* graphicsPointer = &graphics[0])
            {
                fixed (byte* TSApointer = &TSAdata[offset])
                {
                    switch (mode)
                    {
                        case GraphicsMode.Tile8bit:
                            result = TSAmapFrom8bitTileGraphics(graphicsPointer, graphics.Length / 64, TSApointer, TSAsize);
                            break;
                        case GraphicsMode.Tile4bit:
                            result = TSAmapFrom4bitTileGraphics(graphicsPointer, graphics.Length / 32, TSApointer, TSAsize);
                            break;
                        default:
                            throw new ArgumentException("GraphicsMode has to be either Tile8bit or Tile4bit");
                    }
                }
            }
            return result;
        }

        static private byte[] TSAmapFrom8bitTileGraphics(byte* graphics, int amountOfGraphicsBlocks, byte* TSAdata, Size TSAsize)
        {
            int AmountTSAblocks = TSAsize.Height * TSAsize.Width;
            byte[] result = new byte[AmountTSAblocks * 64];

            fixed (byte* resultPointer = &result[0])
            {
                byte* destPointer = resultPointer;
                for (int i = 0; i < AmountTSAblocks; i++)
                {
                    ushort TSA = ((ushort*)TSAdata)[i];
                    int graphicsIndex = TSA & 0x3FF;
                    if (graphicsIndex < amountOfGraphicsBlocks)
                    {
                        bool Hflip = ((TSA >> 10) & 1) == 1;
                        bool Vflip = ((TSA >> 11) & 1) == 1;
                        byte* source = graphics + graphicsIndex * 64;
                        if (Hflip && Vflip)
                            source += 64 - 1;
                        else if (Vflip)
                            source += 64 - 8 + 1;
                        else if (Hflip)
                            source += 8 - 1;

                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 8; x++)
                            {
                                destPointer[0] = source[0];
                                destPointer++;
                                if (Hflip)
                                    source--;
                                else
                                    source++;
                            }

                            if (Hflip != Vflip)
                            {
                                if (Hflip)
                                    source += 8;
                                else
                                    source -= 8;
                            }
                        }
                    }
                }
            }
            return result;
        }

        static private byte[] TSAmapFrom4bitTileGraphics(byte* graphics, int amountOfGraphicsBlocks, byte* TSAdata, Size TSAsize)
        {
            int AmountTSAblocks = TSAsize.Height * TSAsize.Width;
            byte[] result = new byte[AmountTSAblocks * 64];

            fixed (byte* resultPointer = &result[0])
            {
                for (int i = 0; i < AmountTSAblocks; i++)
                {
                    ushort TSA = (ushort)(TSAdata[2 * i] + (TSAdata[2 * i + 1] << 8));
                    int graphicsIndex = TSA & 0x3FF;
                    byte* destPointer = resultPointer + 64 * i;

                    if (graphicsIndex < amountOfGraphicsBlocks)
                    {
                        int palette = (TSA >> 8) & 0xF0;
                        bool Hflip = ((TSA >> 10) & 1) == 1;
                        bool Vflip = ((TSA >> 11) & 1) == 1;
                        byte* source = graphics + graphicsIndex * 32;

                        if (Hflip && Vflip)
                            source += 32 - 1;
                        else if (Vflip)
                            source += 32 - 4;
                        else if (Hflip)
                            source += 4 - 1;

                        for (int y = 0; y < 8; y++)
                        {
                            for (int x = 0; x < 4; x++)
                            {
                                int second = source[0] >> 4;
                                int first = source[0] & 0xF;
                                if (Hflip)
                                {
                                    destPointer[0] = (byte)(second | palette);
                                    destPointer[1] = (byte)(first | palette);
                                    source--;
                                }
                                else
                                {
                                    destPointer[0] = (byte)(first | palette);
                                    destPointer[1] = (byte)(second | palette);
                                    source++;
                                }
                                destPointer += 2;
                            }

                            if (Hflip != Vflip)
                            {
                                if (Hflip)
                                    source += 8;
                                else
                                    source -= 8;
                            }
                        }
                    }
                }
            }

            return result;
        }

        static public byte[][] GenerateGBAImage(Bitmap bitmap, GraphicsMode mode)
        {
            byte[][] result;
            Bitmap quantazised;
            BitmapData bmp;

            switch (mode)
            {
                case GraphicsMode.Tile8bit:
                    result = new byte[3][];
                    quantazised = Quantazase(bitmap);

                    result[0] = ToTile8bit(bitmap, quantazised.Palette.Entries);
                    result[1] = toRawGBAPalette(quantazised.Palette.Entries);

                    //yes, I'm lazy
                    result[2] = new byte[result[0].Length * 2];
                    fixed (byte* temp = &result[2][0])
                    {
                        short* pointer = (short*)temp;
                        int tiles = 0;
                        for (int i = 0; i < result[0].Length; i++)
                        {
                            pointer[i] = (short)tiles++;
                        }
                    }

                    break;
                case GraphicsMode.Tile4bit:
                    result = new byte[3][];
                    quantazised = Quantazase(bitmap);

                    result[0] = ToTile4bit(bitmap, quantazised.Palette.Entries);
                    result[1] = toRawGBAPalette(quantazised.Palette.Entries);

                    //yes, I'm lazy
                    result[2] = new byte[result[0].Length * 2];
                    fixed (byte* temp = &result[2][0])
                    {
                        short* pointer = (short*)temp;
                        int tiles = 0;
                        for (int i = 0; i < result[0].Length; i++)
                        {
                            pointer[i] = (short)tiles++;
                        }
                    }
                    break;
                default:
                    throw new ArgumentException();
            }
            return result;
        }

        static private Bitmap Quantazase(Bitmap bitmap)
        {
            if (bitmap.PixelFormat == PixelFormat.Format8bppIndexed)
                return bitmap;

            Bitmap result = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            Bitmap trueColorBitmap;

            if (bitmap is Bitmap && bitmap.PixelFormat == PixelFormat.Format32bppArgb)
            {
                trueColorBitmap = bitmap;
            }
            else
            {
                trueColorBitmap = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);

                using (Graphics g = Graphics.FromImage(trueColorBitmap))
                {
                    g.PageUnit = GraphicsUnit.Pixel;
                    g.DrawImage(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
            }

            Octree<List<Color>> colors = new Octree<List<Color>>(5, 5);
            BitmapData bmpData = trueColorBitmap.LockBits(new Rectangle(new Point(), trueColorBitmap.Size), ImageLockMode.ReadOnly, trueColorBitmap.PixelFormat);
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