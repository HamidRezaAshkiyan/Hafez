using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HafezLibrary.Models;
using static HafezWPFUI.Helper.Handlers.ColorHandler;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views
{
    public partial class ColorHandlerView
    {
        // public static readonly List<int> AvailableStrokeSizes = Enumerable.Range(0, 100).ToList();

        public static List<string> AvailableColors { get; }
            = typeof(Brushes)
                .GetProperties(BindingFlags.Static | BindingFlags.Public)
                .Select(x => x.Name).ToList();

        public ColorHandlerView()
        {
            InitializeComponent();

            DataContext = AvailableColors;

            //bind size
            // ComboBoxQuranTitleStrokeSize.ItemsSource = AvailableStrokeSizes;
            // ComboBoxQuranPanelStrokeSize.ItemsSource = AvailableStrokeSizes;
            // ComboBoxMafatihTitleStrokeSize.ItemsSource = AvailableStrokeSizes;
            // ComboBoxMafatihPanelStrokeSize.ItemsSource = AvailableStrokeSizes;
        }

        private void OnLoadAllComBox(object sender, RoutedEventArgs e)
        {
            LoadComboBox(sender);
        }

        private void ComboBoxForeground_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var foreground = senderComboBox.SelectedItem.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB Update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, foreground } });

                //GlobalConfig update
                propertyName.SetProperty(foreground);

                //set value
                SetColor(propertyName, foreground);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxBackground_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var panelBackground = senderComboBox.SelectedItem.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, panelBackground } });

                //GlobalConfig update
                propertyName.SetProperty(panelBackground);

                //Set value
                SetColor(propertyName, panelBackground);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxStrokeColor_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var strokeColor = senderComboBox.SelectedItem.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, strokeColor } });

                //GlobalConfig update
                propertyName.SetProperty(strokeColor);

                //Set value
                SetColor(propertyName, strokeColor);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxStrokeSize_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var strokeSize = senderComboBox?.SelectedItem.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, strokeSize } });

                //GlobalConfig update
                propertyName.SetProperty(strokeSize);

                //Set value
                SetStrokeSize(propertyName, Convert.ToDouble(strokeSize));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}