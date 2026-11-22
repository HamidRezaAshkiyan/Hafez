using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HafezLibrary.Models;
using HafezWPFUI.Services;
using HafezWPFUI.Views.Windows;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views
{
    public partial class WebcamHandlerView
    {
        private readonly ModernCameraService _cameraService = new();
        private List<CameraDeviceInfo> _devices = new();

        private bool DeviceExist { get; set; }
        public int? VideoSource { get; private set; }

        public WebcamHandlerView()
        {
            InitializeComponent();
            _cameraService.FrameReady += OnCameraFrameReady;
        }

        private void OnCameraFrameReady(BitmapSource frame)
        {
            Dispatcher.BeginInvoke(() =>
            {
                Output.ImgInput.Source = frame;
            });
        }

        public void CameraControllers(string command)
        {
            switch (command)
            {
                case "E":
                    if (VideoSource.HasValue)
                    {
                        _cameraService.Start(VideoSource.Value);
                    }

                    Output.ImgInput.Visibility = Visibility.Visible;
                    ToggleButtonCameraShowStatus.IsChecked = true;

                    Main.CameraPackIcon.Foreground = MainWindowView.DisabledColor;
                    Main.CameraPackIcon.Kind = PackIconKind.CameraOff;

                    EnabledServiceCount++;
                    Output.StopScreenSaver();
                    break;

                case "D":
                    _cameraService.Stop();
                    Output.ImgInput.Visibility = Visibility.Hidden;
                    ToggleButtonCameraShowStatus.IsChecked = false;

                    Main.CameraPackIcon.Foreground = MainWindowView.EnabledColor;
                    Main.CameraPackIcon.Kind = PackIconKind.CameraAlt;

                    EnabledServiceCount = Math.Max(0, EnabledServiceCount - 1);
                    Output.StartScreenSaver();
                    break;
            }
        }

        private void ComboBoxDefaultCameraName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (ComboBoxDefaultCameraName.Items.Count == 0 || ComboBoxDefaultCameraName.SelectedIndex < 0)
                {
                    return;
                }

                string cameraName = ComboBoxDefaultCameraName.SelectedItem.ToString();

                UserConfigModel.Update(new Dictionary<string, string> { { "DefaultCameraName", cameraName } });
                UserConfig.DefaultCameraName = cameraName;

                if (DeviceExist && ComboBoxDefaultCameraName.SelectedIndex < _devices.Count)
                {
                    _cameraService.Stop();
                    VideoSource = _devices[ComboBoxDefaultCameraName.SelectedIndex].Index;
                }
                else
                {
                    MessageBox.Show("دستگاه وجود ندارد. برنامه را دوباره راه اندازی کنید.");
                }
            }
            catch (Exception exception)
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonCameraShowStatus_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool isChecked = ToggleButtonCameraShowStatus.IsChecked == true;
                string cameraShowStatus = isChecked ? "E" : "D";

                UserConfigModel.Update(new Dictionary<string, string> { { "CameraShowStatus", cameraShowStatus } });
                UserConfig.CameraShowStatus = cameraShowStatus;

                CameraControllers(cameraShowStatus);
            }
            catch (Exception exception)
            {
                LogInformation(exception);
            }
        }

        public void GetCamList()
        {
            try
            {
                _devices = _cameraService.EnumerateDevices();
                ComboBoxDefaultCameraName.Items.Clear();

                if (_devices.Count == 0)
                {
                    DeviceExist = false;
                    VideoSource = null;
                    return;
                }

                DeviceExist = true;
                foreach (CameraDeviceInfo device in _devices)
                {
                    ComboBoxDefaultCameraName.Items.Add(device.Name);
                }

                VideoSource = _devices[0].Index;
            }
            catch (Exception exception)
            {
                DeviceExist = false;
                LogInformation(exception);
            }
        }

        public void StopCamera()
        {
            _cameraService.Stop();
        }
    }
}
