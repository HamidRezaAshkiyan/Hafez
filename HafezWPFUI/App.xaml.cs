using Serilog;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using HafezWPFUI.Properties;

namespace HafezWPFUI
{
    public partial class App
    {
        public Volume Volume { get; set; } = new() { Value = 10 };

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string logDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Hafez",
                "Logs");
            Directory.CreateDirectory(logDir);

            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Information()
                         .WriteTo.Debug()
                         .WriteTo.File(
                             Path.Combine(logDir, "hafez-.log"),
                             rollingInterval: RollingInterval.Day,
                             fileSizeLimitBytes: 5 * 1024 * 1024,
                             retainedFileCountLimit: 14)
                         .CreateLogger();

            DispatcherUnhandledException += OnDispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += OnDomainUnhandledException;

            Log.Information("Hafez application started.");
        }

        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Log.Error(e.Exception, "Unhandled UI exception");
        }

        private void OnDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Unhandled domain exception (IsTerminating={IsTerminating})", e.IsTerminating);
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Log.Information("Hafez application closed.");
            Log.CloseAndFlush();
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
