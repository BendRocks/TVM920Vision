using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ManagedCSharp
{
    public static class ManagedClass
    {
        const int Depth = 4;  // 32bpp for all bitmaps

        /// <summary>
        /// Start camera thread running upon DLL load
        /// </summary>
        static ManagedClass()
        {
            Camera.Start();
            Camera.Input = 0;  
        }

        static public void Stop()
        {
            Camera.Close();
        }

        /// <summary>
        /// Selects input mux
        /// </summary>
        /// <param name="input"></param>
        static public void SetInput(int input)
        {
            if (input == 0)
                Camera.Input = Camera.InputEnum.DownCam;
            else if (input == 1)
                Camera.Input = Camera.InputEnum.UpCam;
        }

        /// <summary>
        /// Gets bitmap uncompressed as a stream of data. First two ints in bytestream are width and height. Total
        /// size of bytestream is width * height * 4 bytes per pixel + 4 bytes to hold width and height (MSB first)
        /// </summary>
        /// <param name="byteCount"></param>
        /// <returns></returns>
        public static void GetData(byte[] data, ref int byteCount)
        {
           

            Bitmap bmp = Camera.GetBitmap();

            Stopwatch sw = Stopwatch.StartNew();

            byte[] localData = ConvertBitmapToPixelArray(bmp);
            byteCount = localData.Length;

            sw.Stop();
            long elapsed = sw.ElapsedMilliseconds;

            Array.Copy(localData, data, byteCount);
            
        }

        public static byte[] GetPng(ref int byteCount)
        {
            Bitmap bmp = Camera.GetBitmap();

            byte[] data = ConvertBitmapToPngArray(bmp);
            byteCount = data.Length;

            return data;
        }

        public static int Add(int i, int j)
        {
            return i + j;
        }

        public static byte[] ConvertBitmapToPixelArray(Bitmap bmp)
        {
            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);

            // Add 4 bytes to hold 2 ints at the beginning:
            // byte[0] = Width MSB
            // byte[1] = Width LSB
            // byte[2 & 3] = Height
            int length = bmpData.Width * bmpData.Height * Depth + 4;
            byte[] data = new byte[length];

            data[0] = (byte)(bmp.Width >> 8);
            data[1] = (byte)(bmp.Width);
            data[2] = (byte)(bmp.Height >> 8);
            data[3] = (byte)(bmp.Height);

            Marshal.Copy(bmpData.Scan0, data, 4, length-4);
            bmp.UnlockBits(bmpData);
            return data;
        }

        public static byte[] ConvertBitmapToPngArray(Bitmap bmp)
        {
            using (var stream = new MemoryStream())
            {
                bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }
    }
}
