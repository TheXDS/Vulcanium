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
V U L C A N I U M - A R I C H U A
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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;

namespace TheXDS.Vulcanium.Arichua
{
    internal static class Program
    {
        private static readonly IEnumerable<Hammer> hammers = DetectHammers();

        private static async Task Main()
        {
            var time = new Stopwatch();
            SetPriority(ProcessPriorityClass.Idle);
            var tasks = new List<Task>();
            time.Start();
            Console.WriteLine($"Tortura iniciada el {DateTime.Now}");
            foreach (var j in hammers)
            {
                tasks.Add(j.Torture());
            }
            Console.CancelKeyPress += OnAbortTorture;
            await Task.WhenAll(tasks);
            time.Stop();
            Console.WriteLine($"Tortura finalizada el {DateTime.Now}, tiempo total: {time.Elapsed}");
        }

        private static IEnumerable<Hammer> DetectHammers()
        {
            var hammers = new List<Hammer>();

            foreach (var t in typeof(Program).Assembly.GetTypes())
            {
                if (TryInstance<Hammer>(t, out var test))
                {
                    Console.WriteLine($"{t.Name} detectado.");
                    hammers.Add(test);
                }
            }

            return hammers;
        }

        private static void OnAbortTorture(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Deteniendo tests de tortura...");
            foreach (var h in hammers) h.Abort();
        }

        private static void SetPriority(ProcessPriorityClass priority)
        {
            var thisProcess = Process.GetCurrentProcess();
            try
            {
                thisProcess.PriorityClass = priority;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"No fue posible cambiar la prioridad de esta aplicación: {ex.Message}. Continuando con prioridad {thisProcess.PriorityClass}");
            }
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
    }

    public abstract class Hammer
    {
        private readonly CancellationTokenSource _abortSignal = new CancellationTokenSource();

        public virtual void Abort()
        {
            _abortSignal.Cancel();
        }

        protected abstract void OnTorture();

        public virtual Task Torture()
        {
            return Task.Run(OnTorture, _abortSignal.Token);
        }

        public virtual Task Torture(TimeSpan span)
        {
            _abortSignal.CancelAfter(span);
            return Torture();
        }
    }

    public class AluHammer : Hammer
    {
        private static readonly int[] knownPrimes = {
            2,   3,   5,   7,   11,  13,  17,  19,  23,  29,
            /*
            31,  37,  41,  43,  47,  53,  59,  61,  67,  71, 
            73,  79,  83,  89,  97,  101, 103, 107, 109, 113,
            127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
            179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
            233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
            283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
            353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
            419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
            467, 479, 487, 491, 499, 503, 509, 521, 523, 541
            */
        };

        protected static bool IsPrime(int value)
        {
            if (value == 2) return true;
            if (value == 1 || value % 2 == 0) return false;

            foreach (var prime in knownPrimes)
                if (value % prime == 0) return false;

            var l = (int)Math.Sqrt(value);
            for (int k = knownPrimes.Max() + 2; k <= l; k += 2)
                if (value % k == 0) return false;

            return true;
        }

        private readonly int[] array = InitArray();

        private static int[] InitArray(int size = 1000000)
        {
            var c = new int[size];
            var u = c.GetUpperBound(0);
            var rnd = new Random();
            for (var j = 0; j < u; j++) c[j] = rnd.Next(1, int.MaxValue);
            return c;
        }

        protected override void OnTorture()
        {
            while (true)
            {
                var count = 0;
                var part = Partitioner.Create(array);
                var syncRoot = new object();

                void TestIfPrime(int j)
                {
                    
                    if (IsPrime(j)) lock(syncRoot) count++;
                }

                Parallel.ForEach(part, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, TestIfPrime);
            }
        }
    }
}
