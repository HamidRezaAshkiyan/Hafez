using Dapper;
using System.Collections.Generic;
using System.Data;
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

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<QuranModel>(query).ToList();
        }

        public static DataTable GetAyahIdBySureId_Obsolete(int sureId)
        {
            const string query = "SELECT AyahID FROM Quran WHERE SuraID = @SureId";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<QuranModel>(query, new { SureId = sureId }).ToList().ToDataTable();
        }

        public static DataTable GetSureIdByPageNumber_Obsolete(int pageNumber)
        {
            const string query = "SELECT SuraID FROM Quran WHERE PageNumber = @PageNumber ORDER BY SuraID";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<QuranModel>(query, new { PageNumber = pageNumber }).ToList().ToDataTable();
        }

        public static DataTable GetAyahsByPageNumber(int pageNumber)
        {
            const string query =
                "SELECT Quran.ID, AyahID, SuraName, AyahText, AyahPersianTranslation, AyahEnglishTranslation " +
                "FROM Quran JOIN SureList ON Quran.SuraID = SureList.ID WHERE PageNumber = @PageNumber";

            using IDbConnection connection = SqlConnector.CreateConnection();
            return connection.Query<QuranModel>(query, new { PageNumber = pageNumber }).ToList().ToDataTable();
        }
    }
}
