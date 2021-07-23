using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using HafezLibrary.Models;
using System.Reflection;

namespace HafezLibrary.DataAccess.Processor
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
        }

        public static List<string> LoadFile(this string file)
        {
            if ( !File.Exists(file) )
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        public static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
        {
            List<string>   lines  = File.ReadAllLines(filePath).ToList();
            List<T>        output = new List<T>();
            T              entry  = new T();
            PropertyInfo[] cols   = entry.GetType().GetProperties();

            // Checks to be sure we have at least one header row and one data row
            if ( lines.Count < 2 )
            {
                throw new IndexOutOfRangeException("The file was either empty or missing.");
            }

            // Splits the header into one column header per entry
            string[] headers = lines[0].Split(',');

            // Removes the header row from the lines so we don't
            // have to worry about skipping over that first row.
            lines.RemoveAt(0);

            foreach ( string row in lines )
            {
                entry = new T();

                // Splits the row into individual columns. Now the index
                // of this row matches the index of the header so the
                // FirstName column header lines up with the FirstName
                // value in this row.
                string[] values = row.Split(',');

                // Loops through each header entry so we can compare that
                // against the list of columns from reflection. Once we get
                // the matching column, we can do the "SetValue" method to 
                // set the column value for our entry variable to the vals
                // item at the same index as this particular header.
                for ( int i = 0; i < headers.Length; i++ )
                {
                    foreach ( PropertyInfo col in cols )
                    {
                        if ( col.Name == headers[i] )
                        {
                            col.SetValue(entry, Convert.ChangeType(values[i], col.PropertyType));
                        }
                    }
                }

                output.Add(entry);
            }

            return output;
        }

        public static void SaveToTextFile<T>(List<T> data, string filePath) where T : class
        {
            List<string>  lines = new List<string>();
            StringBuilder line  = new StringBuilder();

            if ( data == null || data.Count == 0 )
            {
                throw new ArgumentNullException(nameof(data),
                                                "You must populate the data parameter with at least one value.");
            }

            PropertyInfo[] cols = data[0].GetType().GetProperties();

            // Loops through each column and gets the name so it can comma 
            // separate it into the header row.
            foreach ( PropertyInfo col in cols )
            {
                line.Append(col.Name);
                line.Append(",");
            }

            // Adds the column header entries to the first line (removing
            // the last comma from the end first).
            lines.Add(line.ToString().Substring(0, line.Length - 1));

            foreach ( T row in data )
            {
                line = new StringBuilder();

                foreach ( PropertyInfo col in cols )
                {
                    line.Append(col.GetValue(row));
                    line.Append(",");
                }

                // Adds the row to the set of lines (removing
                // the last comma from the end first).
                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }

            File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }

        /*public static List<NotificationModel> ConvertToNotificationModels(this IEnumerable<string> lines)
        {
            var output = new List<NotificationModel>();

            foreach (var line in lines)
            {
                var cols = line.Split(',');

                var n = new NotificationModel();
                //n.Id = int.Parse(cols[0]);
                //n.GroupId = int.Parse(cols[1]);
                //n.Description = cols[2];
                //n.Name = cols[3];
                //n.NotificationType = Convert.ToChar(cols[4]);
                //n.CreatedAt = cols[5];

                n.Description = cols[0];
                n.Name = cols[1];

                output.Add(n);
            }

            return output;
        }

        public static void SaveToNotificationFile(this List<NotificationModel> models, string fileName)
        {
            var lines = models
                .Select(n => $"{n.Id},{n.GroupId},{n.Description},{n.Name},{n.NotificationType},{n.CreatedAt}")
                .ToList();

            File.WriteAllLines(fileName.FullFilePath(), lines, Encoding.UTF8);
        }*/

        public static void SaveToNotificationGroupFile(this List<NotificationModel> models, string fileName)
        {
            List<string> lines = models.Select(n => $"{n.Description},{n.Name}").ToList();
            //lines.Insert(0, $"Description,Name"); // add header

            File.WriteAllLines(fileName.FullFilePath(), lines, Encoding.UTF8);
        }

        /*public static void SaveToUserConfigFile(this DataTable input, string filePath)
        {
            //var lines = new List<string>();
            var userConfig = input.Columns.Cast<DataColumn>().Aggregate("",
                (current, dataColumn) => current + $"{input.Rows[0][dataColumn.ColumnName]},");
            userConfig = userConfig.Remove(userConfig.Length - 1);
            //lines.Add(temp);

            File.WriteAllLines(filePath, new List<string> { userConfig }, Encoding.UTF8);
            //File.WriteAllLines(filePath, lines, Encoding.UTF8);
        }*/
    }
}