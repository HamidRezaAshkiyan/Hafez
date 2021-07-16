using Dapper;
using System.Data;
using System.Data.SqlClient;
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

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, notificationGroup);
        }

        public static void Update(NotificationGroupModel notificationGroup)
        {
            const string query = "UPDATE NotificationGroup SET Name = @Name WHERE Id = @Id;";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, notificationGroup);
        }

        public static void RemoveNotificationGroup(NotificationGroupModel notificationGroup)
        {
            const string query = "DELETE FROM Notification      WHERE GroupId = @Id;" +
                                 "DELETE FROM NotificationGroup WHERE Id      = @Id;";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, notificationGroup);
        }

        public static void RemoveNotificationGroupSortIds(NotificationGroupModel notificationGroup)
        {
            const string query = "UPDATE Notification SET SortId = 0 WHERE GroupId = @GroupId";
            var p = new { GroupId = notificationGroup.Id };

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, p);
        }

        // OLD Way - Using data table
        public static DataTable GetAllNotificationGroup_Obsolete()
        {
            const string query = "SELECT * FROM NotificationGroup";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var outputList = connection.Query<NotificationGroupModel>(query).ToList();

            return outputList.ToDataTable();
        }
    }
}