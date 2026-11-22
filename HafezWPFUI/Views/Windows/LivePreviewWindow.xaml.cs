using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using HafezLibrary.Services;
using HafezLibrary.Settings;
using HafezWPFUI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HafezWPFUI.Views.Windows
{
    public partial class LivePreviewWindow
    {
        private readonly DispatcherTimer _clockTimer;

        public LivePreviewWindow()
        {
            InitializeComponent();

            if (GlobalConfig.Output != null)
            {
                LivePreviewSurface.Fill = new VisualBrush(GlobalConfig.Output)
                {
                    Stretch = Stretch.Uniform
                };
            }

            UpdatePrayerBanner();
            _clockTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
            _clockTimer.Tick += (_, _) => UpdatePrayerBanner();
            _clockTimer.Start();

            Closed += (_, _) => _clockTimer.Stop();
        }

        private void UpdatePrayerBanner()
        {
            AppSettings settings = AppSettingsService.Load();
            PrayerBannerText.Text = MosqueCalendarService.BuildBannerText(
                DateTime.Now,
                settings.CityName,
                settings.Latitude,
                settings.Longitude,
                settings.TimeZoneOffsetHours);
        }

        private void ToggleTheme_OnClick(object sender, RoutedEventArgs e)
        {
            ThemeService.ToggleTheme();
        }

        private void SnapSecondary_OnClick(object sender, RoutedEventArgs e)
        {
            DisplayLayoutService.SnapOutputToSecondaryDisplay(GlobalConfig.Output);
        }

        private void OpenQrPairing_OnClick(object sender, RoutedEventArgs e)
        {
            var remoteServer = App.Services?.GetService<HafezRemoteServer>();
            if (remoteServer != null)
            {
                var qrWindow = new QrPairingWindow(remoteServer);
                qrWindow.Show();
            }
        }
    }
}
