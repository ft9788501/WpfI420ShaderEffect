using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirtyRectTest
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private DirtyRectCollection dirtyRects;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
            dirtyRects = new DirtyRectCollection(ClientSize);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                int width = random.Next(ClientSize.Width / 16, ClientSize.Width / 4 + 1);
                int height = random.Next(ClientSize.Height / 16, ClientSize.Height / 4 + 1);
                dirtyRects.CreateDirtyRect(width, height);
            }
            else
            {
                dirtyRects.RemoveDirtyRect(e.Location);
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            dirtyRects.Draw(e.Graphics);
        }
    }
}
