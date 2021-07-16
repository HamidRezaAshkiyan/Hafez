using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<MafatihModel>(query).ToList();

            return output;
        }

        // OLD Way - Using data table
        [Obsolete("Should change to dapper and join or make a local model to store both")]
        public static DataTable GetFarazTableByDuaId_Obsolete(int duaId)
        {
            // const string query = "SELECT * FROM Mafatih AS M, MafatihList AS ML WHERE M.DuaId = @DuaId AND ML.DuaId = M.DuaId";
            //
            // var p = new { DuaId = duaId };
            //
            // using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            // var outputList = connection.Query<MafatihModel>(query, p).ToList();
            //
            // return outputList.ToDataTable();

            var query = $"SELECT * FROM Mafatih AS M, MafatihList AS ML WHERE M.DuaId = {duaId} AND ML.DuaId = M.DuaId";
            var dataTable = DataAccess.Connector.DataAccess.Select(query);

            return dataTable;
        }
    }
}