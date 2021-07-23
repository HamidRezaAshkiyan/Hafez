using AForge.Video;
using AForge.Video.DirectShow;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HafezLibrary.Models;
using HafezWPFUI.Views.Windows;
using static HafezWPFUI.GlobalConfig;
using Image = System.Drawing.Image;

namespace HafezWPFUI.Views
{
    public partial class WebcamHandlerView
    {
        private bool                 DeviceExist  { get; set; }
        private FilterInfoCollection VideoDevices { get; set; }
        public  VideoCaptureDevice   VideoSource  { get; set; }

        public WebcamHandlerView()
        {
            InitializeComponent();
        }

        public void CameraControllers(string command)
        {
            //var parent = this.TryFindParent<MainWindow>();

            switch ( command )
            {
                case "E":

                    StartVideoDevice(VideoSource);
                    //GlobalConfig.Output.RadWebCam.Start();
                    //GlobalConfig.Output.RadWebCam.Visibility = Visibility.Visible;
                    Output.ImgInput.Visibility             = Visibility.Visible;
                    ToggleButtonCameraShowStatus.IsChecked = true;

                    //Change packIcon
                    Main.CameraPackIcon.Foreground = MainWindowView.DisabledColor;
                    Main.CameraPackIcon.Kind       = PackIconKind.CameraOff;


                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                    // GlobalConfig.CameraShowStatus = "D";
                    break;

                case "D":

                    StopVideoSource(VideoSource);
                    /*GlobalConfig.Output.RadWebCam.Stop();
                    GlobalConfig.Output.RadWebCam.Visibility = Visibility.Hidden;*/
                    Output.ImgInput.Visibility             = Visibility.Hidden;
                    ToggleButtonCameraShowStatus.IsChecked = false;

                    //Change packIcon
                    Main.CameraPackIcon.Foreground = MainWindowView.EnabledColor;
                    Main.CameraPackIcon.Kind       = PackIconKind.CameraAlt;


                    EnabledServiceCount--;
                    Output.StartScreenSaver();
                    // GlobalConfig.CameraShowStatus = "E";
                    break;
            }
        }

        private void ComboBoxDefaultCameraName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxDefaultCameraName.Items.Count == 0 )
                {
                    return;
                }

                string? cameraName = ComboBoxDefaultCameraName.SelectedItem.ToString();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> {{"DefaultCameraName", cameraName}});

                //GlobalConfig update
                UserConfig.DefaultCameraName = cameraName;

                //Set value
                //var friendlyName = RadWebCam.GetVideoCaptureDevices()[ComboBoxDefaultCameraName.SelectedIndex].FriendlyName;

                if ( DeviceExist )
                {
                    if ( VideoSource != null )
                    {
                        StopVideoSource(VideoSource);
                    }

                    if ( ComboBoxDefaultCameraName.SelectedIndex != -1 )
                    {
                        VideoSource =
                            new VideoCaptureDevice(VideoDevices[ComboBoxDefaultCameraName.SelectedIndex].MonikerString);
                    }

                    //Thread.Sleep(500);
                    //if ((bool)ToggleButtonCameraShowStatus.IsChecked) StartVideoDevice(_videoSource);
                }
                else
                {
                    MessageBox.Show("دستگاه وجود ندارد. برنامه را دوباره راه اندازی کنید.");
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonCameraShowStatus_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //var parent = this.TryFindParent<MainWindow>();
                bool   isChecked        = (bool) ToggleButtonCameraShowStatus.IsChecked;
                string cameraShowStatus = isChecked ? "E" : "D";

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> {{"CameraShowStatus", cameraShowStatus}});

                //GlobalConfig update
                UserConfig.CameraShowStatus = cameraShowStatus;

                //Set value
                CameraControllers(cameraShowStatus);
                /*if ((bool) ToggleButtonCameraShowStatus.IsChecked)
            {
                //CameraControllers("D");
               */                                        /* StopVideoSource(_videoSource);

                parent.Output.ImgInput.Visibility = Visibility.Hidden;*/
                /*parent.Output.ImgInput.Source = null;*/ /*
            }
            else
            {
                //CameraControllers("E");
                */ /* StartVideoDevice(_videoSource);

                 parent.Output.ImgInput.Visibility = Visibility.Visible;
                 parent.Output.ImgInput.Source = null;*/ /*
            }*/
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        #region CameraMethods

        // TODO move to library
        public void GetCamList()
        {
            try
            {
                VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                ComboBoxDefaultCameraName.Items.Clear();
                if ( VideoDevices.Count == 0 )
                {
                    throw new ApplicationException();
                }

                DeviceExist = true;
                foreach ( FilterInfo device in VideoDevices )
                {
                    ComboBoxDefaultCameraName.Items.Add(device.Name);
                }

                //ComboBoxDefaultCameraName.SelectedIndex = 0;
            }
            catch ( ApplicationException )
            {
                DeviceExist = false;
                MessageBox.Show("No capture device on your system");
            }
        }

        public void StartVideoDevice(VideoCaptureDevice source)
        {
            if ( source == null )
            {
                return;
            }

            // TODO use delegate to send this method
            source.NewFrame += Video_NewFrame;
            source.Start();
        }

        public static void StopVideoSource(VideoCaptureDevice source)
        {
            if ( source == null )
            {
                return;
            }

            if ( source.IsRunning )
            {
                source.SignalToStop();
            }
        }

        //close the device safely

        public void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Image img = (Bitmap) eventArgs.Frame.Clone();

                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);
                ms.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();

                bitmapImage.Freeze();

                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    Output.ImgInput.Source = bitmapImage;
                }));
            }
            catch ( Exception )
            {
                // ignored
            }
        }

        #endregion
    }
}