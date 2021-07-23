using Serilog;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using HafezWPFUI.Helper.Handlers;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using HafezWPFUI.Properties;

namespace HafezWPFUI
{
    public partial class App
    {
        public Volume Volume { get; set; } = new() {Value = 10};

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ControlHandlers.PhysicalKeyValidation();

            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Debug()
                         .WriteTo.Debug()
                         .WriteTo.File("Logs\\Log.log", fileSizeLimitBytes: 10000, retainedFileCountLimit: 5)
                         //.WriteTo.RollingFile("Logs\\Log.log", fileSizeLimitBytes: 10000, retainedFileCountLimit: 5)
                         .CreateLogger();

            AppCenter.Start("0e71973a-7f39-4947-ab8d-70bb5dd33445",
                            typeof(Analytics), typeof(Crashes));

            Log.Information("Application Stated.");
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.Information("Application Closed.");
        }
    }

    public class Volume : INotifyPropertyChanged
    {
        private double _value;

        public double Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}