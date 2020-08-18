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
V U L C A N I U M - T A J U M U L C O
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
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TheXDS.Vulcanium.Tajumulco.Client
{
    internal static class Program
    {
        private const int FillerSize = 104857600; 
        private const int TxBuffer = 32768;

        private static byte[] _filler = new byte[FillerSize];

        private static async Task Main()
        {
            //var ep = new IPEndPoint(IPAddress.Loopback, 7);
            var ep = new IPEndPoint(new IPAddress(new byte[] {192, 168, 0, 1}), 7);
            //await Task.WhenAll(TcpTest(ep), UdpTest(ep));
            Console.WriteLine($"Buffer de transmisión: {TxBuffer} bytes");
            //await UdpTest(ep);
            await TcpTest(ep);
        }
        
        private static async Task UdpTest(IPEndPoint ep)
        {
            var t = new Stopwatch();
            using var udp = new UdpClient();
            
            t.Start();
            for (var j = 0; j < FillerSize; j += TxBuffer)
            {
                var p = _filler.Skip(j).Take(TxBuffer).ToArray();
                await udp.SendAsync(p, p.Length,ep);
            }
            t.Stop();
            
            var upldSpd = t.ElapsedMilliseconds;
            
            t.Restart();
            while (udp.Available > 0)
            {
                await udp.ReceiveAsync();
            }
            t.Stop();
            try { udp.Close(); } catch { }
            var dwldSpd = t.ElapsedMilliseconds;

            
            Console.WriteLine($"Velocidad de subida UDP: {Spd(upldSpd)}");
            Console.WriteLine($"Velocidad de bajada UDP: {Spd(dwldSpd)}");

            Console.WriteLine("Test UDP finalizado.");
        }

        private static async Task TcpTest(IPEndPoint ep)
        {
            var t = new Stopwatch();
            var tcp = new TcpClient();
            await tcp.ConnectAsync(ep.Address, ep.Port);
            
            tcp.GetStream().Write(BitConverter.GetBytes(FillerSize));

            t.Start();
            for (var j = 0; j < FillerSize; j += TxBuffer)
            {
                await tcp.GetStream().WriteAsync(_filler.Skip(j).Take(TxBuffer).ToArray());
            }
            t.Stop();
            var upldSpd = t.ElapsedMilliseconds;

            
            t.Restart();
            var bsz = 0;
           
            var btsz = new byte[4];
            tcp.GetStream().Read(btsz);
            var tsz = BitConverter.ToInt32(btsz);
            var cnt = 0;
            while (cnt < tsz)
            {
                while ((bsz = tcp.Available) > 0)
                {
                    var b = new byte [bsz];
                    cnt += await tcp.GetStream().ReadAsync(b,0,bsz);
                }
            }

            t.Stop();
            tcp.Close();
            var dwldSpd = t.ElapsedMilliseconds;
            
            Console.WriteLine($"Velocidad de subida TCP: {Spd(upldSpd)}");
            Console.WriteLine($"Velocidad de bajada TCP: {Spd(dwldSpd)}");

            Console.WriteLine("Test TCP finalizado.");
        }

        private static string Spd(long ms)
        {
            if (ms > 0)
            {
                var spd = FillerSize / (double) ms / 1048576.0 * 1000.0;
                return $"{spd:f2} MiB/s ({spd * 8:f2} Mbps)";
            }
            else return "Instantáneo";
        }
    }
}
