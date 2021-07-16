using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class UserController
    {
        public static void CreateUser(UserModel user)
        {
            const string query = "INSERT INTO Users (UserId, Password, Name, Statue, UserType) " +
                                 "VALUES (@UserId, @Password, @Name, @Statue, @UserType)";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, user);
        }

        public static void UpdateUser(UserModel user)
        {
            const string query = "UPDATE Users SET Password = @Password, Name = @Name, Statue = @Statue, UserType = @UserType WHERE Id = @Id";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, user);
        }

        public static void DeleteUser(UserModel user)
        {
            const string query = "DELETE FROM Users WHERE Id = @Id;";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, user);
        }

        public static IEnumerable<UserModel> GetAllUserList()
        {
            const string query = "SELECT * FROM Users WHERE id <> 1";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<UserModel>(query).ToList();

            return output;
        }

        public static UserModel IsUserValid(this UserModel portInputModel)
        {
            const string query = @"SELECT * FROM Users WHERE UserId = @UserId AND Password = @Password ";

            var p = new {portInputModel.UserId, portInputModel.Password};

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<UserModel>(query, p).ToList().FirstOrDefault();

            return (output?.Statue == 'E') ? output : null;
        }
    }
}