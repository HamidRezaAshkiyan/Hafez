using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using HafezLibrary.DataAccess.Connector;
using HafezLibrary.Models;
using HafezWPFUI.Properties;

namespace HafezWPFUI.Helper
{
    public static class NotificationHelper
    {
        public static void ExportListToExcel(List<NotificationModel> input)
        {
            if ( input.Count == 0 )
            {
                throw new Exception("گروه خالی میباشد.");
            }

            NotificationGroupModel groupName = GlobalConfig.Main.NotificationGroupList.NotificationGroupListView.Items
                                                           .Cast<NotificationGroupModel>()
                                                           .First(x => x.Id == input.First().GroupId);
            string fileName = $"{groupName.Name} - {DateTime.Now.ToShortDateString().Replace('/', '-')}";

            using SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = Resources.Excel_Document_Avalabale_Format, FileName = fileName
            };
            try
            {
                if ( sfd.ShowDialog() == DialogResult.OK )
                {
                    Task.Run(() => ExcelConnector.ExportFromListToExcel(input, sfd.FileName));
                }
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}