using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TVM920_v2
{
    public partial class Form1 : Form
    {
        [DllImport("TVM920Vision.dll", CallingConvention = CallingConvention.Cdecl)] 
        static extern void Stop();
        [DllImport("TVM920Vision.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void SetInput(int input);
        [DllImport("TVM920Vision.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern void GetData(IntPtr buffer, ref int byteCount);
        [DllImport("TVM920Vision.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr GetPng(ref int byteCount);
        [DllImport("TVM920Vision.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int Add(int i, int j);

        IntPtr unmanagedPointer = Marshal.AllocHGlobal(720 * 576 * 4 + 4);


        public Form1()
        {
            InitializeComponent();   
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
        }

        private void button1_Click(object sender, EventArgs e)       
        {
            //Text = Add(25, 5).ToString();

            int byteCount = 0;
            Stopwatch sw = Stopwatch.StartNew();

            GetData(unmanagedPointer, ref byteCount);

            byte[] data = new byte[byteCount];

            Marshal.Copy(unmanagedPointer, data, 0, byteCount);

            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            
            Bitmap bmp = ConvertPixelArrayToBitmap(data);

            Text = bmp.Size.ToString() + "   " + sw.ElapsedMilliseconds.ToString();
            pictureBox1.Image = bmp;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Stopwatch sw = Stopwatch.StartNew();

            int byteCount = 0;
            IntPtr dataPtr = GetPng(ref byteCount);
            byte[] data = new byte[byteCount];
            Marshal.Copy(dataPtr, data, 0, byteCount);
            Bitmap bmp;

            using (var stream = new MemoryStream(data))
            {
                bmp = (Bitmap)Image.FromStream(stream);
            }

            sw.Stop();

            Text = bmp.Size.ToString() + "   " + sw.ElapsedMilliseconds.ToString();
            pictureBox1.Image = bmp;
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

        }

        private Bitmap ConvertPixelArrayToBitmap(byte[] pixelArray)
        {
            int width = (pixelArray[0] << 8) + pixelArray[1];
            int height = (pixelArray[2] << 8) + pixelArray[3];

            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            var bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);

            Marshal.Copy(pixelArray, 4, bmpData.Scan0, width * height * 4);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)        
                SetInput(0);
            else if (radioButton2.Checked)
                SetInput(1);
        }

        
    }
}
