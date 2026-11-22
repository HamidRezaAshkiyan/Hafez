using Dapper;
using System.Collections.Generic;
using System.Data;
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

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, new
            {
                user.UserId,
                Password = passwordToStore,
                user.Name,
                Statue = user.Statue.ToString(),
                UserType = user.UserType.ToString()
            });
        }

        public static void UpdateUser(UserModel user)
        {
            const string query =
                "UPDATE Users SET Password = @Password, Name = @Name, Statue = @Statue, UserType = @UserType WHERE Id = @Id";

            string passwordToStore = PasswordHasher.IsHashed(user.Password)
                ? user.Password
                : PasswordHasher.Hash(user.Password);

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, new
            {
                user.Id,
                Password = passwordToStore,
                user.Name,
                Statue = user.Statue.ToString(),
                UserType = user.UserType.ToString()
            });
        }

        public static void DeleteUser(UserModel user)
        {
            const string query = "DELETE FROM Users WHERE Id = @Id;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, user);
        }

        public static IEnumerable<UserModel> GetAllUserList()
        {
            const string query = "SELECT * FROM Users WHERE Id <> 1";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<UserModel>(query).ToList();
        }

        public static UserModel IsUserValid(this UserModel portInputModel)
        {
            if (portInputModel == null || string.IsNullOrWhiteSpace(portInputModel.UserId))
            {
                return null;
            }

            const string query = "SELECT * FROM Users WHERE UserId = @UserId";

            using IDbConnection connection = SqlConnector.CreateConnection();
            UserModel output = connection.Query<UserModel>(query, new { portInputModel.UserId }).FirstOrDefault();

            if (output == null || output.Statue != 'E')
            {
                return null;
            }

            if (!PasswordHasher.Verify(portInputModel.Password, output.Password))
            {
                return null;
            }

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
