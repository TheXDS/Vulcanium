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
V U L C A N I U M - V E S U V I O
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
using System.Linq;

namespace TheXDS.Vulcanium.Vesuvio;

internal static class Program
{
    private static readonly List<Test> Tests = Magma.DiscoverObjects<Test>().ToList();
    
    private static void Main()
    {
        const int size = 1000000;
        if (!Magma.SetMaxPriority()) Console.WriteLine($"No fue posible cambiar la prioridad de esta aplicación.");
        
        Console.WriteLine($"Precompilando tests...");
        RunTests(Environment.ProcessorCount * 10, false);
        
        Console.WriteLine($"Inicializando arreglo de prueba con {size} elementos...");
        RunTests(size, true);

        var best = Tests.OrderBy(p => p.Time).First();
        Console.WriteLine($"Mejor tiempo: {best.Name}, {best.Time} ms");
    }

    private static void RunTests(in int arraySize, bool verbose)
    {
        var c = Magma.InitArray(arraySize);
        foreach(var test in Tests)
        {
            if (verbose) Console.Write($"Conteo utilizando {test.Name}... ");
            test.Run(c);
            if (verbose) Console.WriteLine($"{test.Count}, Tiempo: {test.Time} ms");
        }
    }
}