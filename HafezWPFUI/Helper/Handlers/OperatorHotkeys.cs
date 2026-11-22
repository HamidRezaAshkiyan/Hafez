using System.Windows;
using System.Windows.Input;
using HafezWPFUI.Services;
using HafezWPFUI.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace HafezWPFUI.Helper.Handlers
{
    /// <summary>
    /// Operator keyboard shortcuts for live mosque ceremonies:
    /// Ctrl+D: Toggle Dark Mode | Ctrl+Q: Open Mobile QR Pairing | Ctrl+P: Live Output Preview | F11: Snap Output to Secondary Monitor.
    /// </summary>
    public static class OperatorHotkeys
    {
        public static void Attach(Window window)
        {
            if (window == null)
            {
                return;
            }

            window.PreviewKeyDown += (_, e) =>
            {
                bool ctrl = (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control;

                if (ctrl && e.Key == Key.D)
                {
                    ThemeService.ToggleTheme();
                    e.Handled = true;
                }
                else if (ctrl && e.Key == Key.Q)
                {
                    var remoteServer = App.Services?.GetService<HafezRemoteServer>();
                    if (remoteServer != null)
                    {
                        new QrPairingWindow(remoteServer).Show();
                    }

                    e.Handled = true;
                }
                else if (ctrl && e.Key == Key.P)
                {
                    new LivePreviewWindow().Show();
                    e.Handled = true;
                }
                else if (e.Key == Key.F11)
                {
                    DisplayLayoutService.SnapOutputToSecondaryDisplay(GlobalConfig.Output);
                    e.Handled = true;
                }
            };
        }
    }
}
