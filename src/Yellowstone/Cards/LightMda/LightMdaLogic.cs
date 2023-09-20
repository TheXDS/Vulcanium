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
using System.Text;
using TheXDS.Vulcanium.Yellowstone.Base;

namespace TheXDS.Vulcanium.Yellowstone.Cards.LightMda;

/// <summary>
/// Implementa un controlador lógico de modo texto basado en un subset de
/// características de MDA para modo texto. Únicamente soporta resoluciones de
/// texto de 80x25 y no implementa bytes de atributo.
/// </summary>
public class LightMdaLogic : ITextLogic
{
    private readonly IVideoRamPort vram;
    private ushort row;
    private ushort col;

    /// <summary>
    /// Obtiene o establece la cantidad de columnas disponibles en el modo de
    /// pantalla activo.
    /// </summary>
    public ushort Width { get => 80; set => throw new InvalidOperationException(); }

    /// <summary>
    /// Obtiene o establece la cantidad de filas disponibles en el modo de 
    /// pantalla activo.
    /// </summary>
    public ushort Height { get => 25; set => throw new InvalidOperationException(); }

    /// <summary>
    /// Obtiene o establece la página de códigos a utilizar para decodificar
    /// los caracteres a ser presentados.
    /// </summary>
    public ushort Codepage { get; set; } = 437;

    /// <summary>
    /// Obtiene o establece la fila actual del cursor en la pantalla.
    /// </summary>
    public ushort Row
    {
        get => (ushort)(row + 1);
        set
        {
            if (value < 1 || value > Width) throw new ArgumentOutOfRangeException(nameof(value));
            row = (ushort)(value - 1);
        }
    }

    /// <summary>
    /// Obtiene o establece la columna actual del cursor en la pantalla.
    /// </summary>
    public ushort Column
    {
        get => (ushort)(col + 1);
        set
        {
            if (value < 1 || value > Height) throw new ArgumentOutOfRangeException(nameof(value));
            col = (ushort)(value - 1);
        }
    }

    IVideoRamPort IGraphicsComponent.VideoRamPort => vram;

    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="LightMdaLogic"/>.
    /// </summary>
    /// <param name="videoRamPort">
    /// Puerto de memoria a utilizar para comunicarse con la memoria de video.
    /// </param>
    public LightMdaLogic(IVideoRamPort videoRamPort)
    {
        vram = videoRamPort;
    }

    void ITextLogic.Locate(in ushort row, in ushort column)
    {
        Row = row;
        Column = column;
    }

    void ITextLogic.Print(string text)
    {
        var offset = row * Width + col;
        vram.WriteBlock(Encoding.GetEncoding(Codepage).GetBytes(text), offset);
    }

    void ITextLogic.Scroll(ushort lines)
    {
        vram.ExecuteWithFullAccess(a =>
        {
            for (var j = 0; j < Height - lines; j++)
            {
                Array.Copy(a, (lines + j) * Width, a, j * Width, Width);
            }
            var emptyBuff = new byte[Width];
            for (var j = Height - lines; j < Height; j++)
            {
                Array.Copy(emptyBuff, 0, a, j * Width, Width);
            }
        });
        vram.RefreshAll();
    }
}
