/*
MIT License

Copyright (c) 2019-2020 César Andrés Morgan

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
V U L C A N I U M - K I L A U E A
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

namespace TheXDS.Vulcanium.Kilauea;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("V U L C A N I U M - K I L A U E A");
        Console.WriteLine("Test de controles de UI para terminal. Presione una tecla para continuar.");
        Console.ReadKey();
    }
}

public class Surface
{
    public List<Widget> Widgets { get; } = new List<Widget>();

    private Texel[,] _area;

    public short Width => (short)_area.GetUpperBound(0);
    public short Height => (short)_area.GetUpperBound(1);

    public ConsoleColor Foreground { get; private set; }
    public ConsoleColor Background { get; private set; }

    public void SetForeground(ConsoleColor? value)
    {
        Foreground = value ?? ConsoleColor.Gray;
    }
    public void SetBackground(ConsoleColor? value)
    {
        Background = value ?? ConsoleColor.Black;
    }


}
public abstract class Widget
{
    private short _y;
    private short _x;

    public short X
    {
        get => _x; set
        {
            _x = value;
            Redraw();
        }
    }
    public short Y
    {
        get => _y; set
        {
            _y = value;
            Redraw();
        }
    }
    public void Move(short x, short y)
    {
        _x = x;
        _y = y;
        Redraw();
    }
    public abstract void Redraw();
}

public class Label : Widget
{
    public override void Redraw()
    {
        throw new NotImplementedException();
    }
}

public struct Texel
{
    public char Value { get; set; }
    public ConsoleColor? Foreground { get; set; }
    public ConsoleColor? Background { get; set; }
}
