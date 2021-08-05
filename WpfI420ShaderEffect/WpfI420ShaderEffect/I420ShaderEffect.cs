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
    public class I420ShaderEffect : ShaderEffect
    {
        #region DependencyProperty

        public static readonly DependencyProperty InputProperty = RegisterPixelShaderSamplerProperty("Input", typeof(I420ShaderEffect), 0);
        public static readonly DependencyProperty TextureYProperty = RegisterPixelShaderSamplerProperty("TextureY", typeof(I420ShaderEffect), 1);
        public static readonly DependencyProperty TextureUProperty = RegisterPixelShaderSamplerProperty("TextureU", typeof(I420ShaderEffect), 2);
        public static readonly DependencyProperty TextureVProperty = RegisterPixelShaderSamplerProperty("TextureV", typeof(I420ShaderEffect), 3);

        public Brush Input
        {
            get => (Brush)GetValue(InputProperty);
            set => SetValue(InputProperty, value);
        }
        public ImageBrush TextureY
        {
            get => (ImageBrush)GetValue(TextureYProperty);
            set => SetValue(TextureYProperty, value);
        }
        public ImageBrush TextureU
        {
            get => (ImageBrush)GetValue(TextureUProperty);
            set => SetValue(TextureUProperty, value);
        }
        public ImageBrush TextureV
        {
            get => (ImageBrush)GetValue(TextureVProperty);
            set => SetValue(TextureVProperty, value);
        }

        #endregion

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        DateTime lastDateTime = DateTime.Now;

        static YuvSource yuvSource = new YuvSource()
        {
            Path = "352x288.yuv",
            Width = 352,
            Height = 288
        };
        //static YuvSource yuvSource = new YuvSource()
        //{
        //    Path = "test1080.yuv",
        //    Width = 1920,
        //    Height = 1080
        //};
        static int textureWidth = 1920;
        static int textureHeight = 1080;
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

        public I420ShaderEffect()
        {
            PixelShader = new()
            {
                UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderScaleEffect.ps"), UriKind.Absolute)
                //UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderEffect.ps"), UriKind.Absolute)
            };

            backBufferY = writeableBitmapY.BackBuffer;
            backBufferU = writeableBitmapU.BackBuffer;
            backBufferV = writeableBitmapV.BackBuffer;

            TextureY = new ImageBrush(writeableBitmapY);
            TextureU = new ImageBrush(writeableBitmapU);
            TextureV = new ImageBrush(writeableBitmapV);
            stopwatchYuv.Start();
            long lastYuv = 0;
            long lastRedner = 0;
            long lastWpf = 0;
            yuvLoader.YuvFrameDataReceive((y, u, v, w, h) =>
            {
                //if ((DateTime.Now - lastDateTime).TotalMilliseconds < 25)
                //{
                //    return;
                //}
                lastDateTime = DateTime.Now;
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
                }
                var interval = stopwatchYuv.ElapsedMilliseconds;
                Debug.WriteLine($"yuv data : {interval - lastYuv}");
                lastYuv = interval;
                yuvCount++;
            });

            stopwatchRender.Start();
            DispatcherTimer renderTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(1)
            };
            renderTimer.Tick += (s, e) =>
            {
                if (renderCount == 0)
                {
                    yuvCount = 0;
                }
                writeableBitmapY.Lock();
                writeableBitmapY.AddDirtyRect(new Int32Rect(0, 0, videoWidth, videoHeight));
                writeableBitmapY.Unlock();
                writeableBitmapU.Lock();
                writeableBitmapU.AddDirtyRect(new Int32Rect(0, 0, videoWidth / 2, videoHeight / 2));
                writeableBitmapU.Unlock();
                writeableBitmapV.Lock();
                writeableBitmapV.AddDirtyRect(new Int32Rect(0, 0, videoWidth / 2, videoHeight / 2));
                writeableBitmapV.Unlock();
                UpdateShaderValue(TextureYProperty);
                UpdateShaderValue(TextureUProperty);
                UpdateShaderValue(TextureVProperty);
                renderCount++;
                var interval = stopwatchRender.ElapsedMilliseconds;
                Debug.WriteLine($"render data : {interval - lastRedner}");
                lastRedner = interval;
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
                if (MainWindow.Main != null)
                {
                    MainWindow.Main.Title = $"[Wpf Fps:{(int)wpfFps} Count:{wpfFenderCount}] -- [Render Fps:{(int)renderFps} Count:{renderCount}] -- [Yuv Fps:{(int)yuvFps} Count:{yuvCount}] -- Lost: {yuvCount - renderCount}";
                }
                if (MainWindow1.Main != null)
                {
                    MainWindow1.Main.Title = $"[Wpf Fps:{(int)wpfFps} Count:{wpfFenderCount}] -- [Render Fps:{(int)renderFps} Count:{renderCount}] -- [Yuv Fps:{(int)yuvFps} Count:{yuvCount}] -- Lost: {yuvCount - renderCount}";
                }
            };
            fpsTimer.Start();
        }
    }
}
