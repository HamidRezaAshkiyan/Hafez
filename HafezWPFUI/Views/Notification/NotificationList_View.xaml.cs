using BespokeFusion;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HafezLibrary.Models;
using HafezWPFUI.Helper.Handlers;
using HafezWPFUI.Views.Windows;
using static HafezLibrary.Controllers.NotificationController;
using static HafezWPFUI.Helper.Handlers.MessageBoxHandler;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views.Notification
{
    public partial class NotificationList_View
    {
        private NotificationGroupModel _selectedNotificationGroup;

        public NotificationGroupModel SelectedNotificationGroup
        {
            get => _selectedNotificationGroup;
            set
            {
                _selectedNotificationGroup = value;
                _                          = SetNotificationGroupNameAsync();
            }
        }
        /*public BindingList<NotificationModel> LocalNotifications { get; set; }
        public BindingList<NotificationModel> NetNotifications { get; set; }*/

        public NotificationList_View()
        {
            InitializeComponent();
        }

        public async Task SetNotificationGroupNameAsync()
        {
            if ( SelectedNotificationGroup != null )
            {
                GroupBoxHeader.Text              = "نام گروه انتخاب شده : " + SelectedNotificationGroup.Name;
                NotificationListView.ItemsSource = await GetNotificationListByType();
            }
            else
            {
                GroupBoxHeader.Text = "لیست اعلانات";
            }
        }

        private async void NotificationList_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                /*LocalNotifications = new BindingList<NotificationModel>(await NotificationController.GetAllNotification());
                NetNotifications = new BindingList<NotificationModel>(await NotificationController.GetAllNotification('N'));

                LocalNotifications.RaiseListChangedEvents = true;
                LocalNotifications.ListChanged += LocalNotificationsOnListChanged;
                NetNotifications.ListChanged += NetNotificationsOnListChanged;

                NotificationListView.ItemsSource = LocalNotifications;
                NetNotificationListView.ItemsSource = NetNotifications;*/

                NotificationListView.ItemsSource = await GetNotificationListByType();
                NetNotificationListView.ItemsSource =
                    new ObservableCollection<NotificationModel>(
                                                                await GetAllNotificationByType('N'));
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        /*private void NetNotificationsOnListChanged(object sender, ListChangedEventArgs e)
        {
            NetNotificationListView.ItemsSource = NetNotifications;
        }

        private void LocalNotificationsOnListChanged(object sender, ListChangedEventArgs e)
        {
            NotificationListView.ItemsSource = LocalNotifications;
        }*/

        public async Task<List<NotificationModel>> GetNotificationListByType(char notificationType = 'L')
        {
            List<NotificationModel> notificationModels = await GetAllNotificationByType(notificationType);

            if ( SelectedNotificationGroup != null )
            {
                notificationModels = notificationModels
                                     .Where(item => item.GroupId == SelectedNotificationGroup.Id).ToList();
            }

            return notificationModels;
        }

        public void MenuItemSelect_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationModel selectedNotification;

                if ( Equals(((MenuItem) sender).Name, "NetShow") )
                {
                    selectedNotification = NetNotificationListView.SelectedItem as NotificationModel;
                }
                else
                {
                    selectedNotification = NotificationListView.SelectedItem as NotificationModel;
                }

                Output.ReplaceTitleText(Enums.PanelTypes.Notification.ToString(), selectedNotification.Name);
                Output.ReplaceOutputTable(Enums.PanelTypes.Notification, selectedNotification.Description);

                // Output.Show();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void MenuItemNewNotification_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindowView parent = this.TryFindParent<MainWindowView>();

                parent.ClearPage();
                parent.AddNotification.IsEditMode              = false;
                parent.AddNotification.Visibility              = Visibility.Visible;
                Main.AddNotification.SelectedNotificationGroup = SelectedNotificationGroup;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void MenuItemEditNotification_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationModel selectedNotification;

                if ( Equals(((MenuItem) sender).Name, "NetEdit") )
                {
                    selectedNotification                    = NetNotificationListView.SelectedItem as NotificationModel;
                    Main.AddNotification.TypeOfNotification = 'N';
                }
                else
                {
                    selectedNotification                    = NotificationListView.SelectedItem as NotificationModel;
                    Main.AddNotification.TypeOfNotification = 'L';
                }

                Main.AddNotification.SelectedNotificationGroup = SelectedNotificationGroup;
                Main.AddNotification.IsEditMode                = true;
                Main.AddNotification.FillControlsByNotificationModel(selectedNotification);

                Main.ClearPage();
                Main.AddNotification.Visibility = Visibility.Visible;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void ButtonRemoveNotification_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                int id = Equals(((MenuItem) sender).Name, "NetDelete")
                             ? ((NotificationModel) NetNotificationListView.SelectedItem).Id
                             : ((NotificationModel) NotificationListView.SelectedItem).Id;

                using CustomMaterialMessageBox messageBox = GetMaterialMessageBox("هشدار",
                    "تمامی اعلانات این گروه حذف خواهند شد. ایا مطمئنید؟");
                messageBox.BtnOk.Uid   =  id.ToString();
                messageBox.BtnOk.Click += BtnOkOnClick;
                messageBox.Show();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private async void BtnOkOnClick(object sender, RoutedEventArgs e)
        {
            Button            button               = sender as Button;
            NotificationModel selectedNotification = new NotificationModel {Id = Convert.ToInt32(button.Uid)};

            selectedNotification = RemoveNotification(selectedNotification);

            if ( selectedNotification.NotificationType == 'L' )
            {
                NotificationListView.ItemsSource = await GetNotificationListByType();
            }
            else
            {
                NetNotificationListView.ItemsSource = await GetNotificationListByType('N');
            }

            ShowMessage("حذف", "اعلان با موفقیت حذف شد");
        }

        private void CreateRightClickMenu(object sender, RoutedEventArgs e)
        {
            string key = "RightClickMenu";
            if ( Equals(sender, NetNotificationListView) )
            {
                key = $"Net{key}";
            }

            if ( !(FindResource(key) is ContextMenu cm) )
            {
                return;
            }

            cm.PlacementTarget = sender as Button;
            cm.IsOpen          = true;
        }

        private void TxtSortId_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox           textBox = (TextBox) sender;
                int               id      = Convert.ToInt32(textBox.Uid);
                NotificationModel model   = FindNotification(NotificationListView, id);

                if ( string.IsNullOrWhiteSpace(textBox.Text) )
                {
                    textBox.Text = "0";
                }

                model.SortId = Convert.ToInt32(textBox.Text);

                UpdateNotification(model);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void TxtNetSortId_OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                TextBox           textBox = (TextBox) sender;
                int               id      = Convert.ToInt32(textBox.Uid);
                NotificationModel model   = FindNotification(NetNotificationListView, id);

                if ( string.IsNullOrWhiteSpace(textBox.Text) )
                {
                    textBox.Text = "0";
                }

                model.SortId = Convert.ToInt32(textBox.Text);

                UpdateNotification(model);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public static NotificationModel FindNotification(ListView list, int id)
        {
            NotificationModel output = list.ItemsSource.Cast<NotificationModel>()
                                           .First(x => x.Id == id);

            return output;
        }

        private void TxtSortId_OnKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = !ControlHandlers.IsKeyNumber(e);
        }
    }
}