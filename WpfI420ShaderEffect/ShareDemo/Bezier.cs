using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareDemo
{
    /// <summary>
    /// https://cubic-bezier.com/#.17,.67,.83,.67
    /// </summary>
    public static class Bezier
    {
        private static float Distance(PointF p1, PointF p2)
        {
            return (float)(Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y)));
        }
        /// <summary> 获取绘制n阶贝塞尔曲线的路径点集合
        /// </summary>  
        /// <param name="points">输入点</param>  
        /// <returns>绘制n阶贝塞尔曲线的路径点集合</returns>  
        public static PointF[] GetBezierCurves(PointF[] points, int frameCount)
        {
            int count = points.Length;
            float length = 0;
            for (int i = 1; i < points.Length; i++)
            {
                length += Distance(points[i - 1], points[i]);
            }
            float step = 1f / frameCount;
            List<PointF> bezier_curves_points = new List<PointF>();
            float t = 0F;
            do
            {
                PointF temp_point = BezierInterpolationFunc(t, points, count);    // 计算插值点  
                t += step;
                bezier_curves_points.Add(temp_point);
            }
            while (t <= 1 && count > 1);    // 一个点的情况直接跳出.  
            return bezier_curves_points.ToArray();  // 曲线轨迹上的所有坐标点  
        }
        /// <summary>  
        /// n阶贝塞尔曲线插值计算函数  
        /// 根据起点，n个控制点，终点 计算贝塞尔曲线插值  
        /// </summary>  
        /// <param name="t">当前插值位置0~1 ，0为起点，1为终点</param>  
        /// <param name="points">起点，n-1个控制点，终点</param>  
        /// <param name="count">n+1个点</param>  
        /// <returns></returns>  
        private static PointF BezierInterpolationFunc(float t, PointF[] points, int count)
        {
            PointF PointF = new PointF();
            float[] part = new float[count];
            float sum_x = 0, sum_y = 0;
            for (int i = 0; i < count; i++)
            {
                ulong tmp;
                int n_order = count - 1;    // 阶数  
                tmp = CalcCombinationNumber(n_order, i);
                sum_x += (float)(tmp * points[i].X * Math.Pow((1 - t), n_order - i) * Math.Pow(t, i));
                sum_y += (float)(tmp * points[i].Y * Math.Pow((1 - t), n_order - i) * Math.Pow(t, i));
            }
            PointF.X = sum_x;
            PointF.Y = sum_y;
            return PointF;
        }
        /// <summary> 计算组合数公式
        /// </summary>  
        /// <param name="n"></param>  
        /// <param name="k"></param>  
        /// <returns></returns>  
        private static ulong CalcCombinationNumber(int n, int k)
        {
            ulong[] result = new ulong[n + 1];
            for (int i = 1; i <= n; i++)
            {
                result[i] = 1;
                for (int j = i - 1; j >= 1; j--)
                    result[j] += result[j - 1];
                result[0] = 1;
            }
            return result[k];
        }
    }
}
