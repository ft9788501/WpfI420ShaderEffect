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
    public partial class FormAnimationTween : Form
    {
        Timer timer;
        Random random = new Random();
        private readonly List<DirectUIControl> directUIControls = new();
        private readonly PointF[] ps = new PointF[] { new PointF(0, 0), new PointF(0.8f, 0), new PointF(0.2f, 1), new PointF(1, 1) };
        public FormAnimationTween()
        {
            InitializeComponent();
            DoubleBuffered = true;
            var tracks = Bezier.GetBezierCurves(ps, ClientSize.Width / 10);
            for (int i = 0; i < 16; i++)
            {
                directUIControls.Add(new DirectUIControl()
                {
                    BackColor = Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256)),
                    Location = new Point(0, i * ClientSize.Height / 16),
                    Width = ClientSize.Height / 16,
                    Height = ClientSize.Height / 16,
                    Tag = new ControlFlag()
                    {
                        Index = i * (tracks.Length / 16),
                        Flag = true
                    }
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
                    ControlFlag f = (ControlFlag)item.Tag;
                    if (f.Flag)
                    {
                        item.Location = new Point((int)(tracks[f.Index].X * ClientSize.Width), item.Location.Y);
                        f.Index++;
                        if (f.Index >= tracks.Length - 1)
                        {
                            f.Flag = !f.Flag;
                        }
                    }
                    else
                    {
                        item.Location = new Point((int)(tracks[f.Index].X * ClientSize.Width), item.Location.Y);
                        f.Index--;
                        if (f.Index <= 0)
                        {
                            f.Flag = !f.Flag;
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
            foreach (var item in Bezier.GetBezierCurves(ps, ClientSize.Width / 10))
            {
                e.Graphics.FillEllipse(Brushes.Red, new RectangleF(item.X * ClientSize.Width, item.Y * ClientSize.Height, 8, 8));
            }
        }

        private class ControlFlag
        {
            public int Index { get; set; }
            public bool Flag { get; set; }
        }
    }
}
