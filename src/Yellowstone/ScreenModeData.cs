/*
MIT License

Copyright (c) 2020 César Andrés Morgan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

===============================================================================
V U L C A N I U M - Y E L L O W S T O N E
  _.----._
 (   (    )
(  (    )  )
 (________)
    ||||
  --++++--
    ||||
  .(    ).
 (_(____)_)
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TheXDS.Vulcanium.Yellowstone
{
    public struct ScreenModeData
    {
        public static ScreenModeData Default => new ScreenModeData(640, 400);
        public static ScreenModeData TrueColor => new ScreenModeData(640, 400, PixelFormats.Bgr32, null);

        public int Height { get; }
        public int Width { get; }
        public PixelFormat PixelFormat { get; }
        public BitmapPalette? Palette { get; }

        public ScreenModeData(int width, int height) : this(width, height, PixelFormats.Indexed8, BitmapPalettes.Halftone8)
        {
        }

        public ScreenModeData(int width, int height, string paletteFilePath) : this (width, height, new FileInfo(paletteFilePath))
        {
        }

        public ScreenModeData(int width, int height, FileInfo paletteFile) : this(width, height, paletteFile.Open(FileMode.Open))
        {
        }

        public ScreenModeData(int width, int height, Stream paletteData) : this(width, height, ReadPalette(paletteData))
        {
        }

        public ScreenModeData(int width, int height, PaletteId paletteId) : this(width, height, ReadInternalPalette(paletteId.ToString()))
        {
        }

        public ScreenModeData(int width, int height, (PixelFormat pixelFormat, BitmapPalette palette) paletteData) : this (width, height, paletteData.pixelFormat, paletteData.palette)
        {
        }

        public ScreenModeData(int width, int height, PixelFormat pixelFormat, BitmapPalette? palette)
        {
            Width = width;
            Height = height;
            PixelFormat = pixelFormat;
            Palette = palette;
        }

        private static (PixelFormat, BitmapPalette) ReadInternalPalette(string id)
        {
            using var r = typeof(ScreenModeData).Assembly.GetManifestResourceStream($"Resources.{id}.pal");
            return ReadPalette(r);
        }
        private static (PixelFormat, BitmapPalette) ReadPalette(Stream? stream)
        {
            if (!stream?.CanSeek ?? false)
            {
                using var ms = new MemoryStream();
                stream?.CopyTo(ms);
                return ReadPalette(ms);
            }
            if (stream!.Length % 3 != 0) throw new FormatException();

            return (stream.Length / 3) switch
            {
                0 => (PixelFormats.Indexed1, BitmapPalettes.BlackAndWhite),
                1 => (PixelFormats.Indexed1, new BitmapPalette(MkColors(stream, 1).Append(Colors.Black).ToArray())),
                2 => (PixelFormats.Indexed1, MkPalette(stream, 2)),
                4 => (PixelFormats.Indexed2, MkPalette(stream, 4)),
                16 => (PixelFormats.Indexed4, MkPalette(stream, 16)),
                256 => (PixelFormats.Indexed8, MkPalette(stream, 256)),
                _ => throw new FormatException()
            };
        }
        private static BitmapPalette MkPalette(Stream stream, in int colors)
        {
            return new BitmapPalette(MkColors(stream, colors).ToArray());
        }
        private static IEnumerable<Color> MkColors(Stream stream, int colors)
        {
            while (colors-- > 0)
            {
                yield return new Color
                {
                    A = 255,
                    R = (byte)stream.ReadByte(),
                    G = (byte)stream.ReadByte(),
                    B = (byte)stream.ReadByte()
                };
            }
        }

        public WriteableBitmap CreateSurface()
        {
            return new WriteableBitmap(Width, Height, 96, 96, PixelFormat, Palette);
        }
    }
}
