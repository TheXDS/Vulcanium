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
V U L C A N I U M - K A T M A I
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

using E = System.Text.Encoding;

namespace TheXDS.Vulcanium.Katmai;

internal class BrainFMachine
{
    private readonly byte[] mem;
    private int memoryPointer;
    private int programPointer;
    private readonly IReadOnlyDictionary<char, Action> symbols;

    public string Program { get; }

    public BrainFMachine(in string program, in int memSize = 30000)
    {
        mem = new byte[memSize];
        Program = program;

        symbols = new Dictionary<char, Action>()
        {
            { '+', Inc },
            { '-', Dec },
            { '>', Fwd },
            { '<', Bck },
            { '.', Out },
            { ',', Inp },
            { '[', Sub },
            { ']', Ret },
        };
    }

    public void Run()
    {
        for (;programPointer < Program.Length; programPointer++)
        {
            if (symbols.TryGetValue(Program[programPointer], out var action))
            {
                action.Invoke();
            }
        }
    }

    private void Inc()
    {
        unchecked { mem[memoryPointer]++; }
    }

    private void Dec()
    {
        unchecked { mem[memoryPointer]--; }
    }

    private void Fwd()
    {
        memoryPointer = memoryPointer < mem.GetUpperBound(0) ? memoryPointer + 1 : 0;
    }

    private void Bck()
    {
        memoryPointer = memoryPointer > 0 ? memoryPointer - 1 : mem.GetUpperBound(0);
    }

    private void Out()
    {
        Console.Write(E.ASCII.GetString(mem, memoryPointer, 1));
    }

    private void Inp()
    {
        mem[memoryPointer] = E.ASCII.GetBytes(new char[] { Console.ReadKey().KeyChar })[0];
    }

    private void Sub()
    {
        if (mem[memoryPointer] == 0)
        {
            var l = 0;
            for (var p = programPointer+1; p < Program.Length; p++)
            {
                if (Program[p] == '[') l++;
                if (Program[p] == ']')
                {
                    if (l == 0)
                    {
                        programPointer = p;
                        break;
                    }
                    else
                    {
                        l--;
                    }
                }
            }
        }
    }

    private void Ret()
    {
        if (mem[memoryPointer] != 0)
        {
            var l = 0;
            for (var p = programPointer - 1; p >= 0; p--)
            {
                if (Program[p] == ']') l++;
                if (Program[p] == '[')
                {
                    if (l == 0)
                    {
                        programPointer = p;
                        break;
                    }
                    else
                    {
                        l--;
                    }
                }
            }
        }
    }
}