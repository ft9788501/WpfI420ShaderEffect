using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DirtyRectTest
{
    public class DirtyRect
    {
        static Random random = new Random();
        private DirtyRectCollection owner;
        private Color backColor;

        public long Id { get; }
        public int X { get; }
        public int Y { get; }
        public Point Location => new Point(X, Y);
        public int Width { get; }
        public int Height { get; }
        public Size Size => new Size(Width, Height);
        public Rectangle Bounds => new Rectangle(Location, Size);


        public DirtyRect(DirtyRectCollection owner, long id, int x, int y, int width, int height)
        {
            this.owner = owner;
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            backColor = Color.FromArgb(128, random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
        }

        public void Draw(Graphics graphics)
        {
            using SolidBrush solidBrush = new SolidBrush(backColor);
            graphics.FillRectangle(solidBrush, Bounds);
            graphics.DrawString($"{X},{Y},{Width},{Height}", Control.DefaultFont, Brushes.Black, new PointF(X, Y));
        }

        public override string ToString()
        {
            return $"{X},{Y},{Width},{Height}";
        }
    }
}
