using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class NotificationGroupController
    {
        public static void CreateNotificationGroup(NotificationGroupModel notificationGroup)
        {
            const string query = "INSERT INTO NotificationGroup (Name) VALUES (@Name);";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, notificationGroup);
        }

        public static void Update(NotificationGroupModel notificationGroup)
        {
            const string query = "UPDATE NotificationGroup SET Name = @Name WHERE Id = @Id;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, notificationGroup);
        }

        public static void RemoveNotificationGroup(NotificationGroupModel notificationGroup)
        {
            const string query = "DELETE FROM Notification WHERE GroupId = @Id; " +
                                 "DELETE FROM NotificationGroup WHERE Id = @Id;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, notificationGroup);
        }

        public static void RemoveNotificationGroupSortIds(NotificationGroupModel notificationGroup)
        {
            const string query = "UPDATE Notification SET SortId = 0 WHERE GroupId = @GroupId";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, new { GroupId = notificationGroup.Id });
        }

        public static List<NotificationGroupModel> GetAllNotificationGroups()
        {
            const string query = "SELECT * FROM NotificationGroup ORDER BY Id";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<NotificationGroupModel>(query).ToList();
        }

        public static DataTable GetAllNotificationGroup_Obsolete()
        {
            return GetAllNotificationGroups().ToDataTable();
        }
    }
}
