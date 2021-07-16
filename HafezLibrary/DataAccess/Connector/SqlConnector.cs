using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;

namespace HafezLibrary.DataAccess.Connector
{
    public static class SqlConnector
    {
        // public static SqlConnection Con { get; } = new SqlConnection(GetConnectionString());

        /// <summary>
        /// this method Convert DataTable to List
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<T> ToList<T>(this DataTable dt)
        {
            return dt.Rows.Cast<DataRow>().Select(GetItem<T>).ToList();
        }

        private static T GetItem<T>(DataRow dr)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach ( DataColumn column in dr.Table.Columns )
            {
                foreach ( var pro in temp.GetProperties() )
                {
                    if ( pro.Name == column.ColumnName )
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }

            return obj;
        }

        /// <summary>
        /// this Method Convert IEnumerable to DataTable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach ( var prop in props )
            {
                //Setting column names as Property names  
                dataTable.Columns.Add(prop.Name);
            }

            foreach ( var item in items )
            {
                var values = new object[props.Length];
                for ( var i = 0; i < props.Length; i++ )
                {
                    values[i] = props[i].GetValue(item, null);
                }

                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static string GetConnectionString(string name = "DebugDB")
        {
            return
                @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=C:\USERS\MRSMILE\SOURCE\REPOS\HAFEZ\HAFEZWPFUI\BIN\DEBUG\DATA\DB.MDF;Integrated Security=True";
                // @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='|DataDirectory|\Data\DB.mdf';Integrated Security=True;Connect Timeout=30";
            //return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}