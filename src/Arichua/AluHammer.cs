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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;

namespace TheXDS.Vulcanium.Arichua;

public class AluHammer : Hammer
{

    private readonly int[] array = Magma.InitArray(1000000);
    
    public override Task Torture()
    {
        return Task.Run(OnTorture);
    }

    protected override void OnTorture()
    {
        while (!_abortSignal.IsCancellationRequested)
        {
            var count = 0;
            var part = Partitioner.Create(array);
            var syncRoot = new object();

            void TestIfPrime(int j)
            {
                if (Magma.IsPrime(j)) lock(syncRoot) count++;
            }
            try
            {
                Parallel.ForEach(part, new ParallelOptions 
                { 
                    MaxDegreeOfParallelism = Environment.ProcessorCount,
                    CancellationToken = _abortSignal.Token
                }, TestIfPrime);
            }
            catch { }
        }
    }
}
