using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class NotificationController
    {
        public static List<NotificationModel> GetAllNotifications()
        {
            const string query = "SELECT * FROM Notification ORDER BY Id DESC";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<NotificationModel>(query).ToList();
        }

        public static async Task<List<NotificationModel>> GetAllNotificationByType(char notificationType = 'L')
        {
            const string query = @"SELECT N.*, U.Name AS CreatorName
                                   FROM Notification AS N
                                   LEFT JOIN Users AS U ON N.CreatedBy = U.Id
                                   WHERE NotificationType = @NotificationType";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return (await connection.QueryAsync<NotificationModel>(
                query,
                new { NotificationType = notificationType.ToString() })).ToList();
        }

        public static NotificationModel CreateNotification(NotificationModel model)
        {
            const string query = @"
INSERT INTO Notification (GroupId, SortId, Description, Name, NotificationType, CreatedBy)
VALUES (@GroupId, @SortId, @Description, @Name, @NotificationType, @CreatedBy);
SELECT last_insert_rowid();";

            using IDbConnection connection = SqlConnector.CreateConnection();
            int newId = connection.ExecuteScalar<int>(query, new
            {
                model.GroupId,
                model.SortId,
                model.Description,
                model.Name,
                NotificationType = model.NotificationType.ToString(),
                CreatedBy = model.CreatedBy > 0 ? (int?)model.CreatedBy : null
            });

            model.Id = newId;
            return model;
        }

        public static NotificationModel RemoveNotification(NotificationModel model)
        {
            using IDbConnection connection = SqlConnector.CreateConnection();
            NotificationModel output = connection.QueryFirstOrDefault<NotificationModel>(
                "SELECT * FROM Notification WHERE Id = @Id", new { model.Id });
            connection.Execute("DELETE FROM Notification WHERE Id = @Id", new { model.Id });
            return output ?? model;
        }

        public static void ChangeNotificationTableHeader(DataTable notificationsTable)
        {
            notificationsTable.Columns["Name"].ColumnName = "T1";
            notificationsTable.Columns["Description"].ColumnName = "H1";
        }

        public static void CreateSimpleNotification(NotificationModel notification)
        {
            const string query =
                "INSERT INTO Notification (GroupId, Description, Name, NotificationType) " +
                "VALUES (@GroupId, @Description, @Name, @NotificationType)";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, new
            {
                notification.GroupId,
                notification.Description,
                notification.Name,
                NotificationType = notification.NotificationType.ToString()
            });
        }

        public static void UpdateNotification(NotificationModel notification)
        {
            const string query =
                "UPDATE Notification SET SortId = @SortId, GroupId = @GroupId, NotificationType = @NotificationType, Description = @Description, Name = @Name WHERE Id = @Id";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, new
            {
                notification.Id,
                notification.SortId,
                notification.GroupId,
                NotificationType = notification.NotificationType.ToString(),
                notification.Description,
                notification.Name
            });
        }

        public static async Task ImportNotificationListDataAsync(IEnumerable<NotificationModel> models)
        {
            const string query =
                "INSERT INTO Notification (GroupId, SortId, Description, Name, NotificationType) " +
                "VALUES (@GroupId, @SortId, @Description, @Name, @NotificationType);";

            using IDbConnection connection = SqlConnector.CreateConnection();
            foreach (NotificationModel m in models)
            {
                await connection.ExecuteAsync(query, new
                {
                    m.GroupId,
                    m.SortId,
                    m.Description,
                    m.Name,
                    NotificationType = m.NotificationType.ToString()
                });
            }
        }

        public static DataTable GetNotificationsByGroupId_Obsolete(int groupId)
        {
            const string query = @"SELECT N.Id, NG.Name, Description
                                   FROM Notification AS N
                                   LEFT JOIN NotificationGroup AS NG ON N.GroupId = NG.Id
                                   WHERE N.GroupId = @GroupId;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<NotificationModel>(query, new { GroupId = groupId }).ToList().ToDataTable();
        }

        public static DataTable GetSortedNotificationsByGroupId_Obsolete(int groupId)
        {
            const string query = @"SELECT N.Id, NG.Name, Description
                                   FROM Notification AS N
                                   LEFT JOIN NotificationGroup AS NG ON N.GroupId = NG.Id
                                   WHERE N.GroupId = @GroupId AND N.SortId <> 0
                                   ORDER BY SortId;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<NotificationModel>(query, new { GroupId = groupId }).ToList().ToDataTable();
        }
    }
}
