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

namespace TheXDS.Vulcanium.Yellowstone.Base;

/// <summary>
/// Define un puerto que permite la lectura y escritura de bloques de
/// memoria de video.
/// </summary>
public interface IVideoRamPort
{
    /// <summary>
    /// Obtiene el tamaño en bytes de la memoria de video.
    /// </summary>
    int VideoRamSize { get; }

    /// <summary>
    /// Lee un bloque de memoria de video.
    /// </summary>
    /// <param name="offset">Posición desde la cual leer.</param>
    /// <param name="length">Tamaño del bloque a leer.</param>
    /// <returns>El bloque de memoria de video solicitado.</returns>
    byte[] ReadBlock(int offset, int length);

    /// <summary>
    /// Escribe un bloque de memoria de video.
    /// </summary>
    /// <param name="buffer">Búfer con el contenido a escribir.</param>
    /// <param name="offset">
    /// Posición en la cual iniciar la escritura del bloque de memoria.
    /// </param>
    void WriteBlock(byte[] buffer, int offset);

    /// <summary>
    /// Suscribe un método que será llamado cada vez que ocurra un cambio
    /// en la memoria de video.
    /// </summary>
    /// <param name="observer">
    /// Método a llamar cuando ocurra un cambio en la memoria de video.
    /// </param>
    void SubscribeVideoRamChangeObserver(VideoRamChangeObserver observer);

    /// <summary>
    /// Ejecuta una operación con acceso exclusivo a la totalidad de la
    /// memoria de video.
    /// </summary>
    /// <param name="callback">
    /// Método a ejecutar al obtener acceso completo a la memoria de video.
    /// </param>
    /// <remarks>
    /// Será necesario que el método <paramref name="callback"/> ejecute
    /// manualmente el refresco de cualquier área de memoria que haya sido
    /// modificada, mediante llamadas al método
    /// <see cref="NotifyBlockChange(int, int)"/>.
    /// </remarks>
    void ExecuteWithFullAccess(VideoRamDirectAccessCallback callback);

    /// <summary>
    /// Notifica manualmente del cambio de una región de memoria.
    /// </summary>
    /// <param name="offset">
    /// Posición del bloque de memoria RAM que ha cambiado.
    /// </param>
    /// <param name="length">
    /// Tamaño del bloque de memoria RAM que ha cambiado.
    /// </param>
    void NotifyBlockChange(int offset, int length);

    /// <summary>
    /// Notifica a los suscriptores de memoria que la totalidad de la RAM ha cambiado.
    /// </summary>
    void RefreshAll() => NotifyBlockChange(0, VideoRamSize);

    /// <summary>
    /// Restablece la totalidad de la memoria de video.
    /// </summary>
    void Reset()
    {
        ExecuteWithFullAccess(Array.Clear);
        RefreshAll();
    }
}
