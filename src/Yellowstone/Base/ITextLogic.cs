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

namespace TheXDS.Vulcanium.Yellowstone.Base;

/// <summary>
/// Define un componente lógico que permite realizar operaciones de texto.
/// </summary>
public interface ITextLogic : ILogic
{
    /// <summary>
    /// Coloca el cursor de modo texto en la posición de fila/columna
    /// especificada.
    /// </summary>
    /// <param name="row">Fila en la cual ubicar el cursor.</param>
    /// <param name="column">Columna en la cual ubicar el cursor.</param>
    void Locate(in ushort row, in ushort column);

    /// <summary>
    /// Imprime una cadena y actualiza la posición del cursor.
    /// </summary>
    /// <param name="text">Texto a imprimir.</param>
    void Print(string text);

    /// <summary>
    /// Desplaza la pantalla una fila hacia abajo.
    /// </summary>
    void Scroll() => Scroll(1);

    /// <summary>
    /// Desplaza la pantalla un número de filas hacia abajo.
    /// </summary>
    /// <param name="lines">
    /// Número de filas a desplazar la pantalla.
    /// </param>
    void Scroll(ushort lines);
}
