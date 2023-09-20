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
using System.Runtime.InteropServices;

namespace TheXDS.Vulcanium.Krakatoa
{
    internal static partial class Dwm
    {
        public static bool IsCompositionEnabled()
        {
            return PInvoke.DwmIsCompositionEnabled();
        }
        public static bool DisableEffects(this IntPtr window)
        {
            return SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_DISABLED });
        }
        public static bool SetClientPadding(this IntPtr window, Margins padding)
        {
            return SetFramePadding(window, padding);
        }
        public static bool SetFramePadding(this IntPtr window, Margins padding)
        {
            return IsCompositionEnabled() && (PInvoke.DwmExtendFrameIntoClientArea(window, ref padding) & 0xffff) == 0;
        }
        public static bool EnableBlur(this IntPtr window)
        {
            return SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND });
        }
        public static bool HideCaption(this IntPtr window)
        {
            return HideGwlStyle(window, WindowStyles.WS_CAPTION);
        }
        public static bool HideBorder(this IntPtr window)
        {
            return HideGwlStyle(window, WindowStyles.WS_BORDER);
        }
        public static bool EnableAcrylic(this IntPtr window)
        {
            return SetWindowEffect(window, new AccentPolicy { AccentState = AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND });
        }
        public static bool MakeTransparent(this IntPtr window)
        {
            return SetWindowData(window, WindowData.GWL_EXSTYLE, p => p | WindowStyles.WS_EX_TRANSPARENT);
        }
        public static bool HideClose(this IntPtr window)
        {
            return HideGwlStyle(window, WindowStyles.WS_SYSMENU);
        }
        public static bool HideMaximize(this IntPtr window)
        {
            return HideGwlStyle(window, WindowStyles.WS_MAXIMIZEBOX);
        }
        public static bool HideMinimize(this IntPtr window)
        {
            return HideGwlStyle(window, WindowStyles.WS_MINIMIZEBOX);
        }
        public static bool NotifyWindowFrameChange(this IntPtr window)
        {
            return PInvoke.SetWindowPos(window, IntPtr.Zero, 0, 0, 0, 0,
                (uint)(WindowChanges.SWP_NOMOVE |
                WindowChanges.SWP_NOSIZE |
                WindowChanges.SWP_NOZORDER |
                WindowChanges.SWP_FRAMECHANGED));
        }
        private static bool SetWindowEffect(IntPtr window, AccentPolicy accent)
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
            var retVal = (PInvoke.SetWindowCompositionAttribute(window, ref data) & 0xffff) == 0;
            Marshal.FreeHGlobal(accentPtr);
            return retVal;
        }
        private static bool HideGwlStyle(this IntPtr window, WindowStyles bit)
        {
            return SetWindowData(window, WindowData.GWL_STYLE, p => p & ~bit);
        }
        private static bool SetWindowData(this IntPtr window, WindowData data, Func<WindowStyles, WindowStyles> transform)
        {
            var r = PInvoke.SetWindowLong(window, (int)data, (uint)transform((WindowStyles)PInvoke.GetWindowLong(window, (int)data)));
            return (r & 0xffff) == 0;
        }

    }
}
