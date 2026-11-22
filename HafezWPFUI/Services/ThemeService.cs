using HafezLibrary.Settings;
using MaterialDesignThemes.Wpf;

namespace HafezWPFUI.Services
{
    /// <summary>
    /// Runtime Light / Dark Mode theme switcher persisted to %AppData%\Hafez\settings.json.
    /// </summary>
    public static class ThemeService
    {
        public static bool IsDarkMode { get; private set; }

        public static void ApplySavedTheme()
        {
            AppSettings settings = AppSettingsService.Load();
            ApplyTheme(settings.DarkMode, save: false);
        }

        public static void ToggleTheme()
        {
            ApplyTheme(!IsDarkMode, save: true);
        }

        public static void ApplyTheme(bool darkMode, bool save = true)
        {
            IsDarkMode = darkMode;
            var paletteHelper = new PaletteHelper();
            ITheme theme = paletteHelper.GetTheme();
            theme.SetBaseTheme(darkMode ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);

            if (save)
            {
                AppSettings settings = AppSettingsService.Load();
                settings.DarkMode = darkMode;
                AppSettingsService.Save(settings);
            }
        }
    }
}
