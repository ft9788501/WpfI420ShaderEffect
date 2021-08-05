using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfI420ShaderEffect
{
    public class YuvTextures
    {
        public static YuvTextures Instance { get; }

        static YuvTextures()
        {
            Instance = new YuvTextures();
        }

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        //static YuvSource yuvSource = new YuvSource()
        //{
        //    Path = "352x288.yuv",
        //    Width = 352,
        //    Height = 288
        //};
        static YuvSource yuvSource = new YuvSource()
        {
            Path = "test1080.yuv",
            Width = 1920,
            Height = 1080
        };
        static int textureWidth = 1920 * 2;
        static int textureHeight = 1080 * 2;
        static int videoWidth = yuvSource.Width;
        static int videoHeight = yuvSource.Height;
        static YuvLoader yuvLoader = YuvLoader.LoadFromPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, yuvSource.Path), videoWidth, videoHeight);
        WriteableBitmap writeableBitmapY = new WriteableBitmap(textureWidth, textureHeight, 96, 96, PixelFormats.Gray8, null);
        WriteableBitmap writeableBitmapU = new WriteableBitmap(textureWidth / 2, textureHeight / 2, 96, 96, PixelFormats.Gray8, null);
        WriteableBitmap writeableBitmapV = new WriteableBitmap(textureWidth / 2, textureHeight / 2, 96, 96, PixelFormats.Gray8, null);
        IntPtr backBufferY;
        IntPtr backBufferU;
        IntPtr backBufferV;
        int yuvCount = 0;
        int renderCount = 0;
        int wpfFenderCount = 0;
        Stopwatch stopwatchYuv = new Stopwatch();
        Stopwatch stopwatchRender = new Stopwatch();
        Stopwatch stopwatchWpf = new Stopwatch();

        public WriteableBitmap WriteableBitmapY => writeableBitmapY;
        public WriteableBitmap WriteableBitmapU => writeableBitmapU;
        public WriteableBitmap WriteableBitmapV => writeableBitmapV;
        public int VideoWidth => videoWidth;
        public int VideoHeight => videoHeight;

        public PixelShader PixelShader { get; } = new()
        {
            //UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderScaleEffect.ps"), UriKind.Absolute)
            //UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderEffect.ps"), UriKind.Absolute)
            UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderEffectx2.ps"), UriKind.Absolute)
        };

        public YuvTextures()
        {
            backBufferY = writeableBitmapY.BackBuffer;
            backBufferU = writeableBitmapU.BackBuffer;
            backBufferV = writeableBitmapV.BackBuffer;
            stopwatchYuv.Start();
            long lastYuv = 0;
            long lastRedner = 0;
            long lastWpf = 0;
            yuvLoader.YuvFrameDataReceive((y, u, v, w, h) =>
            {
                unsafe
                {
                    for (int i = 0; i < videoHeight; i++)
                    {
                        IntPtr newVPoint = new(y.ToInt64() + i * videoWidth);
                        IntPtr newPoint = new(backBufferY.ToInt64() + i * textureWidth);
                        CopyMemory(newPoint, newVPoint, videoWidth);
                    }
                    for (int i = 0; i < videoHeight / 2; i++)
                    {
                        IntPtr newVPoint = new(u.ToInt64() + i * videoWidth / 2);
                        IntPtr newPoint = new(backBufferU.ToInt64() + i * textureWidth / 2);
                        CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    }
                    for (int i = 0; i < videoHeight / 2; i++)
                    {
                        IntPtr newVPoint = new(v.ToInt64() + i * videoWidth / 2);
                        IntPtr newPoint = new(backBufferV.ToInt64() + i * textureWidth / 2);
                        CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    }
                    Application.Current.Dispatcher.Invoke(()=> 
                    {
                        writeableBitmapY.Lock();
                        writeableBitmapY.AddDirtyRect(new Int32Rect(0, 0, videoWidth, videoHeight));
                        writeableBitmapY.Unlock();
                        //writeableBitmapU.Lock();
                        //writeableBitmapU.AddDirtyRect(new Int32Rect(0, 0, videoWidth / 2, videoHeight / 2));
                        //writeableBitmapU.Unlock();
                        //writeableBitmapV.Lock();
                        //writeableBitmapV.AddDirtyRect(new Int32Rect(0, 0, videoWidth / 2, videoHeight / 2));
                        //writeableBitmapV.Unlock();
                    });

                    ////for (int i = 0; i < videoHeight; i++)
                    ////{
                    ////    IntPtr newVPoint = new(y.ToInt64() + i * videoWidth);
                    ////    IntPtr newPoint = new(backBufferY.ToInt64() + (i + 1) * textureWidth);
                    ////    CopyMemory(newPoint, newVPoint, videoWidth);
                    ////}
                    ////for (int i = 0; i < videoHeight / 2; i++)
                    ////{
                    ////    IntPtr newVPoint = new(u.ToInt64() + i * videoWidth / 2);
                    ////    IntPtr newPoint = new(backBufferU.ToInt64() + (i + 1) * textureWidth / 2);
                    ////    CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    ////}
                    ////for (int i = 0; i < videoHeight / 2; i++)
                    ////{
                    ////    IntPtr newVPoint = new(v.ToInt64() + i * videoWidth / 2);
                    ////    IntPtr newPoint = new(backBufferV.ToInt64() + (i + 1) * textureWidth / 2);
                    ////    CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    ////}

                    //for (int i = 0; i < videoHeight; i++)
                    //{
                    //    IntPtr newVPoint = new(y.ToInt64() + i * videoWidth);
                    //    IntPtr newPoint = new(backBufferY.ToInt64() + i * textureWidth + videoWidth/2);
                    //    CopyMemory(newPoint, newVPoint, videoWidth);
                    //}
                    //for (int i = 0; i < videoHeight / 2; i++)
                    //{
                    //    IntPtr newVPoint = new(u.ToInt64() + i * videoWidth / 2);
                    //    IntPtr newPoint = new(backBufferU.ToInt64() + i * textureWidth / 2 + videoWidth / 4);
                    //    CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    //}
                    //for (int i = 0; i < videoHeight / 2; i++)
                    //{
                    //    IntPtr newVPoint = new(v.ToInt64() + i * videoWidth / 2);
                    //    IntPtr newPoint = new(backBufferV.ToInt64() + i * textureWidth / 2 + videoWidth / 4);
                    //    CopyMemory(newPoint, newVPoint, videoWidth / 2);
                    //}
                }
                var interval = stopwatchYuv.ElapsedMilliseconds;
                Debug.WriteLine($"yuv data : {interval - lastYuv}");
                lastYuv = interval;
                yuvCount++;
            });

            stopwatchRender.Start();
            DispatcherTimer renderTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(15)
            };
            renderTimer.Tick += (s, e) =>
            {
                //if (renderCount == 0)
                //{
                //    yuvCount = 0;
                //}
              
                //renderCount++;
                //var interval = stopwatchRender.ElapsedMilliseconds;
                //Debug.WriteLine($"render data : {interval - lastRedner}");
                //lastRedner = interval;
            };
            renderTimer.Start();
            stopwatchWpf.Start();
            CompositionTarget.Rendering += (s, e) =>
            {
                wpfFenderCount++;
                var interval = stopwatchWpf.ElapsedMilliseconds;
                Debug.WriteLine($"wpf data : {interval - lastWpf}");
                lastWpf = interval;
            };
            DateTime startTime = DateTime.Now;
            DispatcherTimer fpsTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(5)
            };
            fpsTimer.Tick += (s, e) =>
            {
                var passTime = (DateTime.Now - startTime).TotalSeconds;
                var renderFps = renderCount / passTime;
                var wpfFps = wpfFenderCount / passTime;
                var yuvFps = yuvCount / passTime;
                var title = $"[Wpf Fps:{(int)wpfFps} Count:{wpfFenderCount}] -- [Render Fps:{(int)renderFps} Count:{renderCount}] -- [Yuv Fps:{(int)yuvFps} Count:{yuvCount}] -- Lost: {yuvCount - renderCount}";

                if (MainWindow.Main != null)
                {
                    MainWindow.Main.Title = title;
                }
                if (MainWindow1.Main != null)
                {
                    MainWindow1.Main.Title = title;
                }
                if (MainWindowStatic.Main != null)
                {
                    MainWindowStatic.Main.Title = title;
                }
                if (MainWindowStatic1.Main != null)
                {
                    MainWindowStatic1.Main.Title = title;
                }
            };
            fpsTimer.Start();
        }
    }
}
