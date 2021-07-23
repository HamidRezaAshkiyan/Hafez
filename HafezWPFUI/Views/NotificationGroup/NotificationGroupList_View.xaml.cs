using BespokeFusion;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using HafezLibrary.Models;
using HafezWPFUI.Helper;
using System.Collections;
using System.Collections.Generic;
using static HafezLibrary.Controllers.NotificationController;
using static HafezLibrary.Controllers.NotificationGroupController;
using static HafezWPFUI.GlobalConfig;
using static HafezWPFUI.Helper.Handlers.MessageBoxHandler;
using Button = System.Windows.Controls.Button;
using ContextMenu = System.Windows.Controls.ContextMenu;

namespace HafezWPFUI.Views.NotificationGroup
{
    public partial class NotificationGroupList_View
    {
        public NotificationGroupList_View()
        {
            InitializeComponent();

            FillListView(GetAllNotificationGroup_Obsolete());
        }

        private void MenuItemNewNotificationGroup_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Main.AddNotificationGroup.IsEditMode = false;
                Main.AddNotificationGroup.Visibility = Visibility.Visible;
                Visibility                           = Visibility.Collapsed;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void MenuItemEditNewNotificationGroup_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel notificationGroup =
                    NotificationGroupListView.SelectedItem as NotificationGroupModel;

                Main.AddNotificationGroup.IsEditMode = true;
                Main.AddNotificationGroup.FillControls(notificationGroup);
                Main.AddNotificationGroup.Visibility = Visibility.Visible;
                Visibility                           = Visibility.Collapsed;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void CreateRightClickMenu(object sender, MouseButtonEventArgs e)
        {
            if ( !(FindResource("RightClickMenu") is ContextMenu cm) )
            {
                return;
            }

            cm.PlacementTarget = sender as Button;
            cm.IsOpen          = true;
        }

        private void ButtonRemoveNotificationGroup_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                using CustomMaterialMessageBox messageBox = GetMaterialMessageBox("هشدار",
                    "تمامی اعلانات این گروه حذف خواهند شد. ایا مطمئنید؟");
                messageBox.BtnOk.Uid   =  ((Button) sender).Uid;
                messageBox.BtnOk.Click += BtnOkOnClick;
                messageBox.Show();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void BtnOkOnClick(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            NotificationGroupModel selectedNotificationGroup =
                new NotificationGroupModel {Id = Convert.ToInt32(button.Uid)};

            RemoveNotificationGroup(selectedNotificationGroup);

            ShowMessage("حذف", "گروه با موفقیت حذف شد");

            FillListView(GetAllNotificationGroup_Obsolete());
            Main.AddNotification.FillGroupNameComboBox();
        }

        #region Methods

        public void FillListView(DataTable notificationTable)
        {
            if ( notificationTable == null )
            {
                throw new ArgumentNullException();
            }

            NotificationGroupListView.Items.Clear();
            foreach ( DataRow dtListRow in notificationTable.Rows )
            {
                NotificationGroupModel newItem = new NotificationGroupModel
                {
                    Id = int.Parse($"{dtListRow["Id"]}"), Name = $"{dtListRow["Name"]}"
                };

                NotificationGroupListView.Items.Add(newItem);
            }
        }

        #endregion

        private void MenuItemShowNotification_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                IEnumerable            itemCollection = Main.NotificationList.NotificationListView.ItemsSource;
                NotificationGroupModel selectedItem = NotificationGroupListView.SelectedItem as NotificationGroupModel;

                List<NotificationModel> selectedGroupNotification = itemCollection.Cast<NotificationModel>()
                    .Where(item => item.GroupId == selectedItem.Id).ToList();

                //parent.NotificationList.NotificationListView.Items.Clear();
                Main.NotificationList.NotificationListView.ItemsSource = selectedGroupNotification;
                Main.NotificationList.SelectedNotificationGroup        = selectedItem;
                Main.NotificationList.Visibility                       = Visibility.Visible;
                Visibility                                             = Visibility.Collapsed;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private async void MenuItemImportGroupNotifications_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel selectedNotificationGroup =
                    NotificationGroupListView.SelectedItem as NotificationGroupModel;

                using OpenFileDialog fileDialog =
                    new OpenFileDialog {Filter = Properties.Resources.Excel_Document_Avalabale_Format};
                DialogResult result = fileDialog.ShowDialog();

                if ( result == DialogResult.OK )
                {
                    //var filePath = fileDialog.FileName;

                    await NotificationGroupHelper.ImportGroupNotificationData(selectedNotificationGroup,
                                                                              fileDialog.FileName);

                    /*var notificationModels = ExcelConnector.ImportFromExcelToList(filePath);

                        foreach (var notificationModel in notificationModels)
                        {
                            notificationModel.GroupId = selectedNotificationGroup.Id;
                            notificationModel.Name = selectedNotificationGroup.Name;
                            notificationModel.NotificationType = 'L';
                        }

                        NotificationController.ImportNotificationListData(notificationModels);*/
                }

                ShowMessage("موفقیت", "اعلانات با موفقیت وارد گروه شدند.");
            }
            catch ( Exception exception )
            {
                ShowMessage("خطا", exception.Message);

                LogInformation(exception);
            }
        }

        private void MenuItemExportGroupNotifications_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel selectedNotificationGroup =
                    NotificationGroupListView.SelectedItem as NotificationGroupModel;
                List<NotificationModel> notificationList = Main
                                                           .NotificationList.NotificationListView.Items
                                                           .Cast<NotificationModel>()
                                                           .Where(x => x.GroupId == selectedNotificationGroup.Id)
                                                           .ToList();

                NotificationHelper.ExportListToExcel(notificationList);


                ShowMessage("موفقیت", "خروجی اعلانات در مسیر مشخص شده با موفقیت گرفته شد.");
            }
            catch ( Exception exception )
            {
                ShowMessage("خطا", exception.Message);
                //Console.WriteLine(exception.StackTrace);

                LogInformation(exception);
            }
        }

        private async void MenuItemRemoveSortIds_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel selectedNotificationGroup =
                    NotificationGroupListView.SelectedItem as NotificationGroupModel;

                RemoveNotificationGroupSortIds(selectedNotificationGroup);

                Main.NotificationList.NotificationListView.ItemsSource =
                    await Main.NotificationList.GetNotificationListByType();
                Main.NotificationList.NetNotificationListView.ItemsSource =
                    await Main.NotificationList.GetNotificationListByType('N');
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void MenuItemShowGroupNotifications_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel selectedNotificationGroup =
                    NotificationGroupListView.SelectedItem as NotificationGroupModel;
                DataTable notificationsTable = GetNotificationsByGroupId_Obsolete(selectedNotificationGroup.Id);

                ChangeNotificationTableHeader(notificationsTable);
                SendToOutput(selectedNotificationGroup, notificationsTable);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private void MenuItemShowGroupSortedNotifications_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                NotificationGroupModel selectedNotificationGroup = GetSelectedNotificationGroup(sender);
                DataTable notificationsTable = GetSortedNotificationsByGroupId_Obsolete(selectedNotificationGroup.Id);

                if ( notificationsTable.Rows.Count == 0 )
                {
                    ShowMessage("خطا",
                                "در این گروه هیچ اولویتی انتخاب نشده است بنابراین از اجرای بدون اولویت در راست کلیک استفاده کنید.");
                    return;
                }

                ChangeNotificationTableHeader(notificationsTable);
                SendToOutput(selectedNotificationGroup, notificationsTable);
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public static void SendToOutput(NotificationGroupModel selectedNotificationGroup, DataTable notificationsTable)
        {
            string panelName = Enums.PanelTypes.Notification.ToString();
            Output.ReplaceTitleText(panelName, selectedNotificationGroup.Name);
            Output.ShowTableInPanel(panelName, notificationsTable);
            // Output.Show();
        }

        private NotificationGroupModel GetSelectedNotificationGroup(object sender)
        {
            NotificationGroupModel selectedNotificationGroup;
            if ( sender is Button button )
            {
                selectedNotificationGroup = new NotificationGroupModel {Id = Convert.ToInt32(button.Uid)};

                selectedNotificationGroup = NotificationGroupListView.Items
                                                                     .Cast<NotificationGroupModel>()
                                                                     .First(item => item.Id == selectedNotificationGroup
                                                                                .Id);
            }
            else
            {
                selectedNotificationGroup = NotificationGroupListView.SelectedItem as NotificationGroupModel;
            }

            return selectedNotificationGroup;
        }
    }
}