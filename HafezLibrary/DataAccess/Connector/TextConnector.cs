using HafezLibrary.Controllers;
using HafezLibrary.DataAccess.Processor;
using HafezLibrary.Models;
using System.Collections.Generic;

namespace HafezLibrary.DataAccess.Connector
{
    public class TextConnector
    {
        public const string NotificationFile = "Notifications.csv";

        public static void BackupNotification()
        {
            List<NotificationModel> notifications = NotificationController.GetAllNotifications();

            notifications.SaveToNotificationGroupFile(NotificationFile);
        }

        /*public static List<T> LoadFromTextFile<T>(string filePath) where T : class, new()
        {
            var lines = File.ReadAllLines(filePath).ToList();
            List<T> output = new List<T>();
            T entry = new T();
            var cols = entry.GetType().GetProperties();

            // Checks to be sure we have at least data
            if (lines.Count < 1)
            {
                throw new IndexOutOfRangeException("The file was either empty of missing.");
            }

            var headers = lines[0].Split(',');

            foreach (var line in lines)
            {
                entry = new T();

                var values = line.Split(',');

                for (int i = 0; i < headers.Length; i++)
                {
                    foreach (var col in cols)
                    {
                        if (col.Name == headers[i])
                        {
                            col.SetValue(entry, Convert.ChangeType(values[i], col.PropertyType));
                        }
                    }
                }

                output.Add(entry);
            }

            return output;
        }

        public static void SaveToTextFile<T>(List<T> data, string filePath) where T : class, new()
        {
            var lines = new List<string>();
            var line = new StringBuilder();

            if (data == null || data.Count == 0)
            {
                throw new ArgumentException("data", "You must populate the data parameter with ar least ...");
            }

            var cols = data[0].GetType().GetProperties();

            foreach (var col in cols)
            {
                line.Append(col.Name);
                line.Append(",");
            }

            lines.Add(line.ToString().Substring(0, line.Length - 1));

            foreach (var row in data)
            {
                line = new StringBuilder();

                foreach (var col in cols)
                {
                    line.Append(col.GetValue(row));
                    line.Append(",");
                }

                lines.Add(line.ToString().Substring(0, line.Length - 1));
            }

            File.WriteAllLines(filePath, lines);
        }*/
    }
}