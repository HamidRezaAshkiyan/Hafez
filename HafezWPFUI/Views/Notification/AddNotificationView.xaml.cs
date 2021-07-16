using MahApps.Metro.Controls;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using HafezLibrary.Models;
using HafezLibrary.Validators;
using HafezWPFUI.Helper.Handlers;
using HafezWPFUI.Views.Windows;
using static HafezLibrary.Controllers.NotificationController;
using static HafezLibrary.Controllers.NotificationGroupController;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views.Notification
{
    public partial class AddNotificationView
    {
        public AddNotificationView()
        {
            InitializeComponent();

            FillGroupNameComboBox();

            //DataContext = Notification;
        }

        private bool _isEditMode;
        public NotificationGroupModel SelectedNotificationGroup { get; set; }

        public char TypeOfNotification { get; set; } = 'L';
        //private NotificationModel Notification { get; set; } = new NotificationModel();

        public bool IsEditMode
        {
            private get => _isEditMode;
            set
            {
                _isEditMode = value;

                if ( value )
                {
                    //TxtNotificationId.Visibility = Visibility.Visible;
                    BtnSubmit.Content = "ویرایش اعلان";
                }
                else
                {
                    //TxtNotificationId.Visibility = Visibility.Collapsed;
                    BtnSubmit.Content = "ثبت اعلان";
                }
            }
        }

        #region Methods

        public void FillControlsByNotificationModel(NotificationModel notification)
        {
            TxtNotificationId.Text = notification.Id.ToString();
            TxtNotificationName.Text = notification.Name;
            TxtNotificationDescription.Text = notification.Description;
            TxtNotificationSortId.Text = notification.SortId.ToString();

            //ComboBoxGroupName.SelectedItem = ComboBoxGroupName.Items.Cast<NotificationGroupModel>()
            ComboBoxGroupName.SelectedItem = ComboBoxGroupName.Items.Cast<NotificationGroupModel>()
                .First(x => x.Id == notification.GroupId);

            /*foreach (NotificationGroupModel notificationGroup in ComboBoxGroupName.Items)
                if (notificationGroup.Id == notification.GroupId)
                {
                    ComboBoxGroupName.SelectedItem = notificationGroup;
                    break;
                }*/
        }

        public void FillGroupNameComboBox()
        {
            var notificationGroups = GetAllNotificationGroup_Obsolete();

            ComboBoxGroupName.Items.Clear();
            foreach ( DataRow notificationGroupsRow in notificationGroups.Rows )
            {
                var notificationGroup = new NotificationGroupModel
                {
                    Id = Convert.ToInt32(notificationGroupsRow["Id"]),
                    Name = notificationGroupsRow["Name"].ToString()
                };

                ComboBoxGroupName.Items.Add(notificationGroup);
            }
        }

        #endregion

        #region Events

        private async void BtnSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if ( !ValidateForm() )
                {
                    MessageBoxHandler.ShowMessage("خطا", "لطفا تمامی فیلدها را پر کنید.");
                    return;
                }

                //Create Model
                var notification = new NotificationModel
                {
                    NotificationType = TypeOfNotification,
                    Description = TxtNotificationDescription.Text,
                    Name = TxtNotificationName.Text,
                    SortId = Convert.ToInt32(TxtNotificationSortId.Text),
                    GroupId = ((NotificationGroupModel)ComboBoxGroupName.SelectedItem).Id
                };
                if ( IsEditMode )
                    notification.Id = Convert.ToInt32(TxtNotificationId.Text);

                #region Test

                if ( notification.GroupId != 2 )
                {
                    TypeOfNotification = (char) Enums.NotificationType.Local;
                    notification.NotificationType = TypeOfNotification;
                }

                #endregion

                //Validate Data
                var result = await new NotificationValidator().ValidateAsync(notification);
                if ( result.IsValid == false )
                {
                    MessageBoxHandler.ShowMessage("خطا", result.Errors[0].ErrorMessage);
                    return;
                }

                if ( IsEditMode )
                {
                    UpdateNotification(notification);
                    MessageBoxHandler.ShowMessage("ویرایش", "اعلان با موفقیت ویرایش شد");
                }
                else
                {
                    CreateSimpleNotification(notification);
                    MessageBoxHandler.ShowMessage("ثبت", "اعلان با موفقیت ثبت شد");
                }

                Main.NotificationList.Visibility = Visibility.Visible;
                Main.NotificationList.SelectedNotificationGroup = SelectedNotificationGroup;
                Visibility = Visibility.Collapsed;

                //UPDATE BINDING
                //todo update with adding to binding source
                // test with observation collection for binding

                /*if (TypeOfNotification == (char)NotificationType.Local)
                {
                    //Main.NotificationList.LocalNotifications.Add(notification);

                    Main.NotificationList.NotificationListView.ItemsSource =
                        await Main.NotificationList.GetNotificationListByType();
                }
                else
                {
                    Main.NotificationList.NetNotificationListView.ItemsSource =
                        await Main.NotificationList.GetNotificationListByType('N');
                }*/
                Main.NotificationList.NetNotificationListView.ItemsSource =
                    await Main.NotificationList.GetNotificationListByType('N');
                Main.NotificationList.NotificationListView.ItemsSource =
                    await Main.NotificationList.GetNotificationListByType();

                //CLEAR Fields
                TxtNotificationSortId.Text = "0";
                TxtNotificationId.Text = "";
                TxtNotificationDescription.Text = "";
                TypeOfNotification = (char) Enums.NotificationType.Local;
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        private bool ValidateForm()
        {
            var output = !(string.IsNullOrWhiteSpace(TxtNotificationDescription.Text)
                           || string.IsNullOrWhiteSpace(TxtNotificationName.Text)
                           || ComboBoxGroupName.SelectedIndex == -1);

            return output;
        }

        #endregion

        private void ButtonSubmitGroupName_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create Model
                var notificationGroup = new NotificationGroupModel { Name = TxtGroupName.Text };

                //Validate Model
                var result = new NotificationGroupValidator().Validate(notificationGroup);
                if ( result.IsValid == false )
                {
                    MessageBoxHandler.ShowMessage("هشدار", result.Errors[0].ErrorMessage);
                    return;
                }

                //Insert Model
                CreateNotificationGroup(notificationGroup);
                MessageBoxHandler.ShowMessage("ثبت", "گروه اعلان ثبت شد");

                //DELETE Fields
                TxtGroupName.Text = "";

                //UPDATE BINDING
                var parent = this.TryFindParent<MainWindowView>();
                parent.NotificationGroupList.FillListView(GetAllNotificationGroup_Obsolete());
                FillGroupNameComboBox();

                //SET SELECTED
                ComboBoxGroupName.SelectedItem = ComboBoxGroupName.Items.Cast<NotificationGroupModel>().Last();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}