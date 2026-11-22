using Dapper;
using System.Collections.Generic;
using System.Data;
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

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<SureListModel>(query).ToList();
        }
    }
}
