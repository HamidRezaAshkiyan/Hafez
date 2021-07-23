using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using static HafezLibrary.DataAccess.Connector.SqlConnector;

namespace HafezLibrary.Controllers
{
    public static class NotificationController
    {
        /// <summary>
        /// this method returns predefined data by notification stored procedure
        /// </summary>
        /// <returns>list notification model</returns>
        public static List<NotificationModel> GetAllNotifications()
        {
            using IDbConnection     connection = new SqlConnection(GetConnectionString());
            List<NotificationModel> output = connection.Query<NotificationModel>("dbo.spNotification_GetAll").ToList();

            return output;
        }

        public static async Task<List<NotificationModel>> GetAllNotificationByType(char notificationType = 'L')
        {
            const string query = @"SELECT N.*, U.Name AS CreatorName
                                    FROM Notification AS N
                                    LEFT JOIN Users AS U ON N.CreatedBy = U.Id
                                    WHERE NotificationType = @NotificationType";

            var p = new {NotificationType = notificationType};

            using IDbConnection     connection = new SqlConnection(GetConnectionString());
            List<NotificationModel> output     = (await connection.QueryAsync<NotificationModel>(query, p)).ToList();

            return output;
        }

        public static NotificationModel CreateNotification(NotificationModel model)
        {
            using IDbConnection connection = new SqlConnection(GetConnectionString());
            DynamicParameters   p          = new DynamicParameters();
            p.Add("@GroupId",          model.GroupId);
            p.Add("@Description",      model.Description);
            p.Add("@Name",             model.Name);
            p.Add("@NotificationType", model.NotificationType);
            p.Add("@CreatedBy",        model.CreatedBy);
            p.Add("@Id",               0, DbType.Int32, ParameterDirection.Output);

            connection.Execute("spNotification_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@Id");

            return model;
        }

        public static NotificationModel RemoveNotification(NotificationModel model)
        {
            const string query = "SELECT * FROM Notification WHERE Id = @Id; " +
                                 "DELETE   FROM Notification WHERE Id = @Id;";

            var p = new {model.Id};

            using IDbConnection connection = new SqlConnection(GetConnectionString());
            NotificationModel   output     = connection.Query<NotificationModel>(query, p).First();

            return output;
        }

        /*public static List<NotificationModel> GetNotificationsByGroupId(int groupId)
        {
            var output = new List<NotificationModel>();
            var query =
                $@"SELECT N.Id, NG.Name, Description 
                FROM Notification AS N
                LEFT JOIN NotificationGroup AS NG
                ON N.GroupId = NG.Id
                WHERE N.GroupId = @GroupId";
            var p = new { GroupId = groupId };

            return output;
        }*/

        public static void ChangeNotificationTableHeader(DataTable notificationsTable)
        {
            notificationsTable.Columns["Name"].ColumnName        = "T1";
            notificationsTable.Columns["Description"].ColumnName = "H1";
        }

        /// <summary>
        /// this method create a simple notification. that gives just groupId, description, name and notification type.
        /// for more complete insert use create notification
        /// </summary>
        /// <param name="notification"></param>
        public static void CreateSimpleNotification(NotificationModel notification)
        {
            const string query =
                "INSERT INTO Notification (GroupId, Description, Name, NotificationType) " +
                "VALUES (@GroupId, @Description, @Name, @NotificationType)";

            using IDbConnection connection = new SqlConnection(GetConnectionString());
            connection.Execute(query, notification);
        }

        public static void UpdateNotification(NotificationModel notification)
        {
            const string query =
                "UPDATE Notification SET SortId = @SortId, GroupId = @GroupId, NotificationType = @NotificationType, Description = @Description, Name = @Name WHERE Id = @Id";

            using IDbConnection connection = new SqlConnection(GetConnectionString());
            connection.Execute(query, notification);
        }

        public static async Task ImportNotificationListDataAsync(IEnumerable<NotificationModel> models)
        {
            const string query =
                "INSERT INTO Notification ( GroupId, SortId, Description, Name, NotificationType ) " +
                "VALUES ( @GroupId, @SortId, @Description, @Name, @NotificationType );";

            using IDbConnection connection = new SqlConnection(GetConnectionString());
            await connection.ExecuteAsync(query, models);
        }

        // OLD Way - Using data table
        public static DataTable GetNotificationsByGroupId_Obsolete(int groupId)
        {
            const string query = @"SELECT N.Id, NG.Name, Description 
                                    FROM Notification AS N
                                    LEFT JOIN NotificationGroup AS NG
                                    ON N.GroupId = NG.Id
                                    WHERE N.GroupId = @GroupId;";

            var p = new {GroupId = groupId};

            using IDbConnection     connection = new SqlConnection(GetConnectionString());
            List<NotificationModel> outputList = connection.Query<NotificationModel>(query, p).ToList();

            return outputList.ToDataTable();
        }

        public static DataTable GetSortedNotificationsByGroupId_Obsolete(int groupId)
        {
            const string query = @"SELECT N.Id, NG.Name, Description
                                    FROM Notification AS N
                                    LEFT JOIN NotificationGroup AS NG
                                        ON N.GroupId = NG.Id
                                    WHERE N.GroupId = @GroupId
                                           AND N.SortId <> 0
                                    ORDER BY  SortId ";

            var p = new {GroupId = groupId};

            using IDbConnection     connection = new SqlConnection(GetConnectionString());
            List<NotificationModel> outputList = connection.Query<NotificationModel>(query, p).ToList();

            return outputList.ToDataTable();
        }
    }
}