using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;

namespace HafezLibrary.Controllers
{
    public static class QuranController
    {
        public static IEnumerable<QuranModel> GetAllQuran()
        {
            const string query = "SELECT * FROM Quran";

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var output = connection.Query<QuranModel>(query).ToList();

            return output;
        }

        // OLD Way - Using data table
        public static DataTable GetAyahIdBySureId_Obsolete(int sureId)
        {
            const string query = "SELECT AyahID FROM Quran WHERE suraID = @SureId";

            var p = new { SureId = sureId };

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var outputList = connection.Query<QuranModel>(query, p).ToList();

            return outputList.ToDataTable();
        }

        public static DataTable GetSureIdByPageNumber_Obsolete(int pageNumber)
        {
            const string query = "SELECT SuraID FROM Quran WHERE PageNumber = @PageNumber ORDER BY SuraID";

            var p = new { PageNumber = pageNumber };

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var outputList = connection.Query<QuranModel>(query, p).ToList();

            return outputList.ToDataTable();
        }

        public static DataTable GetAyahsByPageNumber(int pageNumber)
        {
            // BUG JOIN not impletement by dapper ( by me) implement it
            const string query = "SELECT Quran.ID, AyahID, SuraName, AyahText, AyahPersianTranslation, AyahEnglishTranslation " +
                                 "FROM Quran JOIN SureList ON Quran.SuraID = SureList.ID AND PageNumber = @PageNumber";

            var p = new { PageNumber = pageNumber };

            using IDbConnection connection = new SqlConnection(SqlConnector.GetConnectionString());
            var outputList = connection.Query<QuranModel>(query, p).ToList();

            return outputList.ToDataTable();
        }
    }
}