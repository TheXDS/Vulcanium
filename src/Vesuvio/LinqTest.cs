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

using System.Diagnostics;
using System.Linq;

namespace TheXDS.Vulcanium.Vesuvio;

internal class LinqTest : Test
{
    public override string Name => "Count(Func<int, bool>) de Linq";

    protected override void Benchmark(int[] array, Stopwatch t, ref int count)
    {
        t.Start();
        count = array.Count(p => Magma.IsPrime(p));
        t.Stop();
    }
}

internal class LinqTest2 : Test
{
    public override string Name => "uso no tradicional de Linq";

    protected override void Benchmark(int[] array, Stopwatch t, ref int count)
    {
        t.Start();
        count = array.Select(p => Magma.IsPrime(p)).Count(p => p);
        t.Stop();
    }
}