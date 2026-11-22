using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;
using static HafezLibrary.DataAccess.Connector.SqlConnector;

namespace HafezLibrary.DataAccess.Connector
{
    public static class DataAccess
    {
        public static DataTable Select(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using var connection = (SqliteConnection)CreateConnection();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }

                using var reader = command.ExecuteReader();
                var table = new DataTable();
                table.Load(reader);
                return table;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not Select. Error #902\nError detail: {e.Message}");
                return null;
            }
        }

        public static void DoCommand(string query, Dictionary<string, object> parameters = null)
        {
            try
            {
                using var connection = (SqliteConnection)CreateConnection();
                using var command = connection.CreateCommand();
                command.CommandText = query;

                if (parameters != null)
                {
                    foreach (var kvp in parameters)
                    {
                        command.Parameters.AddWithValue(kvp.Key, kvp.Value ?? DBNull.Value);
                    }
                }

                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not Do Command. Error #903\nError detail: {e.Message}");
            }
        }
    }
}
