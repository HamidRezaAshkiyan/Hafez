using System.Windows;
using System.Windows.Media;
using static System.Windows.Media.Color;
using Color = System.Drawing.Color;

namespace HafezWPFUI.Helper.Handlers
{
    public static class ColorHandler
    {
        public static void SetColor(string propertyName, string colorValue)
        {
            Color                      color  = Color.FromName(colorValue);
            System.Windows.Media.Color mColor = FromArgb(color.A, color.R, color.G, color.B);
            SolidColorBrush            brush  = new SolidColorBrush(mColor);

            Application.Current.Resources[propertyName] = brush;
        }

        public static void SetStrokeSize(string propertyName, double strokeSize)
        {
            Application.Current.Resources[propertyName] = strokeSize;
        }

        /*public static void SetForeground(string propertyName, string foreground)
        {
            var color = Color.FromName(foreground);
            var mColor = FromArgb(color.A, color.R, color.G, color.B);
            var brush = new SolidColorBrush(mColor);
            //((SolidColorBrush)Resources["NotificationPanelBackground"]).Color = fromArgb;

            Application.Current.Resources[propertyName] = brush;
        }

        public static void SetBackground(string propertyName, string background)
        {
            var color = Color.FromName(background);
            var mColor = FromArgb(color.A, color.R, color.G, color.B);
            var brush = new SolidColorBrush(mColor);
            //((SolidColorBrush)Resources["NotificationPanelBackground"]).Color = fromArgb;


            /*MessageBox.Show(Properties.Settings.Default.NotificationBackground.ToString());
            Properties.Settings.Default.NotificationBackground = color;
            Properties.Settings.Default.Save();
            MessageBox.Show(Properties.Settings.Default.NotificationBackground.ToString());#1#

            Application.Current.Resources[propertyName] = brush;
        }*/
    }
}