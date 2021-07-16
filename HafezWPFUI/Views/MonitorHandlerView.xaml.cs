using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using HafezLibrary.Models;
using HafezWPFUI.Views.Windows;
using static HafezWPFUI.GlobalConfig;
using ComboBox = System.Windows.Controls.ComboBox;

namespace HafezWPFUI.Views
{
    public partial class MonitorHandlerView
    {
        public MonitorHandlerView()
        {
            InitializeComponent();
            FillMonitorComboBox();
        }

        private void OnLoadAllComBox(object sender, RoutedEventArgs e)
        {
            LoadComboBox(sender);
        }

        private void OnloadAllToggleButton(object sender, RoutedEventArgs e)
        {
            LoadToggleButton(sender);
        }

        private void MonitorHandlerUserControl_OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach ( var panelType in (Enums.PanelTypes[]) Enum.GetValues(typeof(Enums.PanelTypes)) )
            {
                var comboBoxItemName =
                    $"{panelType.ToString()[0]}S{$"{panelType.ToString()}AnimationSpeed".GetProperty()}";
                var comboBoxItem = FindName(comboBoxItemName) as ComboBoxItem;
                ((ComboBox) FindName($"ComboBox{panelType}AnimationSpeed")).SelectedItem =
                    comboBoxItem;
            }
        }

        public void FillMonitorComboBox()
        {
            var allScreensLength = Screen.AllScreens.Length;

            ComboBoxControllerMonitorIndex.Items.Clear();
            ComboBoxOutputMonitorIndex.Items.Clear();
            for ( var i = 0; i < allScreensLength; i++ )
            {
                ComboBoxOutputMonitorIndex.Items.Add($"{i + 1}");
                ComboBoxControllerMonitorIndex.Items.Add($"{i + 1}");
            }
        }

        private void ToggleButtonPanelVerticalAlignment_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var senderToggleButton = sender as ToggleButton;
                var isChecked = (bool)senderToggleButton.IsChecked;
                var panelName = GetPanelName(Convert.ToInt32(senderToggleButton.Uid));
                var panelVerticalAlignment = isChecked ? "U" : "D";
                var alignment = isChecked ? VerticalAlignment.Top : VerticalAlignment.Bottom;
                var propertyName = senderToggleButton.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, panelVerticalAlignment } });

                //GlobalConfig update
                propertyName.SetProperty(panelVerticalAlignment);

                //Set value 
                Output.SetPanelVerticalAlignment(panelName, alignment);
                ComboBoxPanelMargin_OnSelectionChanged(FindName($"ComboBox{panelName}PanelMargin") as ComboBox, null);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxAnimationSpeed_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //todo change time to speed REAL SPEED.
                var senderComboBox = sender as ComboBox;
                var panelName = GetPanelName(Convert.ToInt32(senderComboBox.Uid));
                var speed = ((ComboBoxItem)senderComboBox.SelectedItem).Name.Substring(3);
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, speed } });

                //GlobalConfig update
                propertyName.SetProperty(speed);

                //set value
                Output.RefreshAnimation(panelName);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxPanelMargin_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var senderComboBox = sender as ComboBox;
                var panelName = GetPanelName(Convert.ToInt32(senderComboBox.Uid));
                var panelMargin = ((ComboBoxItem)senderComboBox.SelectedItem).Content.ToString();
                var propertyName = senderComboBox.GetPropertyName();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { propertyName, panelMargin } });

                //GlobalConfig update
                propertyName.SetProperty(panelMargin);

                //Set value
                //Controllers.Program.PanelHandler.SetPanelMargin(panelName);
                Output.SetPanelMargin(panelName,
                    Convert.ToInt32(((ComboBoxItem) senderComboBox.SelectedItem).Name.Substring(2)));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxControllerMonitorIndex_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var parent = this.TryFindParent<MainWindowView>();
                if ( ComboBoxControllerMonitorIndex.SelectedItem == null )
                    return;
                var monitor = ComboBoxControllerMonitorIndex.SelectedItem.ToString();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { "ControllerMonitorIndex", monitor } });

                //GlobalConfig update
                UserConfig.ControllerMonitorIndex = monitor;

                //Set Value
                parent.SetMonitor(Convert.ToInt32(UserConfig.ControllerMonitorIndex));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ComboBoxOutputMonitorIndex_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if ( ComboBoxOutputMonitorIndex.SelectedItem == null )
                    return;
                var monitorNumber = ComboBoxOutputMonitorIndex.SelectedItem.ToString();

                //DB update
                UserConfigModel.Update(new Dictionary<string, string> { { "OutputMonitorIndex", monitorNumber } });

                //GlobalConfig update
                UserConfig.OutputMonitorIndex = monitorNumber;

                //Set value
                Output.SetMonitor(Convert.ToInt32(UserConfig.OutputMonitorIndex));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}