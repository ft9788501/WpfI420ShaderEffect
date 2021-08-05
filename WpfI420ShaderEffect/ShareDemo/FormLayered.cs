using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareDemo
{
    public partial class FormLayered : LayeredForm
    {
        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, UInt32 wParam, UInt32 lParam);
        private readonly UInt32 WM_SYSCOMMAND = 0x112;
        private readonly UInt32 SC_MOVE = 0xF010;
        private readonly UInt32 HTCAPTION = 2;

        public FormLayered()
        {
            InitializeComponent();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            ReleaseCapture();
            SendMessage(Handle, WM_SYSCOMMAND, SC_MOVE + HTCAPTION, 0);
        }

        protected override void OnUpdateLayeredPaint(PaintEventArgs e)
        {
            base.OnUpdateLayeredPaint(e);
            e.Graphics.DrawImage(global::ShareDemo.Properties.Resources.radiation, ClientRectangle);
        }
    }
}
