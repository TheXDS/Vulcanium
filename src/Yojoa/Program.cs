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
V U L C A N I U M - Y O J O A
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
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Yojoa
{
    internal class Program
    {
        private static async Task Main()
        {
            var c = new Crasher();
            Console.WriteLine("Este programa simula una operación multi-hilo que se bloquea por un recurso común, además de una prueba de aborto de hilos bloqueados que superen un tiempo de espera prudencial. Presione cualquier tecla para continuar.");
            Console.ReadKey();
            await c.HangAsync(5);
        }
    }

    internal class Crasher
    {
        private const int MinMs = 3000;
        private const int MaxMs = 12000;
        private const int Threshold = 8000;

        private int _counter;
        private readonly object _lock = new object();


        private void Hang(int hangMilliseconds)
        {
            var thread = ++_counter;
            Console.WriteLine($"Hilo {thread} a la espera");

            lock (_lock)
            {
                Console.WriteLine($"Inicio de hilo {thread}... Hang {hangMilliseconds} ms!");

                var task = Task.Run(() =>
                {
                    // Simula una operación cualquiera en el objeto bloqueado.
                    var _ = _lock.ToString();

                    // Este bloque intencionalmente bloquea la ejecución.
                    Thread.Sleep(hangMilliseconds);
                    Console.WriteLine($"Hilo {thread} finalizará correctamente.");
                });

                Console.WriteLine(task.Wait(TimeSpan.FromMilliseconds(Threshold))
                    ? $"Fin de hilo {thread}"
                    : $"Abortar hilo {thread}");
            }
        }

        public Task HangAsync(int threads)
        {
            var rnd = new Random();
            var tasks = new Collection<Task>();

            for (var j = 1; j <= threads; j++)
            {
                tasks.Add(Task.Run(() => Hang(rnd.Next(MinMs, MaxMs))));
            }

            return Task.WhenAll(tasks);
        }
    }
}
