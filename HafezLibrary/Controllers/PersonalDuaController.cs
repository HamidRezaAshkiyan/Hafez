using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
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

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<PersonalDuaModel>(query).ToList();
        }

        public static async Task ImportNotificationListDataAsync(IEnumerable<PersonalDuaModel> models)
        {
            const string query =
                "INSERT INTO PersonalDua (DuaId, FarazId, ArabicText, PersianText) VALUES (@DuaId, @FarazId, @ArabicText, @PersianText);";

            using IDbConnection connection = SqlConnector.CreateConnection();
            await connection.ExecuteAsync(query, models);
        }

        [Obsolete("Use GetAllPersonalDuas or strongly-typed queries instead of DataTable")]
        public static DataTable GetFarazTableByDuaId_Obsolete(int duaId)
        {
            const string query =
                "SELECT M.*, ML.DuaName FROM PersonalDua AS M INNER JOIN PersonalDuaList AS ML ON ML.DuaId = M.DuaId WHERE M.DuaId = @DuaId";

            return DataAccess.Connector.DataAccess.Select(query, new Dictionary<string, object> { ["@DuaId"] = duaId });
        }
    }
}
