﻿/*
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

namespace TheXDS.Vulcanium.Yellowstone.Base;

/// <summary>
/// Describe un componente gráfico que ejecuta funciones de raster; tomando
/// información de la memoria de video para presentarla en un dispositivo
/// de salida, como un monitor.
/// </summary>
public interface IRaster : IGraphicsComponent
{
    /// <summary>
    /// Obtiene un objeto que representa la salida de video del raster.
    /// </summary>
    object ScreenOutput { get; }

    /// <summary>
    /// Obtiene o establece la posición desde la cual esta instancia leerá
    /// la memoria para crear la salida de video.
    /// </summary>
    int VideoRamOffset { get; set; }
}
