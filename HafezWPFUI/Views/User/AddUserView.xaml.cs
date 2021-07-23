using FluentValidation.Results;
using System;
using System.Windows;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezLibrary.Validators;
using HafezWPFUI.Helper.Handlers;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views.User
{
    public partial class AddUserView
    {
        private bool _isEditMode;

        public AddUserView()
        {
            InitializeComponent();
        }

        public bool IsEditMode
        {
            private get => _isEditMode;
            set
            {
                _isEditMode = value;

                if ( value )
                {
                    //TxtId.Visibility = Visibility.Visible;
                    BtnSubmit.Content = "ویرایش";
                }
                else
                {
                    TxtId.Clear();
                    TxtName.Clear();
                    TxtPassword.Clear();
                    TxtUserId.Clear();
                    ToggleButtonStatue.IsChecked    = false;
                    ToggleButtonAdminType.IsChecked = false;
                    //TxtId.Visibility = Visibility.Hidden;
                    BtnSubmit.Content = "ثبت";
                }
            }
        }

        public void FillControllers(UserModel user)
        {
            if ( user == null )
            {
                throw new ArgumentNullException();
            }

            TxtId.Text           = user.Id.ToString();
            TxtUserId.Text       = user.UserId;
            TxtName.Text         = user.Name;
            TxtPassword.Password = user.Password;

            ToggleButtonStatue.IsChecked    = Convert.ToBoolean(user.Statue   == 'E');
            ToggleButtonAdminType.IsChecked = Convert.ToBoolean(user.UserType == 'A');
        }

        private void BtnSubmit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //define Model
                char statueToggle   = (bool) ToggleButtonStatue.IsChecked ? 'E' : 'D';
                char userTypeToggle = (bool) ToggleButtonAdminType.IsChecked ? 'A' : 'U';

                UserModel user = new UserModel
                {
                    UserId   = TxtUserId.Text,
                    Name     = TxtName.Text,
                    Password = TxtPassword.Password,
                    Statue   = Convert.ToChar(statueToggle),
                    UserType = Convert.ToChar(userTypeToggle)
                };

                if ( IsEditMode )
                {
                    user.Id = Convert.ToInt32(TxtId.Text);
                }

                //Validation Model
                AddUserValidator addUserValidator = new AddUserValidator();
                ValidationResult result           = addUserValidator.Validate(user);
                if ( result.IsValid == false )
                {
                    MessageBox.Show(result.Errors[0].ErrorMessage);
                    return;
                }

                //CreateUser/Update
                if ( IsEditMode )
                {
                    UserController.UpdateUser(user);
                    MessageBoxHandler.ShowMessage("ویرایش", "کاربر ویرایش شد");
                }
                else
                {
                    UserController.CreateUser(user);
                    MessageBoxHandler.ShowMessage("ثبت", "کاربر ثبت شد");
                }

                //GlobalConfig.Main.UserList.FillListView(UserController.GetAllUser());
                Main.UserList.UserListView.ItemsSource = UserController.GetAllUserList();

                // show list
                Visibility               = Visibility.Collapsed;
                Main.UserList.Visibility = Visibility.Visible;

                // clear textboxes
                TxtId.Clear();
                TxtName.Clear();
                TxtUserId.Clear();
                TxtPassword.Clear();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}