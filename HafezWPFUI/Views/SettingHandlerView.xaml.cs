using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using HafezLibrary.Controllers;
using HafezLibrary.DataAccess.Processor;
using HafezLibrary.Models;
using HafezWPFUI.Helper.Handlers;
using static HafezWPFUI.GlobalConfig;
using RadioButton = System.Windows.Controls.RadioButton;

namespace HafezWPFUI.Views
{
    public partial class SettingHandlerView
    {
        public SettingHandlerView()
        {
            InitializeComponent();
        }

        private void OnloadAllToggleButton(object sender, RoutedEventArgs e)
        {
            LoadToggleButton(sender);
        }

        private void SettingHandlerUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            var propertyName = "QuranAnimationMode";
            var value = propertyName.GetProperty();
            var radioButtonName = $"RadioButton{propertyName}{value}";

            ((RadioButton) FindName(radioButtonName)).IsChecked = true;

            propertyName = "MafatihAnimationMode";
            value = propertyName.GetProperty();
            radioButtonName = $"RadioButton{propertyName}{value}";

            ((RadioButton) FindName(radioButtonName)).IsChecked = true;

            TxtPortNumber.Text = UserConfig.PortNumber.ToString();
        }

        private void ToggleButtonScreenSaver_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var isChecked = (bool)ToggleButtonScreenSaver.IsChecked;
                var screenSaverStatue = isChecked ? "E" : "D";

                // Update DB
                UserConfigModel.Update(new Dictionary<string, string> { { "ScreenSaver", screenSaverStatue } });

                // Update GlobalConfig
                UserConfig.ScreenSaver = screenSaverStatue;

                Output.ScreenSaverManager();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonNotificationPanelTitleVisibility_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var senderToggleButton = sender as ToggleButton;
                var panelName = GetPanelName(Convert.ToInt32(senderToggleButton.Uid));
                var isChecked = (bool)senderToggleButton.IsChecked;
                var titleUpdate = isChecked ? "E" : "D";
                var propertyName = senderToggleButton.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, titleUpdate } });

                //GlobalConfig update
                propertyName.SetProperty(titleUpdate);

                Output.TitleBoxVisibilityToggle(panelName,
                    isChecked ? Visibility.Visible : Visibility.Collapsed);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonLogoStart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var isChecked = (bool)ToggleButtonLogoStart.IsChecked;
                var logoPictureUpdate = isChecked ? "E" : "D";

                UserConfigModel.Update(new Dictionary<string, string> { { "LogoStart", logoPictureUpdate } });

                UserConfig.LogoStart = logoPictureUpdate;

                /* if (GlobalConfig.LogoStart == "D")
                    parent.Output.HideLogo();
                 else
                     parent.Output.ShowLogo();*/
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonNotificationAnimationMode_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var isChecked = (bool)ToggleButtonNotificationAnimationMode.IsChecked;
                var continuousNotificationUpdate = isChecked ? "E" : "D";

                //DB update
                UserConfigModel.Update(new Dictionary<string, string>
                    {{"NotificationAnimationMode", continuousNotificationUpdate}});

                //GlobalConfig update
                UserConfig.NotificationAnimationMode = continuousNotificationUpdate;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void RadioButtonAnimationMode_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                var senderRadioButton = sender as RadioButton;
                var value = senderRadioButton.Name.Substring(11).Substring(18);

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { "QuranAnimationMode", value } });

                //GlobalConfig update
                UserConfig.QuranAnimationMode = value;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void TxtPortNumber_OnLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            try
            {
                if ( string.IsNullOrEmpty(TxtPortNumber.Text) )
                    return;

                var propertyName = TxtPortNumber.GetPropertyName();
                var value = Convert.ToInt32(TxtPortNumber.Text);

                //DB Update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, value.ToString() } });

                //GlobalConfig update
                propertyName.SetProperty(value);

                //set value
                //GlobalConfig.Listener.Server
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonExportUserConfig_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //var userConfigTable = UserConfigController.GetAllUserConfig();
                var configList = UserConfigController.GetAllUserConfigList();

                using ( var sfd = new SaveFileDialog { FileName = "خروجی اطلاعات.csv" } )
                {
                    if ( sfd.ShowDialog() == DialogResult.OK )
                        //userConfigTable.SaveToUserConfigFile(sfd.FileName);
                        TextConnectorProcessor.SaveToTextFile(configList, sfd.FileName);
                }

                MessageBoxHandler.ShowMessage("ثبت", "خروجی از اطلاعات با موفقیت انجام شد.");
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonImportUserConfig_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using ( var ofd = new OpenFileDialog() )
                {
                    if ( ofd.ShowDialog() != DialogResult.OK )
                        return;

                    var userConfigModels = TextConnectorProcessor.LoadFromTextFile<UserConfigModel>(ofd.FileName);
                    UserConfigController.ImportUserConfig(userConfigModels.First());
                }

                MessageBoxHandler.ShowMessage("ثبت", "اطلاعات با موفقیت وارد شد.\nبرای دیدن تغییرات برنامه را ری استارت کنید.");
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ToggleButtonIsListenerAutoStart_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var isChecked = (bool)ToggleButtonIsListenerAutoStart.IsChecked;
                var value = isChecked ? "E" : "D";

                //DB update
                UserConfigModel.Update(new Dictionary<string, string>
                    {{"IsListenerAutoStart", value}});

                //GlobalConfig update
                UserConfig.IsListenerAutoStart = value;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        /*private void OnloadAllToggleButton_New(object sender, RoutedEventArgs e)
        {
            GlobalConfig.LoadToggleButton_New(sender);
        }*/
    }
}