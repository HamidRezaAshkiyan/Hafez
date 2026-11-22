using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using OpenCvSharp;
using OpenCvSharp.WpfExtensions;

namespace HafezWPFUI.Services
{
    public record CameraDeviceInfo(int Index, string Name);

    /// <summary>
    /// Modern OpenCvSharp4 camera capture service replacing legacy AForge.Video.DirectShow.
    /// Captures frames on a background thread and emits frozen WPF BitmapSources.
    /// </summary>
    public sealed class ModernCameraService : IDisposable
    {
        private VideoCapture _capture;
        private CancellationTokenSource _cts;
        private Task _captureLoop;
        private readonly object _sync = new();

        public bool IsRunning { get; private set; }
        public int ActiveDeviceIndex { get; private set; } = -1;

        public event Action<BitmapSource> FrameReady;

        public List<CameraDeviceInfo> EnumerateDevices(int maxProbe = 6)
        {
            var devices = new List<CameraDeviceInfo>();
            for (int i = 0; i < maxProbe; i++)
            {
                using var probe = new VideoCapture(i, VideoCaptureAPIs.DSHOW);
                if (probe.IsOpened())
                {
                    devices.Add(new CameraDeviceInfo(i, $"دوربین {i + 1} (Device #{i})"));
                    probe.Release();
                }
            }

            return devices;
        }

        public void Start(int deviceIndex = 0)
        {
            Stop();

            lock (_sync)
            {
                _capture = new VideoCapture(deviceIndex, VideoCaptureAPIs.DSHOW);
                if (!_capture.IsOpened())
                {
                    _capture.Dispose();
                    _capture = null;
                    throw new InvalidOperationException($"Unable to open camera device #{deviceIndex}.");
                }

                ActiveDeviceIndex = deviceIndex;
                IsRunning = true;
                _cts = new CancellationTokenSource();
                CancellationToken token = _cts.Token;

                _captureLoop = Task.Run(() => CaptureLoop(token), token);
            }
        }

        private void CaptureLoop(CancellationToken token)
        {
            using var frame = new Mat();
            while (!token.IsCancellationRequested)
            {
                VideoCapture capture;
                lock (_sync)
                {
                    capture = _capture;
                }

                if (capture == null || !capture.IsOpened())
                {
                    break;
                }

                if (capture.Read(frame) && !frame.Empty())
                {
                    BitmapSource bitmap = frame.ToBitmapSource();
                    bitmap.Freeze();
                    FrameReady?.Invoke(bitmap);
                }
                else
                {
                    Thread.Sleep(30);
                }
            }
        }

        public void Stop()
        {
            lock (_sync)
            {
                IsRunning = false;
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;

                _capture?.Release();
                _capture?.Dispose();
                _capture = null;
                ActiveDeviceIndex = -1;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
