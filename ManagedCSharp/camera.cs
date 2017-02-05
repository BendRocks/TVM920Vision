using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//using Extensions;
using AForge.Imaging.Filters;
using System.Drawing.Imaging;

namespace ManagedCSharp
{
    static class Camera
    {
        private static FilterInfoCollection CaptureDevice;
        private static VideoCaptureDevice LastFrame;

        private static bool _IsStarted = false;

        /// <summary>
        /// Clone of the last received frame
        /// </summary>
        static Bitmap Bmp;

        static Camera()
        {
            CaptureDevice = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            LastFrame = new VideoCaptureDevice(CaptureDevice[0].MonikerString);
            LastFrame.NewFrame += LastFrame_NewFrame;

            if (LastFrame.CheckIfCrossbarAvailable())
                LastFrame.CrossbarVideoInput = LastFrame.AvailableCrossbarVideoInputs[0];

            // Null bitmap in case the camera is asked for a picture 
            // before one has been collected
            Bmp = new Bitmap(720, 576, PixelFormat.Format32bppRgb);
        }

        static internal void Start()
        {
            LastFrame.Start();
            _IsStarted = true;
        }

        static internal bool IsStarted
        {
        get { return _IsStarted; }

        }

        internal enum InputEnum { UpCam = 0, DownCam = 1 };
        static InputEnum _Input;

        internal static InputEnum Input
        {
            get { return _Input; }

            set
            {
                _Input = value;
                if (LastFrame.CheckIfCrossbarAvailable())
                    LastFrame.CrossbarVideoInput = LastFrame.AvailableCrossbarVideoInputs[Convert.ToInt32(_Input)];

                if (value == InputEnum.UpCam)
                    TVM920Control.UpLightOn();

                if (value == InputEnum.DownCam)
                    TVM920Control.DownLightOn();
            }
        }

        /// <summary>
        /// Called everytime a new frame of video is ready. At 30 FPS, this is a new update every 33mS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private static void LastFrame_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            lock (Bmp)
            {
                if (Bmp != null)
                    Bmp.Dispose();

                // Regardless of source, we always want a 32bpp rgb bitmap
                //Bmp = (Bitmap)eventArgs.Frame.Clone();
                Bmp = (Bitmap)eventArgs.Frame.Clone(new Rectangle(0, 0, eventArgs.Frame.Width, eventArgs.Frame.Height), System.Drawing.Imaging.PixelFormat.Format32bppRgb);
            }
        }

        //internal static Bitmap FlipBasedOnInput(Bitmap bmp)
        //{
        //    switch (_Input)
        //    {
        //        case InputEnum.DownCam: bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone); break;
        //        case InputEnum.UpCam: bmp.RotateFlip(RotateFlipType.RotateNoneFlipNone); break;
        //        default: throw new NotImplementedException("Unknown in FlipBasedOnInput()");
        //    }

        //    return bmp;
        //}

        /// <summary>
        /// Returns the last bitmap captured from the camera. The size may vary, but the bit depth will always be 32bppRGB
        /// </summary>
        /// <returns></returns>
        internal static Bitmap GetBitmap()
        {
            Bitmap userBmp;

            // Clone the bitmap so that the UI has its own copy to work on. The UI is responsible for 
            // disposing when finished
            lock (Bmp)
            {
                if ( (Bmp.Width == 720) && (Bmp.Height == 576) )
                    userBmp = (Bitmap)Bmp.Clone();
                else
                    userBmp = new Bitmap(Bmp, 720, 576);
            }

            return userBmp;
        }

        internal static void Close()
        {
            if (LastFrame != null && LastFrame.IsRunning == true)
            {
                LastFrame.SignalToStop();
                LastFrame.WaitForStop();
            }
        }
    }
}

