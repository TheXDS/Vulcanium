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

using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using TheXDS.Vulcanium.Yellowstone.Base;

namespace TheXDS.Vulcanium.Yellowstone.Cards.LightMda;

/// <summary>
/// Implementa un Raster basado en un subset de características de MDA para
/// modo texto, con una resolución de 720x350, monocromo de 1 bit. Consume
/// 2KB de VRAM, únicamente soporta resoluciones de texto de 80x25 y no
/// implementa bytes de atributo.
/// </summary>
public class LightMdaRaster : IRaster<WriteableBitmap>
{
    private static readonly ScreenModeData mode = ScreenModeData.Mda;
    private readonly IVideoRamPort _vram;
    private readonly byte[][] _charRom;

    private static byte FlipBits(byte b)
    {
        return (byte)((byte)((b & 1) << 7)
            | (byte)((b & 2) << 5)
            | (byte)((b & 4) << 3)
            | (byte)((b & 8) << 1)
            | (byte)((b & 16) >> 1)
            | (byte)((b & 32) >> 3)
            | (byte)((b & 64) >> 5)
            | (byte)((b & 128) >> 7));
    }

    private static byte GetLastRowBits(byte character, byte bitRow)
    {
        return character >= 0xc0 && character <= 0xdf ? (byte)(bitRow << 7 & 128) : (byte)0;
    }

    private static byte[][] LoadChrRom(Stream s)
    {
        using var ms = new MemoryStream();
        s.CopyTo(ms);
        var mm = ms.ToArray();
        byte[][] chrBitmap = new byte[256][];
        for (var j = 0; j < 256; j++)
        {
            chrBitmap[j] = mm[(j * 16 + 1)..(j * 16 + 15)].Select(FlipBits).ToArray();
        }
        s.Dispose();
        return chrBitmap;
    }

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="LightMdaRaster"/>.
    /// </summary>
    /// <param name="vram">
    /// Puerto a utilizar para comunicarse con la memoria de video.
    /// </param>
    /// <param name="characterRom">
    /// <see cref="Stream"/> desde el cual leer la ROM de caracteres.
    /// </param>
    public LightMdaRaster(IVideoRamPort vram, Stream characterRom)
    {
        _vram = vram;
        _charRom = LoadChrRom(characterRom);
        vram.SubscribeVideoRamChangeObserver(OnVramChange);
    }

    /// <inheritdoc/>
    public WriteableBitmap ScreenOutput { get; } = mode.CreateSurface();

    /// <inheritdoc/>
    public int VideoRamOffset { get; set; }

    IVideoRamPort IGraphicsComponent.VideoRamPort => _vram;

    private void DrawChar(byte c, int i, int j)
    {
        var xx = _charRom[c].Select(p => GetLastRowBits(c, p)).ToArray();
        ScreenOutput.WritePixels(new Int32Rect(j * 9, i * 14, 8, 14), _charRom[c], 1, 0);
        ScreenOutput.WritePixels(new Int32Rect(j * 9 + 8, i * 14, 1, 14), xx, 1, 0);
    }

    private void OnVramChange(in byte[] buffer, in int offset)
    {
        var o = offset;
        var row = (offset - VideoRamOffset) / 80;
        var col = (offset - VideoRamOffset) % 80;

        foreach (var c in buffer)
        {
            if (o >= 0) { DrawChar(c, row, col); }
            else { o++; }
            col = (col + 1) % 80;
            if (col == 0) row++;
            if (row == 25) break;
        }
    }
}
