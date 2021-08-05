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
    public partial class FormLayeredDirectUI : LayeredForm
    {
        private readonly List<DirectUIControl> directUIControls = new();
        private DirectUIControl mouseDownDirectUIControl;
        public FormLayeredDirectUI()
        {
            InitializeComponent();
            directUIControls.Add(new DirectUIControl()
            {
                Image = global::ShareDemo.Properties.Resources.sks,
                X = 12,
                Y = 12,
                Width = 240,
                Height = 400
            });
            directUIControls.Add(new DirectUIControl()
            {
                Image = global::ShareDemo.Properties.Resources.girl,
                X = 310,
                Y = 12,
                Width = 455,
                Height = 400
            });
            directUIControls.Add(new DirectUIControl()
            {
                X = 97,
                Y = 57,
                Width = 342,
                Height = 309,
                BackColor = Color.FromArgb(128, Color.Red)
            });
            Timer timer = new Timer()
            {
                Interval = 25,
                Enabled = true
            };
            timer.Tick += (s, e) =>
            {
                UpdateLayeredWindow();
            };
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            mouseDownDirectUIControl = directUIControls.LastOrDefault(d => d.Bounds.Contains(e.Location));
            if (mouseDownDirectUIControl != null)
            {
                mouseDownDirectUIControl.OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X - mouseDownDirectUIControl.X, e.Y - mouseDownDirectUIControl.Y, e.Delta));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            var mouseMoveDirectUIControl = directUIControls.LastOrDefault(d => d.Bounds.Contains(e.Location));
            if (mouseMoveDirectUIControl != null)
            {
                mouseMoveDirectUIControl.OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, e.X - mouseMoveDirectUIControl.X, e.Y - mouseMoveDirectUIControl.Y, e.Delta));
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (mouseDownDirectUIControl != null)
            {
                mouseDownDirectUIControl.OnMouseUp(new MouseEventArgs(e.Button, e.Clicks, e.X - mouseDownDirectUIControl.X, e.Y - mouseDownDirectUIControl.Y, e.Delta));
            }
        }

        protected override void OnUpdateLayeredPaint(PaintEventArgs e)
        {
            base.OnUpdateLayeredPaint(e);
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
