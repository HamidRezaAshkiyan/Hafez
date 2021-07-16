using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HafezWPFUI.Helper.Handlers
{
    public static class PanelHandler
    {
        public static BitmapImage GetImageByAddress(this string imagePath)
        {
            if (!File.Exists(imagePath)) return null;
            var output = new BitmapImage(new Uri(imagePath));

            return output;
        }

        public static void SetPanelMargin(string panelName, Thickness margin)
        {
            Application.Current.Resources.Remove($"{panelName}PanelMargin");
            Application.Current.Resources.Add($"{panelName}PanelMargin", margin);
        }
    }
}