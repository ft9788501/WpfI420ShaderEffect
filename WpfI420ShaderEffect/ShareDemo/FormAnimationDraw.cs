using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareDemo
{
    public partial class FormAnimationDraw : Form
    {
        Timer timer;
        Random random = new Random();
        private readonly List<DirectUIControl> directUIControls = new();
        public FormAnimationDraw()
        {
            InitializeComponent();
            DoubleBuffered = true;
            for (int i = 0; i < 16; i++)
            {
                directUIControls.Add(new DirectUIControl()
                {
                    BackColor = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256)),
                    Location = new Point(i * 50, i * ClientSize.Height / 16),
                    Width = ClientSize.Height / 16,
                    Height = ClientSize.Height / 16,
                    Tag = true
                });
            }
            timer = new Timer()
            {
                Interval = 10
            };
            timer.Tick += (s, e) =>
            {
                foreach (DirectUIControl item in directUIControls)
                {
                    if ((bool)item.Tag)
                    {
                        item.Location = new Point(item.Location.X + 5, item.Location.Y);
                        if (item.Bounds.Right > ClientSize.Width)
                        {
                            item.Tag = !(bool)item.Tag;
                        }
                    }
                    else
                    {
                        item.Location = new Point(item.Location.X - 5, item.Location.Y);
                        if (item.Bounds.Left < 0)
                        {
                            item.Tag = !(bool)item.Tag;
                        }
                    }
                }
                Invalidate();
            };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            timer.Enabled = !timer.Enabled;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            foreach (var directUIControl in directUIControls)
            {
                var s = e.Graphics.Save();
                e.Graphics.TranslateTransform(directUIControl.X, directUIControl.Y);
                directUIControl.OnPaint(e);
                e.Graphics.Restore(s);
            }
        }
    }
}
