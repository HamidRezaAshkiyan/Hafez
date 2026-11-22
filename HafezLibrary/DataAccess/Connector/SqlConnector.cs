using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Sqlite;

namespace HafezLibrary.DataAccess.Connector
{
    public static class SqlConnector
    {
        private static string _overrideDatabasePath;

        public static string GetDatabasePath()
        {
            if (!string.IsNullOrWhiteSpace(_overrideDatabasePath))
            {
                return _overrideDatabasePath;
            }

            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Hafez");
            return Path.Combine(appDataDir, "hafez.db");
        }

        public static void OverrideDatabasePathForTesting(string databasePath)
        {
            _overrideDatabasePath = databasePath;
            DatabaseInitializer.ResetInitializationState();
        }

        public static string GetConnectionString(string name = "HafezSqlite")
        {
            var builder = new SqliteConnectionStringBuilder
            {
                DataSource = GetDatabasePath(),
                Mode = SqliteOpenMode.ReadWriteCreate,
                Cache = SqliteCacheMode.Shared
            };

            return builder.ToString();
        }

        public static IDbConnection CreateConnection()
        {
            DatabaseInitializer.EnsureInitialized();
            var connection = new SqliteConnection(GetConnectionString());
            connection.Open();
            return connection;
        }

        /// <summary>
        /// Converts an IEnumerable to a DataTable for legacy WPF DataGrid/marquee bindings.
        /// </summary>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            DataTable dataTable = new(typeof(T).Name);
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in props)
            {
                dataTable.Columns.Add(prop.Name);
            }

            foreach (T item in items)
            {
                object[] values = new object[props.Length];
                for (int i = 0; i < props.Length; i++)
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
