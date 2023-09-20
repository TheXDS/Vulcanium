﻿/*
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

namespace TheXDS.Vulcanium.Krakatoa
{
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
