using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class PersonalDuaController
    {
        public static List<PersonalDuaModel> GetAllPersonalDuas()
        {
            const string query = "SELECT * FROM PersonalDua";

            using IDbConnection    connection = new SqlConnection(SqlConnector.GetConnectionString());
            List<PersonalDuaModel> output     = connection.Query<PersonalDuaModel>(query).ToList();

            return output;
        }

        public static async Task ImportNotificationListDataAsync(IEnumerable<PersonalDuaModel> models)
        {
            const string query =
                "INSERT INTO PersonalDua (DuaId, FarazId, ArabicText, PersianText) VALUES ( @DuaId, @FarazId, @ArabicText, @PersianText );";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            await connection.ExecuteAsync(query, models);
        }

        // OLD Way - Using data table
        [Obsolete("Should change to dapper and join")]
        public static DataTable GetFarazTableByDuaId_Obsolete(int duaId)
        {
            // const string query = "SELECT * FROM PersonalDua AS M, PersonalDuaList AS ML WHERE M.DuaId = @DuaId AND ML.DuaId = M.DuaId";
            //
            // var p = new { DuaId = duaId };
            //
            // using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            // var outputList = connection.Query<PersonalDuaModel>(query, p).ToList();
            //
            // return outputList.ToDataTable();

            string query =
                $"SELECT * FROM PersonalDua AS M, PersonalDuaList AS ML WHERE M.DuaId = {duaId} AND ML.DuaId = M.DuaId";
            DataTable dataTable = DataAccess.Connector.DataAccess.Select(query);

            return dataTable;
        }
    }
}