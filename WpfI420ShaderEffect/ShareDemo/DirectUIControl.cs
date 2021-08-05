using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareDemo
{
    public class DirectUIControl
    {
        private bool isMouseDown = false;
        private Point mouseDownLocation = Point.Empty;
        private Image image;
        private FrameDimension frameDimension;
        private int currentFrame;
        private int frameCount;
        private DateTime lastTime = DateTime.Now;

        public int X { get; set; }
        public int Y { get; set; }
        public Point Location
        {
            get => new Point(X, Y);
            set
            {
                X = value.X;
                Y = value.Y;
            }
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public Size Size => new Size(Width, Height);
        public Rectangle Bounds => new Rectangle(Location, Size);
        public Rectangle ClientBounds => new Rectangle(Point.Empty, Size);
        public Image Image
        {
            get => image;
            set
            {
                image = value;
                frameDimension = new FrameDimension(Image.FrameDimensionsList[0]);
                frameCount = Image.GetFrameCount(frameDimension);
            }
        }
        public Color BackColor { get; set; }
        public object Tag { get; set; }

        public void OnMouseDown(MouseEventArgs e)
        {
            isMouseDown = true;
            mouseDownLocation = e.Location;
        }
        public void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                X += e.X - mouseDownLocation.X;
                Y += e.Y - mouseDownLocation.Y;
            }
        }
        public void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
            mouseDownLocation = Point.Empty;
        }

        public void OnPaint(PaintEventArgs e)
        {
            if (Image != null)
            {
                if (currentFrame >= frameCount)
                {
                    currentFrame = 0;
                }
                if ((DateTime.Now - lastTime).TotalMilliseconds > 60)
                {
                    lastTime = DateTime.Now;
                    Image.SelectActiveFrame(frameDimension, currentFrame++);
                }
                e.Graphics.DrawImage(Image, ClientBounds);
            }
            else
            {
                using SolidBrush brush = new(BackColor);
                e.Graphics.FillRectangle(brush, ClientBounds);
            }
        }
    }
}
