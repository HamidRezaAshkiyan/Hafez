using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class PersonalDuaListController
    {
        public static PersonalDuaListModel CreatePersonalDuaList(PersonalDuaListModel model)
        {
            const string query = "INSERT INTO PersonalDuaList (DuaName) VALUES (@DuaName); SELECT last_insert_rowid();";

            using IDbConnection connection = SqlConnector.CreateConnection();
            model.DuaId = connection.ExecuteScalar<int>(query, new { model.DuaName });
            return model;
        }

        public static IEnumerable<PersonalDuaListModel> GetAllPersonalDuaList()
        {
            const string query = "SELECT * FROM PersonalDuaList";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<PersonalDuaListModel>(query).ToList();
        }

        public static void RemovePersonalDua(PersonalDuaListModel personalDuaList)
        {
            const string query = "DELETE FROM PersonalDua WHERE DuaId = @DuaId; " +
                                 "DELETE FROM PersonalDuaList WHERE DuaId = @DuaId;";

            using IDbConnection connection = SqlConnector.CreateConnection();
            connection.Execute(query, personalDuaList);
        }
    }
}
