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

namespace TheXDS.Vulcanium.Yellowstone;

/// <summary>
/// Define la información de modo de pantalla.
/// </summary>
public readonly struct ScreenModeData
{
    /// <summary>
    /// Obtiene el modo de pantalla predeterminado.
    /// </summary>
    public static ScreenModeData Default => new(640, 400);

    /// <summary>
    /// Obtiene el modo de pantalla utilizado por un adaptador Monochrome
    /// Display Adapter (MDA).
    /// </summary>
    public static ScreenModeData Mda => new(720, 350, (Stream?)null);

    /// <summary>
    /// Obtiene un modo de pantalla con resolución de 640x400, con color de 32 bits.
    /// </summary>
    public static ScreenModeData TrueColor => new(640, 400, PixelFormats.Bgr32, null);

    /// <summary>
    /// Obtiene la altitud en pixeles de este modo de pantalla.
    /// </summary>
    public ushort Height { get; }

    /// <summary>
    /// Obtiene el ancho en pixeles de este modo de pantalla.
    /// </summary>
    public ushort Width { get; }

    /// <summary>
    /// Obtiene el formato de pixel de este modo de pantalla.
    /// </summary>
    public PixelFormat PixelFormat { get; }

    /// <summary>
    /// Para formatos de pixel basados en paleta, obtiene la paleta de color utilizada en este modo de pantalla.
    /// </summary>
    public BitmapPalette? Palette { get; }

    public float ScreenRatio => Width / Height;

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    public ScreenModeData(ushort width, ushort height) : this(width, height, PixelFormats.Indexed8, BitmapPalettes.Halftone8)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="paletteFile">
    /// Ruta al archivo que contiene la información de paleta a utilizar en
    /// este modo de pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, string paletteFilePath) : this(width, height, new FileInfo(paletteFilePath))
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="paletteFile">
    /// Archivo que contiene la información de paleta a utilizar en este
    /// modo de pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, FileInfo paletteFile) : this(width, height, paletteFile.Open(FileMode.Open))
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="paletteData">
    /// <see cref="Stream"/> con la información de paleta y formato de
    /// pixel a utilizar en este modo de pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, Stream? paletteData) : this(width, height, ReadPalette(paletteData))
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="paletteId">
    /// Id de la paleta de color a utilizar en este modo de pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, PaletteId paletteId) : this(width, height, ReadInternalPalette(paletteId.ToString()))
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="paletteData">
    /// Tupla con la información de paleta y formato de pixel a utilizar en
    /// este modo de pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, (PixelFormat pixelFormat, BitmapPalette palette) paletteData) : this(width, height, paletteData.pixelFormat, paletteData.palette)
    {
    }

    /// <summary>
    /// Inicializa una nueva instancia de la estructura
    /// <see cref="ScreenModeData"/>.
    /// </summary>
    /// <param name="width">Ancho del modo de pantalla.</param>
    /// <param name="height">Altura del modo de pantalla.</param>
    /// <param name="pixelFormat">
    /// Formato de pixel a utilizar en este modo de pantalla.
    /// </param>
    /// <param name="palette">
    /// Para formatos de pixel que admiten el uso de una paleta de color,
    /// define la paleta de color a utilizar al dibuar en este modo de
    /// pantalla.
    /// </param>
    public ScreenModeData(ushort width, ushort height, PixelFormat pixelFormat, BitmapPalette? palette)
    {
        Width = width;
        Height = height;
        PixelFormat = pixelFormat;
        Palette = palette;
    }

    /// <summary>
    /// Crea una superficie de dibujo con el tamaño, formato de pixel y
    /// paleta de este modo de pantalla.
    /// </summary>
    /// <returns>
    /// Un nuevo <see cref="WriteableBitmap"/> con el tamaño, formato de
    /// pixel y paleta de este modo de pantalla.
    /// </returns>
    public WriteableBitmap CreateSurface()
    {
        return new WriteableBitmap(Width, Height, 96, 96, PixelFormat, Palette);
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

    private static BitmapPalette MkPalette(Stream stream, in int colors)
    {
        return new BitmapPalette(MkColors(stream, colors).ToArray());
    }

    private static (PixelFormat, BitmapPalette) ReadInternalPalette(string id)
    {
        using var r = typeof(ScreenModeData).Assembly.GetManifestResourceStream($"Resources.{id}.pal");
        return ReadPalette(r);
    }

    private static (PixelFormat, BitmapPalette) ReadPalette(Stream? stream)
    {
        if (!(stream?.CanSeek ?? false))
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
}
