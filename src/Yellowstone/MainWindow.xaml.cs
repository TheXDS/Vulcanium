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

using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using TheXDS.Vulcanium.Yellowstone.Base;
using TheXDS.Vulcanium.Yellowstone.Cards.Common;
using TheXDS.Vulcanium.Yellowstone.Cards.LightMda;

namespace TheXDS.Vulcanium.Yellowstone;

/// <summary>
/// Define un método que se ejecuta al ocurrir un cambio en la memoria de video.
/// </summary>
/// <param name="buffer">Bloque que ha sufrido cambios.</param>
/// <param name="offset">Posición del bloque que ha sido escrito.</param>
public delegate void VideoRamChangeObserver(in byte[] buffer, in int offset);

/// <summary>
/// Define un método que permite acceso completo a la memoria de video.
/// </summary>
/// <param name="vram">Búffer de la memoria de video.</param>
public delegate void VideoRamDirectAccessCallback(byte[] vram);



//    /// <summary>
//    /// Lógica de modo texto para un adaptador de video compatible con CGA.
//    /// </summary>
//    public class CgaTextMode : GraphicsComponent, ITextLogic
//    {
//        /// <summary>
//        /// Describe un Nibble con las propiedades de un byte de atributos CGA.
//        /// </summary>
//        public interface ICgaColorNibble
//        {
//            /// <summary>
//            /// Obtiene o establece el color de este nibble.
//            /// </summary>
//            CgaColor Color { get; set; }

//            /// <summary>
//            /// Obtiene o establece el bit de atributo adicional de este nibble.
//            /// </summary>
//            bool Bit { get; set; }

//            /// <summary>
//            /// Convierte este Nibble a un byte completo.
//            /// </summary>
//            /// <returns>
//            /// Un <see cref="byte"/> con el contenido de este nibble.
//            /// </returns>
//            byte ToNibble()
//            {
//                return (byte)((byte)Color | (byte)(Bit ? 8 : 0));
//            }
//        }

//        /// <summary>
//        /// Nibble que describe el color principal.
//        /// </summary>
//        public struct CgaForegroundColor : ICgaColorNibble
//        {
//            public CgaColor Color { get; set; }

//            /// <summary>
//            /// Obtiene o establece el atributo de color de alta intensidad.
//            /// </summary>
//            public bool Bright { get; set; }

//            bool ICgaColorNibble.Bit
//            {
//                get => Bright;
//                set => Bright = value;
//            }
//        }

//        /// <summary>
//        /// Nibble que describe el colro de fondo.
//        /// </summary>
//        public struct CgaBackgroundColor : ICgaColorNibble
//        {
//            public CgaColor Color { get; set; }

//            /// <summary>
//            /// Obtiene o establece el bit de blinker (pestañeo).
//            /// </summary>
//            public bool Blink { get; set; }

//            bool ICgaColorNibble.Bit
//            {
//                get => Blink;
//                set => Blink = value;
//            }
//        }


//        private int cursorOffset;

//        public int CursorOffset
//        {
//            get => cursorOffset;
//            set
//            {
//                if (value > (Width * Height))
//                {
//                    Scroll((ushort)((value / Width) + 1));
//                    cursorOffset = (Width * (Height - 1)) + (value % Width);
//                }
//                else
//                {
//                    cursorOffset = value;
//                }
//            }
//        }

//        public void Clear()
//        {
//            var attr = EncodeAttrByte();
//            for (var j = 0; j < Adapter.VRAM.Length; j += 2)
//            {
//                Adapter.VRAM[j] = 32;
//                Adapter.VRAM[j + 1] = attr;
//            }
//        }

//        public CgaForegroundColor Foreground { get; set; } = new CgaForegroundColor { Color = CgaColor.White };

//        public CgaBackgroundColor Background { get; set; }

//        public ushort Width { get; set; } = 80;

//        public ushort Height { get; set; } = 25;

//        public void Print(string text)
//        {
//            var arr = String2Bytes(text).ToArray();
//            lock (Adapter.VRAM.SyncRoot) Array.Copy(arr, 0, Adapter.VRAM, CursorOffset, arr.Length);
//            CursorOffset += arr.Length;
//        }

//        public void Scroll(ushort lines)
//        {
//            if (lines < 1 || lines > Height) throw new ArgumentOutOfRangeException(nameof(lines));
//            var delta = lines * Width;
//            var attr = EncodeAttrByte();
//            lock (Adapter.VRAM) for (var row = 0; row < delta; row++)
//                {
//                    Adapter.VRAM[row] = Adapter.VRAM[row + delta];
//                    Adapter.VRAM[row + delta] = row % 2 == 0 ? (byte)32 : attr;
//                }
//        }

//        private IEnumerable<byte> String2Bytes(string text)
//        {
//            var attr = EncodeAttrByte();
//            foreach (var j in text.ToCharArray())
//            {
//                yield return (byte)(j & byte.MaxValue);
//                yield return attr;
//            }
//        }

//        private byte EncodeAttrByte()
//        {
//            return (byte)(((ICgaColorNibble)Foreground).ToNibble() | ((ICgaColorNibble)Background).ToNibble() << 4);
//        }
//    }

//    public class BitmapCgaTextModeRaster : GraphicsComponent, IRaster<WriteableBitmap>
//    {
//        private bool _blink;
//        private bool _vrr;
//        private byte _refreshRate = 15;
//        private readonly System.Timers.Timer _blinkTimer = new System.Timers.Timer(250);
//        private readonly System.Timers.Timer _refresh = new System.Timers.Timer(1000 / 15);

//        public BitmapCgaTextModeRaster(DispatcherObject parent)
//        {
//            _blinkTimer.Elapsed += (sender, e) => _blink = !_blink;
//            _refresh.Elapsed += (sender, e) => parent.Dispatcher.Invoke(DoRefresh);
//            _blinkTimer.Start();
//            _refresh.Start();
//        }

//        /// <summary>
//        /// Habilita o deshabilita el Tweak especial de IBM para el color café
//        /// (color 0x06) de la paleta CGA directamente en el Raster.
//        /// </summary>
//        public bool BrownTweak { get; set; } = true;

//        /// <summary>
//        /// Tasa de refresco de la pantalla (Cuadros por segundo)
//        /// </summary>
//        public byte RefreshRate
//        {
//            get => _vrr ? (byte)0 : _refreshRate;
//            set
//            {
//                if (value == 0)
//                {
//                    DynamicRefresh = true;
//                }
//                else
//                {
//                    _refresh.Interval = 1000 / (_refreshRate = value);
//                    if (_vrr) DynamicRefresh = false;
//                }
//            }
//        }

//        /// <summary>
//        /// Habilita / deshabilita el modo de refresco dinámico variable (VRR).
//        /// </summary>
//        public bool DynamicRefresh
//        {
//            get
//            {
//                return _vrr;
//            }
//            set
//            {
//                if (_vrr = value) _refresh.Stop();
//                else _refresh.Start();
//            }
//        }

//        /// <summary>
//        /// Obtiene el objeto de salida del Raster.
//        /// </summary>
//        public WriteableBitmap ScreenOutput { get; }
//            = new WriteableBitmap(640, 400, 96, 96, PixelFormats.Bgr32, null);
//        //= new WriteableBitmap(640, 200, 96, 96, PixelFormats.Indexed8, BitmapPalettes.Halftone8);

//        private unsafe void DoRefresh()
//        {
//            ScreenOutput.Lock();
//            var vmem = Adapter.VRAM;
//            for (var j = 0; j < 4000; j += 2)
//            {
//                var bmp = GetCharBitmap(vmem[j], vmem);
//                var (fore, bacg) = DecodeVgaAttribute(vmem[j + 1]);
//                for (var row = 0; row < 16; row++)
//                {
//                    for (var column = 0; column < 8; column++)
//                    {
//                        IntPtr pBackBuffer = ScreenOutput.BackBuffer;
//                        var actualrow = row + j / 160 * 16;
//                        var actualcol = column + (j % 160 * 4);

//                        pBackBuffer += actualrow * ScreenOutput.BackBufferStride;
//                        pBackBuffer += actualcol * 4;

//                        *(int*)pBackBuffer = ((bmp[row] >> column) & 1) == 1 ? fore : bacg;
//                    }
//                }
//            }
//            ScreenOutput.AddDirtyRect(new Int32Rect(0, 0, 640, 400));
//            ScreenOutput.Unlock();
//        }

//        public void Refresh()
//        {
//            if (DynamicRefresh) DoRefresh();
//        }

//        public void Refresh(IEnumerable<(int index, int size)> vramBlocks)
//        {
//        }

//        private (int fore, int bacg) DecodeVgaAttribute(in byte attribute)
//        {
//#if CgaAttrLogicDecode
//            //Decodificación lógica
//            var fore = (attribute & 4) == 4 ? 0x00808080 : 0;

//            if ((attribute >> 2 & 1) == 1) fore |= 0x007F0000;
//            if ((attribute >> 1 & 1) == 1) fore |= 0x00007F00;
//            if ((attribute & 1) == 1) fore |= 0x0000007F;

//            var bacg = (attribute & 8) == 8 ? 0x00808080 : 0;
//            if ((attribute >> 6 & 1) == 1) bacg |= 0x007F0000;
//            if ((attribute >> 5 & 1) == 1) bacg |= 0x00007F00;
//            if ((attribute >> 4 & 1) == 1) bacg |= 0x0000007F;

//            return (fore, bacg);
//#elif CgaAttrSwitchDecode
//            // Decodificación de bloque switch
//            int Map(int color)
//            {
//                return color switch
//                {
//                    1 => 0x0000aa,
//                    2 => 0x00aa00,
//                    3 => 0x00aaaa,
//                    4 => 0xaa0000,
//                    5 => 0xaa00aa,
//                    6 => BrownTweak ? 0xaa5500 : 0xaaaa00,
//                    7 => 0xaaaaaa,
//                    8 => 0x555555,
//                    9 => 0x5555ff,
//                    10 => 0x55ff55,
//                    11 => 0x55ffff,
//                    12 => 0xff5555,
//                    13 => 0xff55ff,
//                    14 => 0xffff55,
//                    15 => 0xffffff,
//                    _ => 0,
//                };
//            }
//            var bg = Map((attribute & 0x70) >> 4);
//            return ((_blink && ((attribute & 8) == 8)) ? bg : Map(attribute & 0x0f), bg);
//#elif CgaAttrMathDecode
//            //Decodificación aritmética
//            int Calc(byte color, byte bit) => (int)((2f / 3f * (color & bit) / bit + (1f / 3f) * (color & 8) / 8) * 255) << ((bit-1)*8);
//            var fnibble = (byte)(attribute & 0x0f);
//            var bnibble = (byte)((attribute & 0xf0) >> 4);
//            var fore = Calc(fnibble, 1) | Calc(fnibble, 2) | Calc(fnibble, 3);
//            var bacg = Calc(bnibble, 1) | Calc(bnibble, 2) | Calc(bnibble, 3);
//            return (fore, bacg);
//#endif
//        }

//        private byte[] GetCharBitmap(in byte @char, byte[] vmem)
//        {
//            var retval = new byte[16];
//            Array.Copy(vmem, 258048 + (@char * 16), retval, 0, 16);
//            return retval;
//        }

//        bool IRaster.RequiresManualUpdate => DynamicRefresh;
//    }














//    public abstract class VirtualVgaScreenMode
//    {
//        protected VirtualVgaScreenMode(ScreenModeData data)
//        {
//            ModeData = data;
//        }

//        public ScreenModeData ModeData { get; }

//        public virtual void Clear(byte[] vram)
//        {
//            Array.Clear(vram, 0, vram.Length);
//        }

//        protected internal abstract void OnRaster(WriteableBitmap raster, byte[] vram);
//        protected virtual void InitVram(byte[] vram) => Clear(vram);
//    }

//    /* Info:
//     * =====
//     * Para todos los modos de video, el consumo máximo recomendado es de 
//     * 256,000 bytes. Los 6144 bytes restantes se recomiendan utilizar de
//     * la siguiente manera:
//     * - hasta 2048 bytes para paletas de color u otra información 
//     *   personalizada.
//     * - 4096 Bytes para los mapas de bits de caracteres.
//     */
//    public class TextMode : VirtualVgaScreenMode
//    {
//        private int _cursorOffset;


//        public ushort CursorRow { get; set; }
//        public ushort CursorColumn { get; set; }
//        public ushort CursorShape { get; set; }

//        public ushort TextRows => (ushort)(ModeData.Width / 8);


//        public TextMode() : base(ScreenModeData.TrueColor)
//        {
//        }

//        protected internal override void OnRaster(WriteableBitmap raster, byte[] vmem)
//        {
//            unsafe
//            {
//                for (var j = 0; j < 4000; j += 2)
//                {
//                    var bmp = GetCharBitmap(vmem[j], vmem);
//                    var (fore, bacg) = DecodeVgaAttribute(vmem[j + 1]);
//                    for (var row = 0; row < 16; row++)
//                    {
//                        for (var column = 0; column < 8; column++)
//                        {
//                            IntPtr pBackBuffer = raster.BackBuffer;
//                            var actualrow = row + j / 160 * 16;
//                            var actualcol = column + (j % 160 * 4);

//                            pBackBuffer += actualrow * raster.BackBufferStride;
//                            pBackBuffer += actualcol * 4;

//                            *(int*)pBackBuffer = ((bmp[row] >> column) & 1) == 1 ? fore : bacg;
//                        }
//                    }
//                }
//            }
//        }

//        private (int fore, int bacg) DecodeVgaAttribute(in byte attribute)
//        {
//            var fore = (attribute >> 3 & 1) == 1 ? 0x00808080 : 0;
//            if ((attribute >> 2 & 1) == 1) fore |= 0x007F0000;
//            if ((attribute >> 1 & 1) == 1) fore |= 0x00007F00;
//            if ((attribute & 1) == 1) fore |= 0x0000007F;

//            var bacg = (attribute >> 7 & 1) == 1 ? 0x00808080 : 0;
//            if ((attribute >> 6 & 1) == 1) bacg |= 0x007F0000;
//            if ((attribute >> 5 & 1) == 1) bacg |= 0x00007F00;
//            if ((attribute >> 4 & 1) == 1) bacg |= 0x0000007F;

//            return (fore, bacg);
//        }

//        private byte[] GetCharBitmap(in byte @char, byte[] vmem)
//        {
//            var retval = new byte[16];
//            Array.Copy(vmem, 258048 + (@char * 16), retval, 0, 16);
//            return retval;
//        }
//    }



 


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        InitializeComponent();
        var logic = InitDisplayAdapter(imgTest);

        for (ushort j = 1; j <= 25; j++)
        {
            logic.Locate(j, 1);
            logic.Print($"Hello world! {j}");
        }
        logic.Scroll(5);
        logic.Locate(21, 1);
        logic.Print("Programar es mi pasión :D (ok, la pantalla no aguanta Unicode y está en ASCII - CP437) ... A continuación un patrón de muestra.");
    }

    private ITextLogic InitDisplayAdapter(System.Windows.Controls.Image screen)
    {
        IVideoRamPort ram = new SimpleVideoRam(2048);
        IRaster<WriteableBitmap> mda;
        ITextLogic logic = new LightMdaLogic(ram);
        using (var fs = new FileStream(@".\Resources\CP437.chr", FileMode.Open))
        {
            mda = new LightMdaRaster(ram, fs);
        }
        screen.Source = mda.ScreenOutput;
        return logic;
    }
}
