using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class PersonalDuaListController
    {
        public static PersonalDuaListModel CreatePersonalDuaList(PersonalDuaListModel model)
        {
            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());

            var p = new DynamicParameters();
            p.Add("@DuaName", model.DuaName);
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);

            connection.Execute("spPersonalDua_Insert", p, commandType: CommandType.StoredProcedure);

            model.DuaId = p.Get<int>("@Id");

            return model;
        }

        public static IEnumerable<PersonalDuaListModel> GetAllPersonalDuaList()
        {
            const string query = "SELECT * FROM PersonalDuaList";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<PersonalDuaListModel>(query).ToList();

            return output;
        }

        public static void RemovePersonalDua(PersonalDuaListModel personalDuaList)
        {
            const string query = "DELETE FROM PersonalDua     WHERE DuaId = @DuaId; " +
                                 "DELETE FROM PersonalDuaList WHERE DuaId = @DuaId; ";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            connection.Execute(query, personalDuaList);
        }
    }
}