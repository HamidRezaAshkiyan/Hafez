using System;
using System.Threading.Tasks;
using System.Windows;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using static HafezLibrary.Controllers.NotificationController;
using static HafezWPFUI.GlobalConfig;

namespace HafezWPFUI.Helper
{
    public static class NotificationGroupHelper
    {
        public static async Task ImportGroupNotificationData(NotificationGroupModel notificationGroup, string filePath)
        {
            try
            {
                var notificationModels = await Task.Run(() => ExcelConnector.ImportFromExcelToList(filePath));

                foreach ( var notificationModel in notificationModels )
                {
                    notificationModel.GroupId = notificationGroup.Id;
                    notificationModel.NotificationType = 'L';

                    if ( string.IsNullOrWhiteSpace(notificationModel.Name) )
                        notificationModel.Name = notificationGroup.Name;
                }

                await ImportNotificationListDataAsync(notificationModels);

                Main.NotificationList.NotificationListView.ItemsSource = await Main.NotificationList.GetNotificationListByType();
            }
            catch ( Exception exception )
            {
                LogInformation(exception);
                MessageBox.Show(exception.Message);
            }
        }
    }
}