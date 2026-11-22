using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class MafatihController
    {
        public static List<MafatihModel> GetAllMafatih()
        {
            const string query = "SELECT * FROM Mafatih";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<MafatihModel>(query).ToList();
        }

        [Obsolete("Use GetAllMafatih or strongly-typed queries instead of DataTable")]
        public static DataTable GetFarazTableByDuaId_Obsolete(int duaId)
        {
            const string query =
                "SELECT M.*, ML.DuaName FROM Mafatih AS M INNER JOIN MafatihList AS ML ON ML.DuaId = M.DuaId WHERE M.DuaId = @DuaId";

            return DataAccess.Connector.DataAccess.Select(query, new Dictionary<string, object> { ["@DuaId"] = duaId });
        }
    }
}
