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
V U L C A N I U M - M O M O T O M B O
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
using System.IO;
using System.Linq;

namespace TheXDS.Vulcanium.Momotombo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Contains("--green")) Console.ForegroundColor = ConsoleColor.DarkGreen;
            if (args.Contains("--amber")) Console.ForegroundColor = ConsoleColor.DarkYellow;
            var baud = args.SingleOrDefault(p => p.StartsWith("--baud=")) is { } s && s.Split(new[] { '=' }, 2)[1] is { } v && int.TryParse(v, out var b) ? b : 300;
            var slp = (int)(1000.0 / (baud / 8.0));
            using var cin = Console.OpenStandardInput();
            using var cout = Console.OpenStandardOutput();
            using var sr = new BinaryReader(cin);
            using var sw = new BinaryWriter(cout);
            try
            {
                while (cin.CanRead)
                {
                    sw.Write(sr.ReadChar());
                    System.Threading.Thread.Sleep(slp);
                }
            }
            catch { }
        }
    }
}
