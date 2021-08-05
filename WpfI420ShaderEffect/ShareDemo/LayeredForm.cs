using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareDemo
{

    public class LayeredForm : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int Width;
            public int Height;

            public SIZE(int width, int height)
            {
                Width = width;
                Height = height;
            }
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;

            public BLENDFUNCTION(
                byte op, byte flags, byte alpha, byte format)
            {
                BlendOp = op;
                BlendFlags = flags;
                SourceConstantAlpha = alpha;
                AlphaFormat = format;
            }
        }

        public const int CS_NOCLOSE = 0x0200;
        public const int WS_EX_LAYERED = 0x00080000;
        public const byte AC_SRC_OVER = 0x00;
        public const byte AC_SRC_ALPHA = 0x01;
        public const Int32 ULW_ALPHA = 2;

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr handle);
        [DllImport("gdi32.dll")]
        internal static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll", ExactSpelling = true, PreserveSig = true)]
        internal static extern IntPtr SelectObject(IntPtr hdc, IntPtr hgdiobj);
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);
        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr handle, IntPtr hdc);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteDC(IntPtr hdc);

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams parameters = base.CreateParams;
                parameters.ClassStyle |= CS_NOCLOSE;
                parameters.ExStyle |= WS_EX_LAYERED;
                return parameters;
            }
        }

        public LayeredForm()
        {
            UpdateLayeredWindow();
        }

        public void UpdateLayeredWindow()
        {
            using var bitmap = new Bitmap(Width, Height);
            using var graphics = Graphics.FromImage(bitmap);
            OnUpdateLayeredPaint(new PaintEventArgs(graphics, this.ClientRectangle));
            SetBits(bitmap);
        }

        protected virtual void OnUpdateLayeredPaint(PaintEventArgs e)
        {

        }

        private void SetBits(Bitmap bmp)
        {
            IntPtr oldBits = IntPtr.Zero;
            IntPtr screenDC = GetDC(IntPtr.Zero);
            IntPtr hBitmap = IntPtr.Zero;
            IntPtr memDc = CreateCompatibleDC(screenDC); //内存句柄
            try
            {
                POINT topLoc = new POINT(this.Left, this.Top);
                SIZE bitMapSize = new SIZE(bmp.Width, bmp.Height);
                BLENDFUNCTION blendFunc = new BLENDFUNCTION();
                POINT srcLoc = new POINT(0, 0);
                hBitmap = bmp.GetHbitmap(Color.FromArgb(0)); // 为传入的PNG图片设置一个背景
                oldBits = SelectObject(memDc, hBitmap); //将图片写入内存
                blendFunc.BlendOp = AC_SRC_OVER;
                blendFunc.SourceConstantAlpha = 255;
                blendFunc.AlphaFormat = AC_SRC_ALPHA;
                blendFunc.BlendFlags = 0;

                if (!IsDisposed)
                    UpdateLayeredWindow(Handle, screenDC, ref topLoc, ref bitMapSize, memDc, ref srcLoc, 0, ref blendFunc, ULW_ALPHA);
            }
            finally
            {
                ReleaseDC(IntPtr.Zero, screenDC);
                if (hBitmap != IntPtr.Zero)
                {
                    SelectObject(memDc, oldBits);
                    DeleteObject(hBitmap);
                }
                DeleteDC(memDc);
            }
        }
    }
}
