using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfI420ShaderEffect
{
    public class YuvLoader : IDisposable
    {
        private class YuvFrameData
        {
            public byte[] Y { get; set; }
            public byte[] U { get; set; }
            public byte[] V { get; set; }
        }

        public static YuvLoader LoadFromPath(string yuvFilePath, int width, int height)
        {
            if (File.Exists(yuvFilePath))
            {
                return new YuvLoader(yuvFilePath, width, height);
            }
            else
            {
                return null;
            }
        }

        private readonly FileStream fileStream;
        private YuvFrameData[] YuvFrameDatas { get; }

        public int FrameCount { get; }
        public int Width { get; }
        public int Height { get; }

        private YuvLoader(string yuvFilePath, int width, int height)
        {
            fileStream = new FileStream(yuvFilePath, FileMode.Open);
            Width = width;
            Height = height;
            FrameCount = (int)(fileStream.Length / (width * height * 1.5));
            YuvFrameDatas = new YuvFrameData[FrameCount];
            int index = 0;
            while (fileStream.Position != fileStream.Length)
            {
                var yBuffer = new byte[Width * Height];
                var uBuffer = new byte[Width * Height / 4];
                var vBuffer = new byte[Width * Height / 4];
                fileStream.Read(yBuffer, 0, yBuffer.Length);
                fileStream.Read(uBuffer, 0, uBuffer.Length);
                fileStream.Read(vBuffer, 0, vBuffer.Length);

                var yuvFrameData = new YuvFrameData
                {
                    Y = yBuffer,
                    U = uBuffer,
                    V = vBuffer
                };
                YuvFrameDatas[index++] = yuvFrameData;
            }
        }
        public void YuvFrameDataReceive(Action<IntPtr, IntPtr, IntPtr, int, int> callback)
        {
            int index = 0;
            int interval = 25;
            new Thread(() =>
            {
                while (true)
                {
                    if (index == FrameCount)
                    {
                        index = 0;
                    }
                    var d = YuvFrameDatas[index++];
                    byte[] y = new byte[d.Y.Length];
                    byte[] u = new byte[d.U.Length];
                    byte[] v = new byte[d.V.Length];
                    Buffer.BlockCopy(d.Y, 0, y, 0, d.Y.Length);
                    Buffer.BlockCopy(d.U, 0, u, 0, d.U.Length);
                    Buffer.BlockCopy(d.V, 0, v, 0, d.V.Length);

                    callback?.Invoke(Marshal.UnsafeAddrOfPinnedArrayElement(y, 0), Marshal.UnsafeAddrOfPinnedArrayElement(u, 0), Marshal.UnsafeAddrOfPinnedArrayElement(v, 0), Width, Height);
                    Thread.Sleep(interval);
                }
            })
            {
                IsBackground = true
            }.Start();
        }

        public void Dispose()
        {
            fileStream?.Dispose();
        }
    }
}
