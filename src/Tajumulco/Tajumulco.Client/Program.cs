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
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
 
namespace TheXDS.Vulcanium.Tajumulco.Client
{
    internal static class Program
    {
        private static Task<int> Main(string[] args)
        {
            var rc = new RootCommand("Servicio de prueba de ancho de banda")
            {
                new Option<IPEndPoint>(
                     "--endpoint",
                    GetEndpoint,
                    true,
                    "Establece la IP y el puerto a utilizar para escuchar conexiones entrantes."),
                new Option<int>(
                    "--bytes",
                    () => 104857600,
                    "Establece la cantidad de bytes a enviar."),
                new Option<int>(
                    "--txbuffer",
                    () => 1460,
                    "Establece el tamaño del buffer de comunicaciones.")
            };

            rc.Handler = CommandHandler.Create<IPEndPoint, int, int>(async (endpoint, bytes, txbuffer) =>
            {
                Console.WriteLine($"Buffer de transmisión: {txbuffer} bytes");
                Console.WriteLine($"Pre-compilando (Test de {txbuffer} bytes");
                await Test(endpoint, txbuffer, txbuffer);
                await Test(endpoint, bytes, txbuffer);
            });
            return rc.InvokeAsync(args);
        }

        private static async Task Test(IPEndPoint endpoint, int bytes, int txbuffer)
        {
            Console.WriteLine($"Enviando {bytes} bytes de prueba");
            using var c = new TcpClient();
            await c.ConnectAsync(endpoint.Address, endpoint.Port);
            using var ns = c.GetStream();
            await ns.WriteAsync(BitConverter.GetBytes(bytes));

            var t = new Stopwatch();
            var tx = bytes;
            t.Start();
            var a = new byte[txbuffer];
            while (tx > txbuffer)
            {
                await ns.WriteAsync(a);
                tx -= txbuffer;
            }
            if (tx > 0)
            {
                a = new byte[tx];
                await ns.WriteAsync(a);
            }
            t.Stop();
            var upldSpd = t.ElapsedMilliseconds;

            var total = bytes;
            t.Restart();
            a = new byte[txbuffer];
            while (total > txbuffer)
            {
                total -= await ns.ReadAsync(a);
            }
            if (total > 0)
            {
                a = new byte[total];
                total -= await ns.ReadAsync(a);
            }
            t.Stop();
            var dwldSpd = t.ElapsedMilliseconds;
            c.Close();

            Console.WriteLine($"Velocidad de subida TCP: {Spd(upldSpd, bytes)}");
            Console.WriteLine($"Velocidad de bajada TCP: {Spd(dwldSpd, bytes)}");
        }

        private static string Spd(long ms, int bytes)
        {
            if (ms > 0)
            {
                var spd = bytes / (double) ms / 1048576.0 * 1000.0;
                return $"{bytes} bytes en {ms/1000.0} @ {spd:f2} MiB/s ({spd * 8:f2} Mbps)";
            }
            else return "Instantáneo";
        }

        private static IPEndPoint GetEndpoint(ArgumentResult r)
        {
            if (r.Tokens.Count == 1)
            {
                if (IPEndPoint.TryParse(r.Tokens[0].Value, out var ep)) return ep;
                else
                {
                    r.ErrorMessage = "Se esperaba un valor con el formato '0.0.0.0:00000'";
                }
            }
            else if (r.Tokens.Count == 0) return new IPEndPoint(IPAddress.Loopback, 65535);
            else
            {
                r.ErrorMessage = "No se soporta la conexión a más de un servidor.";
            }
            return null!;
        }
    }
}
