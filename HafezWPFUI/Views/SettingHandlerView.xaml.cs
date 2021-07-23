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
            string propertyName    = "QuranAnimationMode";
            object value           = propertyName.GetProperty();
            string radioButtonName = $"RadioButton{propertyName}{value}";

            ((RadioButton) FindName(radioButtonName)).IsChecked = true;

            propertyName    = "MafatihAnimationMode";
            value           = propertyName.GetProperty();
            radioButtonName = $"RadioButton{propertyName}{value}";

            ((RadioButton) FindName(radioButtonName)).IsChecked = true;

            TxtPortNumber.Text = UserConfig.PortNumber.ToString();
        }

        private void ToggleButtonScreenSaver_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                bool   isChecked         = (bool) ToggleButtonScreenSaver.IsChecked;
                string screenSaverStatue = isChecked ? "E" : "D";

                // Update DB
                UserConfigModel.Update(new Dictionary<string, string> {{"ScreenSaver", screenSaverStatue}});

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
                ToggleButton senderToggleButton = sender as ToggleButton;
                string       panelName          = GetPanelName(Convert.ToInt32(senderToggleButton.Uid));
                bool         isChecked          = (bool) senderToggleButton.IsChecked;
                string       titleUpdate        = isChecked ? "E" : "D";
                string       propertyName       = senderToggleButton.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> {{propertyName, titleUpdate}});

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
                bool   isChecked         = (bool) ToggleButtonLogoStart.IsChecked;
                string logoPictureUpdate = isChecked ? "E" : "D";

                UserConfigModel.Update(new Dictionary<string, string> {{"LogoStart", logoPictureUpdate}});

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
                bool   isChecked                    = (bool) ToggleButtonNotificationAnimationMode.IsChecked;
                string continuousNotificationUpdate = isChecked ? "E" : "D";

                //DB update
                UserConfigModel.Update(new Dictionary<string, string>
                {
                    {"NotificationAnimationMode", continuousNotificationUpdate}
                });

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
                RadioButton senderRadioButton = sender as RadioButton;
                string      value             = senderRadioButton.Name.Substring(11).Substring(18);

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> {{"QuranAnimationMode", value}});

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
                {
                    return;
                }

                string propertyName = TxtPortNumber.GetPropertyName();
                int    value        = Convert.ToInt32(TxtPortNumber.Text);

                //DB Update
                UserConfigModel.Update(new Dictionary<string, string> {{propertyName, value.ToString()}});

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
                List<UserConfigModel> configList = UserConfigController.GetAllUserConfigList();

                using ( SaveFileDialog sfd = new SaveFileDialog {FileName = "خروجی اطلاعات.csv"} )
                {
                    if ( sfd.ShowDialog() == DialogResult.OK )
                        //userConfigTable.SaveToUserConfigFile(sfd.FileName);
                    {
                        TextConnectorProcessor.SaveToTextFile(configList, sfd.FileName);
                    }
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
                using ( OpenFileDialog ofd = new OpenFileDialog() )
                {
                    if ( ofd.ShowDialog() != DialogResult.OK )
                    {
                        return;
                    }

                    List<UserConfigModel> userConfigModels =
                        TextConnectorProcessor.LoadFromTextFile<UserConfigModel>(ofd.FileName);
                    UserConfigController.ImportUserConfig(userConfigModels.First());
                }

                MessageBoxHandler.ShowMessage("ثبت",
                                              "اطلاعات با موفقیت وارد شد.\nبرای دیدن تغییرات برنامه را ری استارت کنید.");
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
                bool   isChecked = (bool) ToggleButtonIsListenerAutoStart.IsChecked;
                string value     = isChecked ? "E" : "D";

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> {{"IsListenerAutoStart", value}});

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