using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HafezWPFUI.Services;

namespace HafezWPFUI.ViewModels
{
    public partial class WebcamHandlerViewModel : ObservableObject
    {
        private readonly ModernCameraService _cameraService;

        [ObservableProperty]
        private bool _isCameraRunning;

        [ObservableProperty]
        private CameraDeviceInfo _selectedDevice;

        public ObservableCollection<CameraDeviceInfo> Devices { get; } = new();

        public WebcamHandlerViewModel(ModernCameraService cameraService)
        {
            _cameraService = cameraService;
        }

        [RelayCommand]
        private void RefreshDevices()
        {
            Devices.Clear();
            foreach (CameraDeviceInfo device in _cameraService.EnumerateDevices())
            {
                Devices.Add(device);
            }
        }

        [RelayCommand]
        private void ToggleCamera()
        {
            if (IsCameraRunning)
            {
                _cameraService.Stop();
                IsCameraRunning = false;
            }
            else if (SelectedDevice != null)
            {
                _cameraService.Start(SelectedDevice.Index);
                IsCameraRunning = true;
            }
        }
    }
}
