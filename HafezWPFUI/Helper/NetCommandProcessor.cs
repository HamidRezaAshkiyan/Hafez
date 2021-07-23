using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using HafezLibrary.Controllers;
using HafezLibrary.Models;
using HafezWPFUI.Views.NotificationGroup;
using HafezWPFUI.Views.Windows;
using System.Collections.Generic;
using System.Data;
using static HafezLibrary.Controllers.NotificationController;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Helper
{
    public static class NetCommandProcessor
    {
        public static UserModel ParseInput(string input)
        {
            UserModel output        = new();
            string[]  splitCommands = input.Split('/');

            output.Id       = -1;
            output.UserId   = splitCommands[0];
            output.Password = splitCommands[1];
            output.Command  = splitCommands[2] + splitCommands[3];

            output = output.IsUserValid();

            return output;
        }

        public static async System.Threading.Tasks.Task DoCommandAsync(UserModel userCommand)
        {
            if ( Listener.Visibility != Visibility.Visible )
            {
                return;
            }

            try
            {
                List<string> commands = userCommand.Command.ToLower().Split('/').ToList();
                switch ( userCommand.UserType )
                {
                    case 'S':
                    case 'A':
                        switch ( commands[0] )
                        {
                            case "notification":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( commands.Count > 1 && !string.IsNullOrWhiteSpace(commands[2]) )
                                            {
                                                int id = Convert.ToInt32(commands[2]);
                                                NotificationGroupModel notificationGroup =
                                                    Main.NotificationGroupList.NotificationGroupListView
                                                        .Items.Cast<NotificationGroupModel>().First(x => x.Id == id);

                                                if ( notificationGroup == null )
                                                {
                                                    return;
                                                }

                                                DataTable notificationsTable =
                                                    GetSortedNotificationsByGroupId_Obsolete(notificationGroup.Id);
                                                //NotificationController.GetNotificationsByGroupId(notificationGroup.Id);

                                                ChangeNotificationTableHeader(notificationsTable);
                                                NotificationGroupList_View.SendToOutput(notificationGroup,
                                                    notificationsTable);
                                            }

                                            if ( Main.PlayPackIcon.Foreground == MainWindowView.EnabledColor )
                                            {
                                                Main.NotificationPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "stop":
                                        {
                                            if ( Main.PlayPackIcon.Foreground == MainWindowView.DisabledColor )
                                            {
                                                Main.NotificationPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "send":
                                        {
                                            // send/title/description
                                            NotificationModel notification = new()
                                            {
                                                GroupId          = 2,
                                                Name             = commands[2],
                                                Description      = commands[3],
                                                NotificationType = 'N',
                                                CreatedBy        = userCommand.Id
                                            };

                                            CreateNotification(notification);

                                            /*Main.NotificationList.NetNotificationListView.SelectedItem = //notification;
                                                Main.NotificationList.NetNotificationListView.Items.Cast<NotificationModel>()
                                                    .Where(x => x.Id == notification.Id);

                                            Main.NotificationList.MenuItemSelect_OnClick(null, null);*/
                                            //NotificationController.CreateNotification(notification);

                                            Main.NotificationList.NetNotificationListView.ItemsSource =
                                                new ObservableCollection<
                                                    NotificationModel>(await GetAllNotificationByType('N'));

                                            Main.SnackbarUserName.Text    = userCommand.Name;
                                            Main.SnackbarMessage.IsActive = true;
                                        }
                                        break;
                                }

                                break;

                            case "picture":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( commands.Count > 1 )
                                            {
                                                bool hasValue = int.TryParse(commands[2], out int value);

                                                if ( Main.PackIconSlide.Foreground == MainWindowView.EnabledColor )
                                                {
                                                    Main.ImageSlide_OnClick(null, null);
                                                }

                                                if ( hasValue && value > 0 )
                                                {
                                                    Main.ComboBoxImage.SelectedIndex = value - 1;
                                                }
                                            }

                                            Main.ButtonPlayImage_OnClick(null, null);
                                        }
                                        break;
                                    case "next":
                                        {
                                            Main.ButtonNextImage_OnClick(null, null);
                                        }
                                        break;
                                    case "back":
                                        {
                                            Main.ButtonPreviousImage_OnClick(null, null);
                                        }
                                        break;
                                    case "stop":
                                        {
                                            Main.ButtonStopImage_OnClick(null, null);
                                        }
                                        break;
                                    case "pause":
                                        {
                                            Main.ButtonStopImage_OnClick(null, null);
                                        }
                                        break;
                                }

                                break;

                            case "camera":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( Main.CameraPackIcon.Foreground == MainWindowView.EnabledColor )
                                            {
                                                Main.CameraButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "stop":
                                        {
                                            if ( Main.CameraPackIcon.Foreground == MainWindowView.DisabledColor )
                                            {
                                                Main.CameraButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                }

                                break;

                            case "quran":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( commands.Count > 1 )
                                            {
                                                bool hasValue = int.TryParse(commands[2], out int value);

                                                if ( hasValue && value > 0 )
                                                {
                                                    Main.ComboBoxQuranSure.SelectedIndex = value - 1;
                                                }
                                            }

                                            if ( commands.Count > 2 )
                                            {
                                                bool hasValue = int.TryParse(commands[3], out int value);

                                                if ( hasValue && value > 0 )
                                                {
                                                    Main.TxtQuranPageNumber.Text = commands[3];
                                                }
                                            }
                                            /*if (commands.Count > 2 && !string.IsNullOrWhiteSpace(commands[3]))
                                            {
                                                Main.TxtQuranPageNumber.Text = commands[3];
                                            }*/

                                            if ( Output.QuranContainer.Visibility == Visibility.Collapsed )
                                            {
                                                Main.QuranPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "pause":
                                        {
                                            if ( Output.QuranContainer.Visibility == Visibility.Visible )
                                            {
                                                Main.QuranPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "stop":
                                        {
                                            Output.ResetAyah();
                                        }
                                        break;
                                    case "next":
                                        {
                                            Output.SetNextAyah();
                                        }
                                        break;
                                    case "back":
                                        {
                                            Output.SetPreviousAyah();
                                        }
                                        break;
                                }

                                break;

                            case "mafatih":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( commands.Count > 1 && !string.IsNullOrWhiteSpace(commands[2]) &&
                                                 commands[2]    != "0" )
                                            {
                                                Main.ComboBoxMafatihDuaNames.SelectedIndex =
                                                    Convert.ToInt32(commands[2]) - 1;
                                            }

                                            if ( Output.MafatihContainer.Visibility == Visibility.Collapsed )
                                            {
                                                Main.MafatihPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "pause":
                                        {
                                            if ( Output.MafatihContainer.Visibility == Visibility.Visible )
                                            {
                                                Main.MafatihPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "stop":
                                        {
                                            Output.ResetFaraz();
                                        }
                                        break;
                                    case "next":
                                        {
                                            Output.SetNextFaraz();
                                        }
                                        break;
                                    case "back":
                                        {
                                            Output.SetPreviousFaraz();
                                        }
                                        break;
                                }

                                break;
                            case "personal":
                                switch ( commands[1] )
                                {
                                    case "play":
                                        {
                                            if ( commands.Count > 1 && !string.IsNullOrWhiteSpace(commands[2]) &&
                                                 commands[2]    != "0" )
                                            {
                                                Main.ComboBoxPersonalDuaNames.SelectedIndex =
                                                    Convert.ToInt32(commands[2]) - 1;
                                            }

                                            if ( Output.MafatihContainer.Visibility == Visibility.Collapsed )
                                            {
                                                Main.PersonalDuaPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "pause":
                                        {
                                            if ( Output.MafatihContainer.Visibility == Visibility.Visible )
                                            {
                                                Main.PersonalDuaPlayButton_OnClick(null, null);
                                            }
                                        }
                                        break;
                                    case "stop":
                                        {
                                            Output.ResetFaraz();
                                        }
                                        break;
                                    case "next":
                                        {
                                            Output.SetNextFaraz();
                                        }
                                        break;
                                    case "back":
                                        {
                                            Output.SetPreviousFaraz();
                                        }
                                        break;
                                }

                                break;
                        }

                        break;

                    case 'U':
                        switch ( commands[0] )
                        {
                            case "notification":
                                switch ( commands[1] )
                                {
                                    case "send":
                                        {
                                            // send/title/description
                                            NotificationModel notification = new()
                                            {
                                                //GroupId = 1,
                                                GroupId          = 2,
                                                Name             = commands[2],
                                                Description      = commands[3],
                                                NotificationType = 'N',
                                                CreatedBy        = userCommand.Id
                                            };

                                            notification =
                                                CreateNotification(
                                                                   notification);

                                            Main.NotificationList.NetNotificationListView.ItemsSource =
                                                new ObservableCollection<
                                                    NotificationModel>(await GetAllNotificationByType('N'));

                                            Main.SnackbarUserName.Text    = userCommand.Name;
                                            Main.SnackbarMessage.IsActive = true;
                                        }
                                        break;
                                }

                                break;
                        }

                        break;
                }
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
            }
        }
    }
}