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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace TheXDS.Vulcanium.Vesuvio
{
    internal static class Program
    {
        private static void SetMaxPriority()
        {
            var thisProcess = Process.GetCurrentProcess();
            try
            {
                thisProcess.PriorityClass = ProcessPriorityClass.RealTime;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No fue posible cambiar la prioridad de esta aplicación: {ex.Message}. Continuando con prioridad {thisProcess.PriorityClass}");
            }
        }

        private static int[] InitArray(int size = 1000000)
        {
            Console.WriteLine($"Inicializando arreglo de prueba con {size} elementos...");
            var c = new int[size];
            var u = c.GetUpperBound(0);
            var rnd = new Random();
            for (var j = 0; j < u; j++) c[j] = rnd.Next(1, int.MaxValue);
            return c;
        }

        private static IEnumerable<Test> DetectTests()
        {
            var tests = new List<Test>();

            foreach (var t in typeof(Program).Assembly.GetTypes())
            {
                if (TryInstance<Test>(t, out var test))
                {
                    tests.Add(test);
                }
                if (TryInstance<ITestFactory>(t, out var factory))
                {
                    tests.AddRange(factory.ManufactureTests());
                }                
            }

            return tests;
        }

        private static bool TryInstance<T>(this Type t, [NotNullWhen(true)] [MaybeNullWhen(false)] out T instance)
        {
            if (t is null) throw new ArgumentNullException(nameof(t));
            if (!t.IsAbstract && !t.IsInterface &&
                typeof(T).IsAssignableFrom(t) &&
                t.GetConstructor(Type.EmptyTypes) is { } ctor)
            {
                try
                {
                    instance = (T)ctor.Invoke(null);
                    return true;
                }
                catch { }
            }
            instance = default;
            return false;
        }

        private static void Main()
        {
            SetMaxPriority();
            var c = InitArray();
            var tests = DetectTests();
            foreach(var test in tests)
            {
                test.Run(c);
            }

            var best = tests.OrderBy(p => p.Time).First();

            Console.WriteLine($"Mejor tiempo: {best.Name}, {best.Time} ms");
        }
    }
}