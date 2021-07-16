using BespokeFusion;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Media;

namespace HafezWPFUI.Helper.Handlers
{
    public static class MessageBoxHandler
    {
        public static void ShowMessage(string title, string message)
        {
            // todo replace iran sans with Estedad or sth ( for copy right )
            // var font = new FontFamily(new Uri("Resources/Fonts/"), "./#Vazir");
            var font = new FontFamily("Vazir");
            var brush = new SolidColorBrush(new PaletteHelper().GetTheme().PrimaryMid.Color);

            using var msg = new CustomMaterialMessageBox
            {
                FontFamily = font,
                TxtMessage = { Text = message },
                TxtTitle = { Text = title },
                BtnOk = { Content = "تایید", Background = brush, BorderBrush = Brushes.Transparent },
                BtnCancel = { Visibility = Visibility.Collapsed },
                BtnCopyMessage = { Visibility = Visibility.Collapsed },
                //MainContentControl = { Background = backColor },
                TitleBackgroundPanel = { Background = brush },
                BorderBrush = brush,
                FlowDirection = FlowDirection.RightToLeft,
                Width = 300,
                Height = 220
            };

            msg.Show();
        }

        public static CustomMaterialMessageBox GetMaterialMessageBox(string title, string message)
        {
            //var fontFamily = new FontFamily(new Uri("Resources/Fonts/"), "./#Vazir");
            var font = new FontFamily("Vazir");
            var brush = new SolidColorBrush(
                new PaletteHelper().GetTheme().PrimaryMid.Color);

            var msg = new CustomMaterialMessageBox
            {
                FontFamily = font,
                TxtMessage = { Text = message },
                TxtTitle = { Text = title },
                BtnCancel =
                {
                    Content = "لغو",
                    BorderBrush = Brushes.Transparent,
                    Foreground = Brushes.White
                },
                BtnOk =
                {
                    Content = "تایید",
                    Background = brush,
                    BorderBrush = Brushes.Transparent,
                },
                BtnCopyMessage = { Visibility = Visibility.Hidden },
                TitleBackgroundPanel = { Background = brush },
                BorderBrush = brush,
                FlowDirection = FlowDirection.RightToLeft,
                Width = 300,
                Height = 220
            };

            return msg;
        }
    }
}