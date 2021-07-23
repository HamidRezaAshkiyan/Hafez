using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class MafatihListController
    {
        public static IEnumerable<MafatihListModel> GetAllDuas()
        {
            const string query = "SELECT * FROM MafatihList";

            using IDbConnection    connection = new SqlConnection(SqlConnector.GetConnectionString());
            List<MafatihListModel> output     = connection.Query<MafatihListModel>(query).ToList();

            return output;
        }
    }
}