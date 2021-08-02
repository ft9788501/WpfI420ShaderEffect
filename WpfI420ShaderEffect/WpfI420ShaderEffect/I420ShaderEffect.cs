using System;
using System.Collections.Generic;
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

        static int textureWidth = 2200;
        static int textureHeight = 1200;
        static int videoWidth = 1920;
        static int videoHeight = 1080;
        YuvLoader yuvLoader = YuvLoader.LoadFromPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test1080.yuv"), videoWidth, videoHeight);
        WriteableBitmap writeableBitmapY = new WriteableBitmap(textureWidth, textureHeight, 96, 96, PixelFormats.Gray8, null);
        WriteableBitmap writeableBitmapU = new WriteableBitmap(textureWidth / 2, textureHeight / 2, 96, 96, PixelFormats.Gray8, null);
        WriteableBitmap writeableBitmapV = new WriteableBitmap(textureWidth / 2, textureHeight / 2, 96, 96, PixelFormats.Gray8, null);
        IntPtr backBufferY;
        IntPtr backBufferU;
        IntPtr backBufferV;

        public I420ShaderEffect()
        {
            PixelShader = new()
            {
                UriSource = new Uri(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I420ShaderEffect.ps"), UriKind.Absolute)
            };

            backBufferY = writeableBitmapY.BackBuffer;
            backBufferU = writeableBitmapU.BackBuffer;
            backBufferV = writeableBitmapV.BackBuffer;

            TextureY = new ImageBrush(writeableBitmapY);
            TextureU = new ImageBrush(writeableBitmapU);
            TextureV = new ImageBrush(writeableBitmapV);

            DispatcherTimer dispatcherTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(25)
            };

            yuvLoader.YuvFrameDataReceive((y, u, v, w, h) =>
            {
                if ((DateTime.Now - lastDateTime).TotalMilliseconds < 25)
                {
                    return;
                }
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
            });
            dispatcherTimer.Tick += (s, e) =>
            {
                writeableBitmapY.Lock();
                writeableBitmapY.AddDirtyRect(new Int32Rect(0, 0, textureWidth, textureHeight));
                writeableBitmapY.Unlock();
                writeableBitmapU.Lock();
                writeableBitmapU.AddDirtyRect(new Int32Rect(0, 0, textureWidth / 2, textureHeight / 2));
                writeableBitmapU.Unlock();
                writeableBitmapV.Lock();
                writeableBitmapV.AddDirtyRect(new Int32Rect(0, 0, textureWidth / 2, textureHeight / 2));
                writeableBitmapV.Unlock();
            };
            dispatcherTimer.Start();
        }
    }
}
