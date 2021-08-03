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
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TheXDS.Vulcanium.Tajumulco.Server
{
    internal static class Program
    {
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
            else if (r.Tokens.Count == 0) return new IPEndPoint(IPAddress.Any, 65535);
            else
            {
                r.ErrorMessage = "No se soporta la creación de más de 1 socket de escucha.";
            }
            return null!;
        }
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
                    "--txbuffer",
                    () => 1460,
                    "Establece el tamaño del buffer de comunicaciones.")
            };
            rc.Handler = CommandHandler.Create<IPEndPoint, int>(async (endpoint, txbuffer) => {
                var l = new TcpListener(endpoint);
                var tcs = new TaskCompletionSource<bool>();
                Console.CancelKeyPress += (_, __) => tcs.SetResult(true);
                Console.WriteLine($"Servidor iniciado en {endpoint} con buffer de {txbuffer} bytes");

                l.Start();
                while (!tcs.Task.IsCompleted)
                {
                    var t = l.AcceptTcpClientAsync();

                    if (await Task.WhenAny(tcs.Task, t, Task.Run(Console.ReadLine)) == t)
                    {
                        AttendClientAsync(t.Result, txbuffer);
                    }
                    else
                    {
                        break;
                    }
                }
                l.Stop();

                Console.WriteLine("El servidor se ha detenido correctamente.");
                Environment.Exit(0);
            });
            return rc.InvokeAsync(args);
        }

        private static async void AttendClientAsync(TcpClient c, int txBuffer)
        {
            try
            {
                using var ns = c.GetStream();
                var bf = new byte[4];
                await ns.ReadAsync(bf);
                var total = BitConverter.ToInt32(bf);
                Console.WriteLine($"Atendiendo solicitud de {total} bytes por {c.Client.RemoteEndPoint}...");
                var tx = total;
                int rcvd = 0;
                var a = new byte[txBuffer];
                while (total > txBuffer)
                {
                    var r = await ns.ReadAsync(a);
                    total -= r;
                    rcvd += r;
                }
                if (total > 0)
                {
                    a = new byte[total];
                    var r = await ns.ReadAsync(a);
                    total -= r;
                    rcvd += r;
                }
                Console.WriteLine($"Recibidos {rcvd} bytes.");
                a = new byte[txBuffer];
                while (tx > txBuffer)
                {
                    await ns.WriteAsync(a);
                    tx -= txBuffer;
                }
                if (tx > 0)
                {
                    a = new byte[tx];
                    await ns.WriteAsync(a);
                }
                Console.WriteLine($"Solicitud completada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al procesar la solicitud: {ex.Message}.");
            }
        }
    }
}
