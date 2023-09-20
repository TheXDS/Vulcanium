/*
MIT License

Copyright (c) 2020 César Andrés Morgan

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
V U L C A N I U M - Y E L L O W S T O N E
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
using System.Collections.Concurrent;
using TheXDS.Vulcanium.Yellowstone.Base;

namespace TheXDS.Vulcanium.Yellowstone.Cards.Common;

/// <summary>
/// Provee de acceso simplificado y uniforme a un búffer de memoria que puede
/// ser utilizado como memoria de video directamente por instancias de 
/// <see cref="IGraphicsComponent"/>.
/// </summary>
public class SimpleVideoRam : IVideoRamPort
{
    private readonly byte[] _vram;
    private readonly ConcurrentBag<VideoRamChangeObserver> _obs = new();

    /// <summary>
    /// Inicializa una nueva instancia de la clase
    /// <see cref="SimpleVideoRam"/>.
    /// </summary>
    /// <param name="size">Tamaño de la memoria de video, en bytes.</param>
    public SimpleVideoRam(int size)
    {
        _vram = new byte[size];
    }

    int IVideoRamPort.VideoRamSize => _vram.Length;

    void IVideoRamPort.ExecuteWithFullAccess(VideoRamDirectAccessCallback callback)
    {
        lock (_vram) callback.Invoke(_vram);
    }

    void IVideoRamPort.NotifyBlockChange(int offset, int length)
    {
        byte[] block = ((IVideoRamPort)this).ReadBlock(offset, length);
        foreach (var j in _obs)
        {
            j.Invoke(block, offset);
        }
    }

    byte[] IVideoRamPort.ReadBlock(int offset, int length)
    {
        lock (_vram) return _vram[offset..(offset + length)];
    }

    void IVideoRamPort.SubscribeVideoRamChangeObserver(VideoRamChangeObserver observer)
    {
        _obs.Add(observer);
    }

    void IVideoRamPort.WriteBlock(byte[] buffer, int offset)
    {
        Array.Copy(buffer, 0, _vram, offset, buffer.Length);
        ((IVideoRamPort)this).NotifyBlockChange(offset, buffer.Length);
    }
}
