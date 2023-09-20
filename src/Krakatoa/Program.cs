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
V U L C A N I U M - K R A K A T O A
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
using System.Linq;

namespace TheXDS.Vulcanium.Krakatoa
{
    internal static class Program
    {
        private class Arg
        {
            private readonly Func<IntPtr, bool> argAction;
            private readonly string argDesc;
            private readonly string argName;
            private readonly bool nfc;

            public bool NotifyWindowFrameChange { get; private set; }

            public Arg(string argName, string argDesc, Func<IntPtr, bool> argAction, bool triggersNotifyWindowFrameChange = false)
            {
                this.argName = argName;
                this.argDesc = argDesc;
                this.argAction = argAction;
                nfc = triggersNotifyWindowFrameChange;
            }

            public void Run(string[] args, IntPtr consoleWindow)
            {
                if (args.Contains(argName))
                {
                    NotifyWindowFrameChange= nfc;
                    argAction.Invoke(consoleWindow);
                    Console.WriteLine($"{argDesc} [{(argAction.Invoke(consoleWindow) ? "OK":"FAIL")}]");
                }
            }
        }

        private static void Main(string[] args)
        {
            /* Si, ya sé... esto NO es SOLID */
            if (Environment.OSVersion.Platform != PlatformID.Win32NT)
            {
                Console.WriteLine(@"Este demo únicamente funciona en Microsoft Windows...    ¯\_(ツ)_/¯ ");
                Console.WriteLine("Here's an atomic explosion instead:");
                Console.WriteLine(@"
  _.----._
 (   (    )
(  (    )  )
 (________)
    ||||
  --++++--
    ||||
  .(    ).
 (_(____)_)");
                return;
            }
            Console.WriteLine(@"NOTA: Este demo no funcionará en Windows Terminal...    ¯\_(ツ)_/¯ ");
            var cw = PInvoke.GetConsoleWindow();
            if (cw == IntPtr.Zero)
            { 
                Console.WriteLine("Neles. No se pudo obtener la consola.");
                return;
            }

            var x = new[] { 
                new Arg("transparent", "Ventana transparente", Dwm.MakeTransparent),
                new Arg("blur", "Blur habilitado", Dwm.EnableBlur),
                new Arg("acrylic", "Acrílico habilitado ( /!\\ BOGUS!!!)", Dwm.EnableAcrylic),
                new Arg("noclose", "Sin botón de cerrar", Dwm.HideClose, true),
                new Arg("nomax", "Sin botón de maximizar", Dwm.HideMaximize, true),
                new Arg("nomin", "Sin botón de minimizar", Dwm.HideMinimize, true),
                new Arg("nocaption", "Sin caption", Dwm.HideCaption, true),
                new Arg("noborder", "Sin borde", Dwm.HideBorder, true),
            };
            foreach (var j in x) j.Run(args, cw); 
            if (x.Any(p => p.NotifyWindowFrameChange)) cw.NotifyWindowFrameChange();
            Console.ReadLine();
        }
    }
}
