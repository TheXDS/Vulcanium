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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace TheXDS.Vulcanium.Tajumulco.Magma
{
    public static class Magma
    {
    }

    /// <summary>
    /// Abstrae el control y la transmisión de datos arbitrariamente grandes
    /// por medio del protocolo TCP.
    /// </summary>
    public class TcpTunnel
    {
        public TcpClient Client { get; }
        public ushort TxBufferSize { get; set; } = 32768;

        public TcpTunnel(TcpClient client)
        {
            Client = client;
            client.SendBufferSize
        }

        public async Task SendAsync(byte[] data)
        {
            Client.GetStream().Write(BitConverter.GetBytes(data.Length));
            for (var j = 0; j < data.Length; j += TxBufferSize)
            {
                await Client.GetStream().WriteAsync(data.Skip(j).Take(TxBufferSize).ToArray());
            }
        }

        public async Task<byte[]> ReceiveAsync()
        {
            var data = new List<byte>();
            var btsz = new byte[4];
            Client.GetStream().Read(btsz);
            var tsz = BitConverter.ToInt32(btsz);
            while (data.Count < tsz)
            {
                var bsz = 0;
                while ((bsz = c.Available) > 0)
                {
                    var b = new byte [bsz];
                    await c.GetStream().ReadAsync(b,0,bsz);
                    data.AddRange(b);
                }
            }
        }
    }
}