using System.Windows;
using System.Windows.Media;

namespace HafezWPFUI.Helper.Handlers
{
    public static class FontHandler
    {
        public static void SetFontSize(string propertyName, double fontSize)
        {
            Application.Current.Resources[propertyName] = fontSize;
        }

        public static void SetFontFamily(string propertyName, FontFamily fontFamily)
        {
            Application.Current.Resources[propertyName] = fontFamily;
        }

        public static void SetPanelHeight(string propertyName, double panelHeight)
        {
            Application.Current.Resources[propertyName] = panelHeight;
        }
    }
}