using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using HafezLibrary.Models;

namespace HafezLibrary.Settings
{
    public class ScenePreset
    {
        public string Name { get; set; } = string.Empty;
        public bool ShowQuran { get; set; }
        public bool ShowMafatih { get; set; }
        public bool ShowNotifications { get; set; }
        public bool ShowCamera { get; set; }
        public bool ShowPrayerBanner { get; set; } = true;
    }

    public class AppSettings
    {
        public int SettingsVersion { get; set; } = 2;
        public bool DarkMode { get; set; }
        public int ApiPort { get; set; } = 5050;
        public bool AutoStartApiServer { get; set; } = true;
        public bool AutoAssignSecondaryMonitor { get; set; } = true;
        public string CityName { get; set; } = "تهران";
        public double Latitude { get; set; } = 35.6892;
        public double Longitude { get; set; } = 51.3890;
        public double TimeZoneOffsetHours { get; set; } = 3.5;
        public int SlideShowSeconds { get; set; } = 15;
        public int LogoShowSeconds { get; set; } = 15;
        public UserConfigModel DisplayConfig { get; set; } = new();
        public List<ScenePreset> ScenePresets { get; set; } = new()
        {
            new ScenePreset { Name = "نماز جمعه", ShowQuran = true, ShowNotifications = true, ShowPrayerBanner = true },
            new ScenePreset { Name = "دعای کمیل", ShowMafatih = true, ShowNotifications = false, ShowPrayerBanner = true },
            new ScenePreset { Name = "سخنرانی زنده", ShowCamera = true, ShowNotifications = true, ShowPrayerBanner = true }
        };
    }

    /// <summary>
    /// Versioned JSON settings persisted to %AppData%\Hafez\settings.json.
    /// </summary>
    public static class AppSettingsService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        private static string _overrideSettingsPath;

        public static string GetSettingsPath()
        {
            if (!string.IsNullOrWhiteSpace(_overrideSettingsPath))
            {
                return _overrideSettingsPath;
            }

            string dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Hafez");
            return Path.Combine(dir, "settings.json");
        }

        public static void OverrideSettingsPathForTesting(string path)
        {
            _overrideSettingsPath = path;
        }

        public static AppSettings Load()
        {
            string path = GetSettingsPath();
            if (!File.Exists(path))
            {
                var defaults = new AppSettings();
                Save(defaults);
                return defaults;
            }

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<AppSettings>(json, JsonOptions) ?? new AppSettings();
        }

        public static void Save(AppSettings settings)
        {
            string path = GetSettingsPath();
            string dir = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(path, json);
        }

        public static void Export(AppSettings settings, string exportPath)
        {
            string json = JsonSerializer.Serialize(settings, JsonOptions);
            File.WriteAllText(exportPath, json);
        }

        public static AppSettings Import(string importPath)
        {
            string json = File.ReadAllText(importPath);
            AppSettings imported = JsonSerializer.Deserialize<AppSettings>(json, JsonOptions)
                ?? throw new InvalidDataException("Invalid settings JSON file.");
            Save(imported);
            return imported;
        }
    }
}
