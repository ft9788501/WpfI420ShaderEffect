using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirtyRectTest
{
    /// <summary>
    /// https://www.cnblogs.com/timesking/archive/2010/05/24/1742735.html
    /// </summary>
    public class DirtyRectCollection : IList<DirtyRect>
    {
        Size canvasSize;
        List<DirtyRect> dirtyRects = new List<DirtyRect>();

        public DirtyRectCollection(Size canvasSize)
        {
            this.canvasSize = canvasSize;
        }

        public void Draw(Graphics graphics)
        {
            foreach (var item in dirtyRects)
            {
                item.Draw(graphics);
            }
        }

        public DirtyRect CreateDirtyRect(int width, int height)
        {
            Rectangle newBounds = GetAvailableArea(0, 0, width, height);
            var dirtyRect = new DirtyRect(this, dirtyRects.Count, newBounds.X, newBounds.Y, newBounds.Width, newBounds.Height);
            Add(dirtyRect);
            return dirtyRect;
        }

        private Rectangle GetAvailableArea(int x, int y, int width, int height)
        {
            Rectangle newBounds = new Rectangle(x, y, width, height);
            var intersectDirtyRect = dirtyRects.FirstOrDefault(r => newBounds.IntersectsWith(r.Bounds));
            if (intersectDirtyRect != null)
            {
                x = intersectDirtyRect.Bounds.Right;
                return GetAvailableArea(x, y, width, height);
            }
            else
            {
                if (newBounds.Right > canvasSize.Width)
                {
                    x = 0;
                    newBounds = new Rectangle(x, y, width, height);
                    intersectDirtyRect = dirtyRects.FirstOrDefault(r => newBounds.IntersectsWith(r.Bounds));
                    y = intersectDirtyRect.Bounds.Bottom;
                    return GetAvailableArea(x, y, width, height);
                }
            }
            return newBounds;
        }

        public void RemoveDirtyRect(Point location)
        {
            var dirtyRect = dirtyRects.FirstOrDefault(d => d.Bounds.Contains(location));
            if (dirtyRect == null)
            {
                return;
            }
            Remove(dirtyRect);
        }

        public DirtyRect this[int index] { get => dirtyRects[index]; set => dirtyRects[index] = value; }

        public int Count => dirtyRects.Count;

        public bool IsReadOnly => false;

        public void Add(DirtyRect item)
        {
            dirtyRects.Add(item);
        }

        public void Clear()
        {
            dirtyRects.Clear();
        }

        public bool Contains(DirtyRect item)
        {
            return dirtyRects.Contains(item);
        }

        public void CopyTo(DirtyRect[] array, int arrayIndex)
        {
            dirtyRects.CopyTo(array, arrayIndex);
        }

        public IEnumerator<DirtyRect> GetEnumerator()
        {
            return dirtyRects.GetEnumerator();
        }

        public int IndexOf(DirtyRect item)
        {
            return dirtyRects.IndexOf(item);
        }

        public void Insert(int index, DirtyRect item)
        {
            dirtyRects.Insert(index, item);
        }

        public bool Remove(DirtyRect item)
        {
            return dirtyRects.Remove(item);
        }

        public void RemoveAt(int index)
        {
            dirtyRects.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dirtyRects.GetEnumerator();
        }
    }
}
