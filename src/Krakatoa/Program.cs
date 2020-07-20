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
using System.Runtime.InteropServices;

namespace TheXDS.Vulcanium.Krakatoa
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            /* Si, ya sé... esto NO es SOLID en lo absoluto */

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
            var cw = PInvoke.GetConsoleWindow();
            if (cw == IntPtr.Zero)
            { 
                Console.WriteLine("Neles. No se pudo obtener la consola.");
                return;
            }
            
            var nfc = false;
            if (args.Contains("transparent"))
            {
                cw.MakeTransparent();
                Console.WriteLine("Ventana transparente");
            }
            if (args.Contains("blur"))
            {
                cw.EnableBlur();
                Console.WriteLine("Blur habilitado");
            }
            if (args.Contains("acrylic"))
            {
                cw.EnableAcrylic();
                Console.Write("Acrílico habilitado");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("( /!\\ BOGUS!!!)");
                Console.ResetColor();
            }
            if (args.Contains("noclose")) 
            {
                nfc = true;
                cw.HideClose();
                Console.WriteLine("Sin botón de cerrar");
            }
            if (args.Contains("nomax"))
            { 
                nfc = true;
                cw.HideMaximize();
                Console.WriteLine("Sin botón de maximizar");
            }
            if (args.Contains("nomin"))
            { 
                nfc = true;
                cw.HideMinimize();
                Console.WriteLine("Sin botón de minimizar");
            }
            if (args.Contains("nocaption")) 
            {
                nfc = true;
                cw.HideCaption();
                Console.WriteLine("Sin caption");
            }
            if (args.Contains("noborder"))
            { 
                nfc = true;
                cw.HideBorder();
                Console.WriteLine("Sin borde");
            }

            if (nfc) cw.NotifyWindowFrameChange();
            Console.ReadLine();
        }
    }

    internal static class PInvoke
    {
        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("dwmapi.dll")]
        internal static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMargins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        internal static extern bool DwmIsCompositionEnabled();

        [DllImport("user32.dll")]
        internal static extern uint GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hwnd, int index, uint newStyle);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int width, int height, uint flags);
    }

    internal static class Dwm
    {
        public static bool IsCompositionEnabled()
        {
            return PInvoke.DwmIsCompositionEnabled();
        }
        public static void DisableEffects(this IntPtr window)
        {
            SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_DISABLED });
        }
        public static void SetClientPadding(this IntPtr window, Margins padding)
        {
            SetFramePadding(window, padding);
        }
        public static void SetFramePadding(this IntPtr window, Margins padding)
        {
            if (IsCompositionEnabled())
            {
                PInvoke.DwmExtendFrameIntoClientArea(window, ref padding);
            }
        }
        public static void EnableBlur(this IntPtr window)
        {
            SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND });
        }
        public static void HideCaption(this IntPtr window)
        {
            HideGwlStyle(window, WindowStyles.WS_CAPTION);
        }
        public static void HideBorder(this IntPtr window)
        {
            HideGwlStyle(window, WindowStyles.WS_BORDER);
        }
        public static void EnableAcrylic(this IntPtr window)
        {
            SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND });
        }
        public static void MakeTransparent(this IntPtr window)
        {
            SetWindowData(window, WindowData.GWL_EXSTYLE, p => p | WindowStyles.WS_EX_TRANSPARENT);
        }
        public static void HideClose(this IntPtr window)
        {
            HideGwlStyle(window, WindowStyles.WS_SYSMENU);
        }
        public static void HideMaximize(this IntPtr window)
        {
            HideGwlStyle(window, WindowStyles.WS_MAXIMIZEBOX);
        }
        public static void HideMinimize(this IntPtr window)
        {
            HideGwlStyle(window, WindowStyles.WS_MINIMIZEBOX);
        }
        public static void NotifyWindowFrameChange(this IntPtr window)
        {
            PInvoke.SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0,
                (uint)(WindowChanges.SWP_NOMOVE |
                WindowChanges.SWP_NOSIZE |
                WindowChanges.SWP_NOZORDER |
                WindowChanges.SWP_FRAMECHANGED));
        }
        private static void SetWindowEffect(IntPtr window, AccentPolicy accent)
        {
            var accentStructSize = Marshal.SizeOf(accent);
            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);
            var data = new WindowCompositionAttributeData
            {
                Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY,
                SizeOfData = accentStructSize,
                Data = accentPtr
            };
            PInvoke.SetWindowCompositionAttribute(window, ref data);
            Marshal.FreeHGlobal(accentPtr);
        }
        private static void HideGwlStyle(this IntPtr window, WindowStyles bit)
        {
            SetWindowData(window, WindowData.GWL_STYLE, p => p & ~bit);
        }
        private static void SetWindowData(this IntPtr window, WindowData data, Func<WindowStyles, WindowStyles> transform)
        {
            PInvoke.SetWindowLong(window, (int)data, (uint)transform((WindowStyles)PInvoke.GetWindowLong(window, (int)data)));
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_ENABLE_ACRYLICBLURBEHIND = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Margins
    {
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }

    internal enum SysCommand
    {
        SC_CONTEXTHELP = 0xF180
    }

    [Flags]
    internal enum WindowChanges : uint
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_FRAMECHANGED = 0x0020
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowData
    {
        GWL_WNDPROC = -4,
        GWL_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20,
        GWL_USERDATA = -21
    }

    [Flags]
    internal enum WindowStyles : uint
    {
        WS_VSCROLL = 0x00200000u,
        WS_VISIBLE = 0x10000000u,
        WS_TILEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_TILED = 0x00000000u,
        WS_TABSTOP = 0x00010000u,
        WS_SYSMENU = 0x00080000u,
        WS_SIZEBOX = 0x00040000u,
        WS_THICKFRAME = 0x00040000u,
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        WS_OVERLAPPEDWINDOW =
            WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUP = 0x80000000u,
        WS_OVERLAPPED = 0x00000000u,
        WS_MINIMIZE = 0x20000000u,
        WS_MAXIMIZE = 0x01000000u,
        WS_ICONIC = 0x20000000u,
        WS_HSCROLL = 0x00100000u,
        WS_GROUP = 0x00020000u,
        WS_DLGFRAME = 0x00400000u,
        WS_DISABLED = 0x08000000u,
        WS_CLIPSIBLINGS = 0x04000000u,
        WS_CLIPCHILDREN = 0x02000000u,
        WS_CHILDWINDOW = 0x40000000u,
        WS_CHILD = 0x40000000u,
        WS_CAPTION = 0x00C00000u,
        WS_BORDER = 0x00800000u,
        WS_MINIMIZEBOX = 0x00020000u,
        WS_MAXIMIZEBOX = 0x00010000u,

        WS_EX_ACCEPTFILES = 0x00000010u,
        WS_EX_APPWINDOW = 0x00040000u,
        WS_EX_CLIENTEDGE = 0x00000200u,
        WS_EX_COMPOSITED = 0x02000000u,
        WS_EX_CONTEXTHELP = 0x00000400u,
        WS_EX_CONTROLPARENT = 0x00010000u,
        WS_EX_DLGMODALFRAME = 0x00000001u,
        WS_EX_LAYERED = 0x00080000u,
        WS_EX_LAYOUTRTL = 0x00400000u,
        WS_EX_LEFT = 0x00000000u,
        WS_EX_LEFTSCROLLBAR = 0x00004000u,
        WS_EX_LTRREADING = 0x00000000u,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_NOACTIVATE = 0x08000000,
        WS_EX_NOINHERITLAYOUT = 0x00100000u,
        WS_EX_NOPARENTNOTIFY = 0x00000004u,
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000u,
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
        WS_EX_RIGHT = 0x00001000u,
        WS_EX_RIGHTSCROLLBAR = 0x00000000u,
        WS_EX_RTLREADING = 0x00002000u,
        WS_EX_STATICEDGE = 0x00020000u,
        WS_EX_TOOLWINDOW = 0x00000080u,
        WS_EX_TOPMOST = 0x00000008u,
        WS_EX_TRANSPARENT = 0x00000020u,
        WS_EX_WINDOWEDGE = 0x00000100u
    }
}
