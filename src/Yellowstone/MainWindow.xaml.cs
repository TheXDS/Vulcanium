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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheXDS.Vulcanium.Yellowstone
{
    public sealed class VirtualVga
    {
        private List<VirtualVgaScreenMode> modes = new List<VirtualVgaScreenMode>();

        /* Info:
         * =====
         * Para todos los modos de video, el consumo máximo recomendado es de 
         * 256,000 bytes. Los 6144 bytes restantes se recomiendan utilizar de
         * la siguiente manera:
         * - hasta 2048 bytes para paletas de color u otra información 
         *   personalizada.
         * - 4096 Bytes para los mapas de bits de caracteres.
         */
        public byte[] vmem = new byte[262144];

        public WriteableBitmap Output { get; } = new WriteableBitmap(640, 400, 96, 96, PixelFormats.Bgr32, null);

        public VirtualVgaScreenMode CurrentMode { get; set; }

        public void Raster()
        {
            try
            {
                Output.Lock();
                CurrentMode.Raster(Output, vmem);
                Output.AddDirtyRect(new Int32Rect(0,0, 640, 400));
            }
            finally
            {
                Output.Unlock();
            }
        }
    }

    public abstract class VirtualVgaScreenMode
    {
        public abstract void Raster(WriteableBitmap raster, byte[] vmem);
    }

    public class TextMode : VirtualVgaScreenMode
    {
        public override void Raster(WriteableBitmap raster, byte[] vmem)
        {
            unsafe
            {
                //IntPtr pBackBuffer = raster.BackBuffer;

                for (var j = 0; j < 4000; j+=2)
                {
                    var bmp = GetCharBitmap(vmem[j], vmem);
                    var (fore, bacg) = DecodeVgaAttribute(vmem[j + 1]);
                    for (var row = 0; row < 16; row++)
                    {
                        for (var column = 0; column < 8; column++)
                        {
                            IntPtr pBackBuffer = raster.BackBuffer;
                            var actualrow = row + j / 160 * 16;
                            var actualcol = column + (j % 160 * 4);

                            pBackBuffer += actualrow * raster.BackBufferStride;
                            pBackBuffer += (actualcol) * 4;

                            *((int*)pBackBuffer) = ((bmp[row] >> column) & 1) == 1 ? fore : bacg;
                        }
                    }
                }
            }
        }
        private (int fore, int bacg) DecodeVgaAttribute(in byte attribute)
        {
            var fore = (attribute >> 3 & 1) == 1 ? 0x00808080 : 0;
            if ((attribute >> 2 & 1) == 1) fore |= 0x007F0000;
            if ((attribute >> 1 & 1) == 1) fore |= 0x00007F00;
            if ((attribute & 1) == 1) fore |= 0x0000007F;

            var bacg = (attribute >> 7 & 1) == 1 ? 0x00808080 : 0;
            if ((attribute >> 6 & 1) == 1) bacg |= 0x007F0000;
            if ((attribute >> 5 & 1) == 1) bacg |= 0x00007F00;
            if ((attribute >> 4 & 1) == 1) bacg |= 0x0000007F;

            return (fore, bacg);
        }

        private byte[] GetCharBitmap(in byte @char, byte[] vmem)
        {
            var retval = new byte[16];
            Array.Copy(vmem, 258048 + (@char * 16), retval, 0, 16);
            return retval;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VirtualVga Vga = new VirtualVga();
        public MainWindow()
        {
            InitializeComponent();
            Vga.CurrentMode = new TextMode();

            var i = new Image();
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(i, EdgeMode.Aliased);

            var btn = new Button
            {
                Content = "Raster",
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            btn.Click += (sender, e) =>
            {
                var abmp = new byte[] {
                0b00000000,
                0b00000000,
                0b00000000,
                0b00010000,
                0b00111000,
                0b01101100,
                0b11000110,
                0b11000110,
                0b11111110,
                0b11000110,
                0b11000110,
                0b11000110,
                0b11000110,
                0b00000000,
                0b00000000,
                0b00000000 };

                
                for (var j = 0; j < 256; j++)
                Array.Copy(abmp, 0, Vga.vmem, 258048 + (j * 16), 16);
                Vga.vmem[0] = 65;
                Vga.vmem[1] = 7;
                Vga.Raster();




                //var r = new Random();
                //for (var j = 258048; j < 262144; j++) //258048
                //    Vga.vmem[j] = (byte)r.Next(0, 256);

                //Vga.vmem[0] = 64;
                //Vga.vmem[1] = 7;
                //Vga.Raster();
            };
            Content = new Grid
            {
                Children =
                {
                    i, btn
                }
            };
            i.Source = Vga.Output;

            i.Stretch = Stretch.None;
            i.HorizontalAlignment = HorizontalAlignment.Left;
            i.VerticalAlignment = VerticalAlignment.Top;
        }
    }
}
