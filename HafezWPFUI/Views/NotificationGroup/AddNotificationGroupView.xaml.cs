using System;
using System.Windows;
using HafezLibrary.Models;
using HafezLibrary.Validators;
using HafezWPFUI.Helper.Handlers;
using static HafezLibrary.Controllers.NotificationGroupController;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views.NotificationGroup
{
    public partial class AddNotificationGroupView
    {
        private bool _isEditMode;

        public AddNotificationGroupView()
        {
            InitializeComponent();
        }

        public bool IsEditMode
        {
            private get => _isEditMode;
            set
            {
                //todo change to binding
                _isEditMode = value;

                if ( value )
                {
                    //TxtNotificationGroupId.Visibility = Visibility.Visible;
                    BtnSubmit.Content = "ویرایش گروه";
                }
                else
                {
                    TxtNotificationGroupName.Text = DateTime.Now.ToString("yyyy-MM-dd");
                    //TxtNotificationGroupId.Visibility = Visibility.Collapsed;
                    TxtNotificationGroupId.Text = "";
                    BtnSubmit.Content = "ثبت گروه";
                }
            }
        }

        private void BtnSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //Create Model
                var notificationGroup = new NotificationGroupModel
                {
                    Name = TxtNotificationGroupName.Text
                };

                if ( !string.IsNullOrWhiteSpace(TxtNotificationGroupId.Text) )
                    notificationGroup.Id = Convert.ToInt32(TxtNotificationGroupId.Text);

                //Validate Model
                var result = new NotificationGroupValidator().Validate(notificationGroup);
                if ( result.IsValid == false )
                {
                    MessageBox.Show(result.Errors[0].ErrorMessage);
                    return;
                }


                if ( IsEditMode )
                {
                    //Update Model
                    Update(notificationGroup);
                    MessageBoxHandler.ShowMessage("ویرایش", "گروه اعلان ویرایش شد");
                }
                else
                {
                    //Insert Model
                    CreateNotificationGroup(notificationGroup);
                    MessageBoxHandler.ShowMessage("ثبت", "گروه اعلان ثبت شد");
                }


                //UPDATE BINDING
                Visibility = Visibility.Collapsed;
                Main.NotificationGroupList.Visibility = Visibility.Visible;
                Main.AddNotification.FillGroupNameComboBox();
                Main.NotificationGroupList.FillListView(GetAllNotificationGroup_Obsolete());

                //DELETE Fields
                TxtNotificationGroupName.Text = "";
                TxtNotificationGroupId.Text = "";
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        public void FillControls(NotificationGroupModel notificationGroup)
        {
            TxtNotificationGroupId.Text = notificationGroup.Id.ToString();
            TxtNotificationGroupName.Text = notificationGroup.Name;
        }
    }
}