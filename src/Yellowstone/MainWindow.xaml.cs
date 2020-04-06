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
using System.IO;
using System.IO.Compression;
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
        public byte[] _vram;
        public WriteableBitmap _raster;
        private VirtualVgaScreenMode currentMode;

        public VirtualVga(int vRamSize)
        {
            if (vRamSize < 1) throw new ArgumentOutOfRangeException(nameof(vRamSize));
            _vram = new byte[vRamSize];
            CurrentMode = new TextMode();
        }

        public VirtualVga() : this(262144) { }

        public VirtualVgaScreenMode CurrentMode
        {
            get => currentMode;
            set
            {
                currentMode = value;
                _raster = value.ModeData.CreateSurface();
            }
        }






        public void Raster()
        {
            CurrentMode.Raster(_raster, _vram);
        }
    }

    public abstract class VirtualVgaScreenMode
    {
        protected VirtualVgaScreenMode(ScreenModeData data)
        {
            ModeData = data;
        }

        public ScreenModeData ModeData { get; }

        public virtual void Clear(byte[] vram)
        {
            for (var j = 0; j < vram.Length; j++)
            {
                vram[j] = 0;
            }
        }

        protected abstract void OnRaster(WriteableBitmap raster, byte[] vmem);

        internal void Raster(WriteableBitmap raster, byte[] vmem)
        {
            try
            {
                raster.Lock();
                OnRaster(raster, vmem);
                raster.AddDirtyRect(new Int32Rect(0, 0, ModeData.Width, ModeData.Height));
            }
            finally
            {
                raster.Unlock();
            }
        }
    }

    /* Info:
     * =====
     * Para todos los modos de video, el consumo máximo recomendado es de 
     * 256,000 bytes. Los 6144 bytes restantes se recomiendan utilizar de
     * la siguiente manera:
     * - hasta 2048 bytes para paletas de color u otra información 
     *   personalizada.
     * - 4096 Bytes para los mapas de bits de caracteres.
     */
    public class TextMode : VirtualVgaScreenMode
    {
        public TextMode() : base(ScreenModeData.TrueColor)
        {
        }

        protected override void OnRaster(WriteableBitmap raster, byte[] vmem)
        {
            unsafe
            {
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
                            pBackBuffer += actualcol * 4;

                            *(int*)pBackBuffer = ((bmp[row] >> column) & 1) == 1 ? fore : bacg;
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
            var btn2 = new Button
            {
                Content = "Parse",
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            btn.Click += (sender, e) =>
            {
                var rnd = new Random();
                using var fi = new FileStream(@"C:\Users\xds_x\source\repos\TheXDS\Vulcanium\src\Yellowstone\Resources\CP437.chr", FileMode.Open);
                var bmp = new byte[4096];
                fi.Read(bmp, 0, 4096);
                Array.Copy(bmp, 0, Vga._vram, 258048, 4096);
                for (int j = 0; j < 256; j++)
                {
                    Vga._vram[j * 2] = (byte)j;
                    Vga._vram[(j * 2) + 1] = (byte)rnd.Next(0,256);
                }
                Vga.Raster();
            };

            btn2.Click += Convert;

            Content = new Grid
            {
                Children =
                {
                    i, btn2, btn
                }
            };
            i.Source = Vga._raster;

            i.Stretch = Stretch.None;
            i.HorizontalAlignment = HorizontalAlignment.Left;
            i.VerticalAlignment = VerticalAlignment.Top;
        }



        public void Convert(object sender, RoutedEventArgs e)
        {
            using var fi = new FileStream(@"C:\Users\xds_x\Downloads\Sin título.bmp", FileMode.Open);
            using var fo = new FileStream(@"C:\Users\xds_x\Downloads\CP850.bin",FileMode.Create);

            for (var i = 0; i < 4; i++)
            {
                var row = 0;
                var col = 0;
                byte[][] bytes = new byte[80][];

                for (col = 0; col < 80; col++)
                {
                    bytes[col] = new byte[16];
                }

                for (row = 0; row < 16; row++)
                {
                    for (col = 0; col < 80; col++)
                    {
                        bytes[col][row]=(byte)fi.ReadByte();
                    }
                }

                for (col = 0; col < 80; col++)
                {
                    fo.Write(bytes[col].Select(p => System.Convert.ToByte(new string(System.Convert.ToString(p, 2).PadLeft(8, '0').Reverse().ToArray()), 2)).ToArray(), 0, 16);
                }
            }
            fo.SetLength(4096);
            fo.Close();
        }
    }
}
