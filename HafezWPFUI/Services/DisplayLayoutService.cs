using System.Linq;
using System.Windows;
using System.Windows.Forms;
using HafezLibrary.Settings;
using HafezWPFUI.Views.Windows;

namespace HafezWPFUI.Services
{
    /// <summary>
    /// Automatically assigns OutputWindowView to a secondary projector/LED display
    /// and applies saved mosque ScenePresets.
    /// </summary>
    public static class DisplayLayoutService
    {
        public static bool SnapOutputToSecondaryDisplay(Window outputWindow)
        {
            if (outputWindow == null)
            {
                return false;
            }

            Screen[] screens = Screen.AllScreens;
            Screen target = screens.FirstOrDefault(s => !s.Primary) ?? Screen.PrimaryScreen;
            if (target == null)
            {
                return false;
            }

            outputWindow.WindowStartupLocation = WindowStartupLocation.Manual;
            outputWindow.WindowState = WindowState.Normal;
            outputWindow.Left = target.Bounds.Left;
            outputWindow.Top = target.Bounds.Top;
            outputWindow.Width = target.Bounds.Width;
            outputWindow.Height = target.Bounds.Height;
            outputWindow.WindowState = WindowState.Maximized;
            return !target.Primary;
        }

        public static void ApplyPreset(ScenePreset preset, MainWindowView mainWindow)
        {
            if (preset == null || mainWindow == null)
            {
                return;
            }

            if (preset.ShowCamera)
            {
                mainWindow.WebcamHandler?.CameraControllers("E");
            }
            else
            {
                mainWindow.WebcamHandler?.CameraControllers("D");
            }
        }
    }
}
