using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezWPFUI.Helper;
using HafezWPFUI.Models;
using HafezWPFUI.Views.Windows;

namespace HafezWPFUI
{
    public static class GlobalConfig
    {
        public static string ImageLocation { get; } =
            AppDomain.CurrentDomain.BaseDirectory + @"Gallery\Album\";

        public static string LogoLocation { get; } =
            AppDomain.CurrentDomain.BaseDirectory + @"Gallery\Logo\";

        public static MainWindowView     Main       { get; set; }
        public static OutputWindowView   Output     { get; } = new();
        public static ListenerWindowView Listener   { get; } = new();
        public static UserConfigModel    UserConfig { get; private set; }

        public static List<PersonalDuaListDisplayModel> PersonalDuaListsDisplay { get; private set; } =
            UpdatePersonalDuaListsDisplay();

        public static List<PersonalDuaListDisplayModel> UpdatePersonalDuaListsDisplay()
        {
            DbCache.UpdatePersonalDuaLists();

            PersonalDuaListsDisplay = Bootstrapper
                                      .ConfigureAutoMapper()
                                      .Map<List<PersonalDuaListDisplayModel>>(DbCache.PersonalDuaLists);

            int count = PersonalDuaListsDisplay.Count();
            for ( int i = 0; i < count; i++ )
            {
                PersonalDuaListsDisplay[i].ListIndex = i + 1;
            }

            return PersonalDuaListsDisplay;
        }

        public static int EnabledServiceCount { get; set; } = 0;

        private static string _portInput;

        public static string PortInput
        {
            get => _portInput;

            set
            {
                _portInput = value;
                _          = NetCommandProcessor.DoCommandAsync(NetCommandProcessor.ParseInput(value));
            }
        }

        public static object GetProperty(this string propertyName)
        {
            return UserConfig.GetProperty(propertyName);
        }

        public static void SetProperty(this string propertyName, object value)
        {
            UserConfig.SetProperty(propertyName, value);
        }

        public static void GetGlobalUserConfig()
        {
            UserConfig = UserConfigModel.GetAllUserConfig();
        }

        public static void LoadComboBox(object sender)
        {
            ComboBox senderCombobox = (ComboBox) sender;
            object   value          = GetProperty(senderCombobox.GetPropertyName());

            senderCombobox.Text = value.ToString();
        }

        public static void LoadTextBox(object sender)
        {
            TextBox senderTextBox = (TextBox) sender;
            object  value         = GetProperty(senderTextBox.GetPropertyName());

            senderTextBox.Text = value.ToString();
        }

        public static void LoadToggleButton(object sender)
        {
            ToggleButton senderToggleButton = (ToggleButton) sender;
            object       value              = GetProperty(senderToggleButton.GetPropertyName());

            bool isChecked = value.ToString() != "D";
            senderToggleButton.IsChecked = isChecked;
        }

        /*internal static void LoadToggleButton_New(object sender)
        {
            var senderToggleButton = (ToggleButton)sender;
            var value = (bool)senderToggleButton.GetPropertyName().GetProperty();

            senderToggleButton.IsChecked = value;
        }*/

        public static string GetPanelName(int panelIndex)
        {
            return panelIndex switch
            {
                1 => Enums.PanelTypes.Notification.ToString(),
                2 => Enums.PanelTypes.Quran.ToString(),
                3 => Enums.PanelTypes.Mafatih.ToString(),
                4 => Enums.PanelTypes.QuranTranslation.ToString(),
                5 => Enums.PanelTypes.MafatihTranslation.ToString(),

                _ => null
            };
        }

        public static string GetPropertyName(this ComboBox input)
        {
            return input.Name.Substring(8);
        }

        public static string GetPropertyName(this TextBox input)
        {
            return input.Name.Substring(3);
        }

        public static string GetPropertyName(this ToggleButton input)
        {
            return input.Name.Substring(12);
        }

        public static List<string> GetGalleryFullPath(this string folderName)
        {
            //var filesList = Directory.GetFiles(ImageLocation + folderName, "*",
            //    SearchOption.AllDirectories).ToList();

            List<string> filters = new List<string>
            {
                "jpg",
                "jpeg",
                "png",
                "tiff",
                "bmp",
                "svg"
            };
            List<string> output = GetFilesFrom(ImageLocation + folderName, filters, false);

            return output;
        }

        public static List<string> GetLogoFullPath(this string folderName)
        {
            List<string> filters = new List<string> {"png"};
            List<string> output  = GetFilesFrom(LogoLocation + folderName, filters, false);

            return output;
        }

        public static List<string> GetFilesFrom(string searchFolder, IEnumerable<string> filters, bool isRecursive)
        {
            List<string> filesFound   = new List<string>();
            SearchOption searchOption = isRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach ( string filter in filters )
            {
                filesFound.AddRange(Directory.GetFiles(searchFolder, $"*.{filter}", searchOption));
            }

            return filesFound.ToList();
        }

        public static void LogInformation(Exception exception)
        {
            string logInformation = $"Error Message: {exception.Message}\n Error Location: {exception.StackTrace}";

            //Console.WriteLine(logInformation); 
            Log.Warning(logInformation);
        }
    }
}