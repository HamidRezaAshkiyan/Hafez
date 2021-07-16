using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HafezLibrary.Models;
using static System.Windows.Media.Fonts;
using static HafezWPFUI.Helper.Handlers.FontHandler;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views
{
    public partial class SizeHandlerView
    {
        private ObservableCollection<string> Fonts { get; } =
            new ObservableCollection<string>(SystemFontFamilies.Select(x => x.Source).ToList());

        public SizeHandlerView()
        {
            InitializeComponent();

            DataContext = Fonts;
        }

        private void OnLoadAllComBox(object sender, RoutedEventArgs e)
        {
            LoadComboBox(sender);
        }

        private void SizeHandlerUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            ComboBoxNotificationPanelFontFamily.SelectedItem = UserConfig.NotificationPanelFontFamily;
            ComboBoxNotificationTitleFontFamily.SelectedItem = UserConfig.NotificationTitleFontFamily;
            ComboBoxQuranPanelFontFamily.SelectedItem = UserConfig.QuranPanelFontFamily;
            ComboBoxQuranTitleFontFamily.SelectedItem = UserConfig.QuranTitleFontFamily;
            ComboBoxMafatihPanelFontFamily.SelectedItem = UserConfig.MafatihPanelFontFamily;
            ComboBoxMafatihTitleFontFamily.SelectedItem = UserConfig.MafatihTitleFontFamily;
        }

        private void ComboBoxPanelHeight_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var panelHeight = ((ComboBoxItem) senderComboBox.SelectedItem).Content.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, panelHeight } });

                //GlobalConfig update
                propertyName.SetProperty(panelHeight);

                //Set value
                SetPanelHeight(propertyName, Convert.ToDouble(panelHeight));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxFontSize_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var panelName = GetPanelName(Convert.ToInt32(senderComboBox.Uid));
                var fontSize = ((ComboBoxItem) senderComboBox.SelectedItem).Content.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, fontSize } });

                //GlobalConfig update
                propertyName.SetProperty(fontSize);

                //set value
                SetFontSize(propertyName, Convert.ToDouble(fontSize));

                Output.RefreshAnimation(panelName);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxFontFamily_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var panelName = GetPanelName(Convert.ToInt32(senderComboBox.Uid));
                if ( senderComboBox.SelectedItem == null )
                    return;
                var familyName = senderComboBox.SelectedItem.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, familyName } });

                //GlobalConfig update
                propertyName.SetProperty(familyName);

                //Set value
                SetFontFamily(propertyName, new FontFamily(familyName));

                //refresh animation
                Output.RefreshAnimation(panelName);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}