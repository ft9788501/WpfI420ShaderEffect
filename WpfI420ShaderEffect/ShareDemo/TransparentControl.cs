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
    public class TransparentControl : Control
    {
        #region DLLImport

        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);

        private readonly UInt32 WM_SYSCOMMAND = 0x112;
        private readonly UInt32 SC_MOVE = 0xF010;
        private readonly UInt32 HTCAPTION = 2;

        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        public TransparentControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.Opaque, true);
            Button button = new Button() { Width = 100, Height = 20 };
            this.Controls.Add(button);
        }
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle = 0x20; //WS_EX_TRANSPARENT
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(128, Color.Red));
            e.Graphics.FillRectangle(solidBrush, ClientRectangle);
        }
    }
}
