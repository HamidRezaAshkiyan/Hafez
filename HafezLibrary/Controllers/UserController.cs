using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using HafezLibrary.Security;

namespace HafezLibrary.Controllers
{
    public static class UserController
    {
        public static void CreateUser(UserModel user)
        {
            const string query = "INSERT INTO Users (UserId, Password, Name, Statue, UserType) " +
                                 "VALUES (@UserId, @Password, @Name, @Statue, @UserType)";

            string passwordToStore = PasswordHasher.IsHashed(user.Password)
                ? user.Password
                : PasswordHasher.Hash(user.Password);

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, new
            {
                user.UserId,
                Password = passwordToStore,
                user.Name,
                user.Statue,
                user.UserType
            });
        }

        public static void UpdateUser(UserModel user)
        {
            const string query =
                "UPDATE Users SET Password = @Password, Name = @Name, Statue = @Statue, UserType = @UserType WHERE Id = @Id";

            string passwordToStore = PasswordHasher.IsHashed(user.Password)
                ? user.Password
                : PasswordHasher.Hash(user.Password);

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, new
            {
                user.Id,
                Password = passwordToStore,
                user.Name,
                user.Statue,
                user.UserType
            });
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
            List<UserModel> output = connection.Query<UserModel>(query).ToList();

            return output;
        }

        public static UserModel IsUserValid(this UserModel portInputModel)
        {
            if (portInputModel == null || string.IsNullOrWhiteSpace(portInputModel.UserId))
            {
                return null;
            }

            const string query = @"SELECT * FROM Users WHERE UserId = @UserId";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            UserModel output = connection.Query<UserModel>(query, new { portInputModel.UserId }).FirstOrDefault();

            if (output == null || output.Statue != 'E')
            {
                return null;
            }

            if (!PasswordHasher.Verify(portInputModel.Password, output.Password))
            {
                return null;
            }

            // Seamlessly upgrade legacy plaintext passwords to PBKDF2 hashes on successful login.
            if (!PasswordHasher.IsHashed(output.Password))
            {
                output.Password = PasswordHasher.Hash(portInputModel.Password);
                connection.Execute("UPDATE Users SET Password = @Password WHERE Id = @Id",
                    new { output.Password, output.Id });
            }

            return output;
        }
    }
}
