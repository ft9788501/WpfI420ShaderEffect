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
    public partial class FormAnimationControl : Form
    {
        Timer timer;
        Random random = new Random();
        public FormAnimationControl()
        {
            InitializeComponent();
            for (int i = 0; i < 16; i++)
            {
                Controls.Add(new Control()
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
                foreach (Control item in Controls)
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
            };
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            timer.Enabled = !timer.Enabled;
        }
    }
}
