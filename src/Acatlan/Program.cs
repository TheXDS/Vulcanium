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
V U L C A N I U M - A C A T L A N
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
using System.Linq;

namespace TheXDS.Vulcanium.Acatlan
{
    internal static class Program
    {
        private static void Main()
        {
            const int size = 1000000;
            Console.WriteLine($"Inicializando arreglo de prueba con {size} elementos...");
            var c = Magma.InitArray(size);
            var d = 0;
            var tests = Magma.DiscoverObjects<MpTest>();
            foreach (var test in tests)
            {
                Console.WriteLine($"A continuación: {test.Name}{test.Description}");
                var time = System.Diagnostics.Stopwatch.StartNew();
                test.Run(c);
                time.Stop();
                test.Time = (int)time.ElapsedMilliseconds;
                Console.WriteLine($"{Environment.NewLine}Resultado: {test.Count}");
                Console.WriteLine(new string('-', 80));
                if (((ITest)test).IsDefault)
                {
                    d = test.Count;
                }
            }
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Resultados de las pruebas (en orden descendente de tiempo):");
            foreach (var j in tests.OrderByDescending(p => p.Time))
            {
                Console.WriteLine($"{string.Concat((((ITest)j).IsDefault ? "(Referencia) " : null), j.Name),-50}{(j.Count == d ? "[OK]" : $"[FAIL ({d - j.Count})]")} en {j.Time} ms");
            }
        }
    }
}
