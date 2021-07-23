using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezWPFUI.Helper.Handlers;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Views.User
{
    public partial class UserList_View
    {
        public UserList_View()
        {
            InitializeComponent();
        }

        private void UserList_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                UserListView.ItemsSource = UserController.GetAllUserList();
                //FillListView(UserController.GetAllUser());
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }

        /*public void FillListView(DataTable userDataTable)
        {
            if (userDataTable == null) return;
            if (UserListView == null) return;
            UserListView.Items.Clear();

            foreach (DataRow userDataTableRow in userDataTable.Rows)
            {
                var newItem = new UserModel
                {
                    Id = Convert.ToInt32($"{userDataTableRow["Id"]}"),
                    UserId = $"{userDataTableRow["UserId"]}",
                    Password = $"{userDataTableRow["Password"]}",
                    Name = $"{userDataTableRow["Name"]}",
                    Statue = Convert.ToChar($"{userDataTableRow["Statue"]}"),
                    UserType = Convert.ToChar($"{userDataTableRow["UserType"]}")
                };

                UserListView.Items.Add(newItem);
            }
        }*/

        private void UserListView_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            CreateRightClickMenu(sender, e);
        }

        private void CreateRightClickMenu(object sender, RoutedEventArgs e)
        {
            if ( !(FindResource("RightClickMenu") is ContextMenu cm) )
            {
                return;
            }

            cm.PlacementTarget = sender as Button;
            cm.IsOpen          = true;
        }

        private void MenuItemNewUser_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Main.ClearPage();
                Main.AddUser.IsEditMode = false;
                Main.AddUser.Visibility = Visibility.Visible;
            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
            }
        }

        private void MenuItemEditUser_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                UserModel user = UserListView.SelectedItem as UserModel;

                Main.AddUser.FillControllers(user);
                Main.ClearPage();
                Main.AddUser.IsEditMode = true;
                Main.AddUser.Visibility = Visibility.Visible;
            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
            }
        }

        private void ButtonRemoveUser_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Button    button     = sender as Button;
                UserModel deleteUser = new UserModel {Id = Convert.ToInt32(button.Uid)};
                UserController.DeleteUser(deleteUser);
                UserListView.ItemsSource = UserController.GetAllUserList();
                //FillListView(UserController.GetAllUser());

                MessageBoxHandler.ShowMessage("حذف", "کاربر با موفقیت حذف شد");
            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
            }
        }
    }
}