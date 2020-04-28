﻿/*
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
using System.Diagnostics;
using System.Linq;

namespace TheXDS.Vulcanium.Vesuvio
{
    internal abstract class Test
    {
        private static readonly int[] knownPrimes = {
            2,   3,   5,   7,   11,  13,  17,  19,  23,  29,
            31,  37,  41,  43,  47,  53,  59,  61,  67,  71, 
            73,  79,  83,  89,  97,  101, 103, 107, 109, 113,
            127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
            179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
            233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
            283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
            353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
            419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
            467, 479, 487, 491, 499, 503, 509, 521, 523, 541
        };

        public abstract string Name { get; }
        public abstract void Benchmark(int[] array, Stopwatch t, ref int count);
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

        public long Time { get; set; }
        public void Run(int[] array)
        {
            Console.Write($"Conteo utilizando {Name}... ");
            var t = new Stopwatch();
            var count = 0;
            Benchmark(array, t, ref count);
            Time = t.ElapsedMilliseconds;
            Console.WriteLine($"{count}, Tiempo: {Time} ms");
        }
    }
}