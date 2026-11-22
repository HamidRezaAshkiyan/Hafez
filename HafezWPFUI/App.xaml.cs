using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using HafezLibrary.DataAccess;
using HafezLibrary.Remote;
using HafezLibrary.Services;
using HafezLibrary.Settings;
using HafezWPFUI.Helper.Handlers;
using HafezWPFUI.Properties;
using HafezWPFUI.Services;
using HafezWPFUI.ViewModels;
using HafezWPFUI.ViewModels.Windows;
using HafezWPFUI.Views.Windows;

namespace HafezWPFUI
{
    public partial class App
    {
        public static IServiceProvider Services { get; private set; }

        public Volume Volume { get; set; } = new() { Value = 10 };

        private async void Application_Startup(object sender, StartupEventArgs e)
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

            DatabaseInitializer.EnsureInitialized();

            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            var cacheService = Services.GetRequiredService<IContentCacheService>();
            await cacheService.WarmUpAsync();

            AppSettings settings = AppSettingsService.Load();
            if (settings.AutoStartApiServer)
            {
                try
                {
                    var remoteServer = Services.GetRequiredService<HafezRemoteServer>();
                    await remoteServer.StartAsync(settings.ApiPort);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "Could not auto-start Hafez Remote API server");
                }
            }

            ThemeService.ApplySavedTheme();

            var mainWindow = Services.GetRequiredService<MainWindowView>();
            OperatorHotkeys.Attach(mainWindow);
            MainWindow = mainWindow;
            mainWindow.Show();

            Log.Information("Hafez application started.");
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IContentCacheService, ContentCacheService>();
            services.AddSingleton<RemoteSessionTokenService>();
            services.AddSingleton<HafezRemoteServer>();
            services.AddSingleton<ModernCameraService>();
            services.AddSingleton<OutputWindowViewModel>();
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<WebcamHandlerViewModel>();
            services.AddSingleton<ListenerWindowViewModel>();
            services.AddSingleton<MainWindowView>();
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
            if (Services is IDisposable disposable)
            {
                disposable.Dispose();
            }

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
