using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class SureListController
    {
        public static List<SureListModel> GetAllSureList()
        {
            const string query = "SELECT * FROM SureList";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<SureListModel>(query).ToList();

            return output;
        }
    }
}