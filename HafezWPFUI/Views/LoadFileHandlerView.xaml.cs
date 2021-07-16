using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using HafezLibrary.Models;
using HafezWPFUI;
using HafezWPFUI.Helper.Handlers;

namespace HafezWPFUI.Views
{
    public partial class LoadFileHandlerView
    {
        private string AppLogoFolderName { get; } = @"Gallery\CompanyLogo";

        public LoadFileHandlerView()
        {
            InitializeComponent();
        }

        private void TxtBoxLoad(object sender, RoutedEventArgs e)
        {
            GlobalConfig.LoadTextBox(sender);
        }

        private static string GetDestinationPath(string filename, string folderName)
        {
            var appStartPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule?.FileName);

            appStartPath = string.Format(appStartPath + "\\{0}\\" + filename, folderName);
            return appStartPath;
        }

        private void BrowseSmallLogo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var ofd = new OpenFileDialog();
                if ( ofd.ShowDialog() == true )
                {
                    TxtPathSmallLogo.Text = ofd.FileName;
                    ImgInputSmallLogo.Source = new BitmapImage(new Uri(ofd.FileName));
                }
            }
            catch ( Exception exception )
            {
                GlobalConfig.LogInformation(exception);
            }
        }

        private void BrowseFullLogo_Click(object sender, RoutedEventArgs e)
        {
            var ofd = new OpenFileDialog();
            if ( ofd.ShowDialog() == true )
            {
                TxtPathFullLogo.Text = ofd.FileName;
                ImgInputFullLogo.Source = new BitmapImage(new Uri(ofd.FileName));

                //var parent = this.TryFindParent<MainWindow>();
                //parent.OutputWindow.LogoScreenSaver.Source = new BitmapImage(new Uri(ofd.FileName));
                //parent.OutputWindow.Logo.Source = new BitmapImage(new Uri(ofd.FileName));
            }
        }


        public void SaveImage(string sourceFileName)
        {
            try
            {
                var imageName = Path.GetFileName(sourceFileName);
                var destinationPath = GetDestinationPath(imageName, AppLogoFolderName);

                if ( !File.Exists(destinationPath) )
                    File.Copy(sourceFileName, destinationPath, true);
            }
            catch ( Exception e )
            {
                Console.WriteLine(e);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var updateDictionary = new Dictionary<string, string>();

                if ( !string.IsNullOrEmpty(TxtPathSmallLogo.Text) )
                {
                    SaveImage(TxtPathSmallLogo.Text);
                    var panelImageName = Path.GetFileName(TxtPathSmallLogo.Text);

                    updateDictionary.Add("PanelImageLocation", panelImageName);
                }

                if ( !string.IsNullOrEmpty(TxtPathFullLogo.Text) )
                {
                    SaveImage(TxtPathFullLogo.Text);
                    var screenSaverImageLocationName = Path.GetFileName(TxtPathFullLogo.Text);

                    updateDictionary.Add("ScreenSaverImageLocation", screenSaverImageLocationName);
                }

                if ( !string.IsNullOrEmpty(TxtCompanyName.Text) )
                {
                    GlobalConfig.UserConfig.CompanyName = TxtCompanyName.Text;

                    updateDictionary.Add("CompanyName", GlobalConfig.UserConfig.CompanyName);
                }

                UserConfigModel.Update(updateDictionary);
                MessageBoxHandler.ShowMessage("تایید", "ذخیره انجام شد\n برای دیدن تغییرات برنامه را دوباره راه اندازی کنید.");
            }
            catch ( Exception exception )
            {
                GlobalConfig.LogInformation(exception);
            }
        }
    }
}