using System;
using System.Collections.Generic;
using ClosedXML.Excel;
using HafezLibrary.Models;

namespace HafezLibrary.DataAccess.Connector
{
    /// <summary>
    /// Office-free Excel import/export powered by ClosedXML (.xlsx).
    /// </summary>
    public static class ExcelConnector
    {
        public static void ExportFromListToExcel(List<NotificationModel> input, string saveFilePath)
        {
            using var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("خروجی اطلاعات");

            for (int i = 0; i < input.Count; i++)
            {
                worksheet.Cell(i + 1, 1).Value = input[i].SortId;
                worksheet.Cell(i + 1, 2).Value = input[i].Description ?? string.Empty;
                worksheet.Cell(i + 1, 3).Value = input[i].Name ?? string.Empty;
            }

            workbook.SaveAs(saveFilePath);
        }

        public static List<NotificationModel> ImportFromExcelToList(string filePath)
        {
            var output = new List<NotificationModel>();

            using var workbook = new XLWorkbook(filePath);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            IXLRange range = worksheet.RangeUsed();
            if (range == null)
            {
                return output;
            }

            int colCount = range.ColumnCount();
            int rowCount = range.RowCount();

            for (int i = 1; i <= rowCount; i++)
            {
                if (colCount == 1)
                {
                    string desc = range.Cell(i, 1).GetString();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        output.Add(new NotificationModel { SortId = 0, Description = desc });
                    }
                }
                else if (colCount == 2)
                {
                    int.TryParse(range.Cell(i, 1).GetString(), out int sortId);
                    string desc = range.Cell(i, 2).GetString();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        output.Add(new NotificationModel { SortId = sortId, Description = desc });
                    }
                }
                else
                {
                    int.TryParse(range.Cell(i, 1).GetString(), out int sortId);
                    string desc = range.Cell(i, 2).GetString();
                    string name = range.Cell(i, 3).GetString();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        output.Add(new NotificationModel { SortId = sortId, Description = desc, Name = name });
                    }
                }
            }

            return output;
        }

        public static List<PersonalDuaModel> ImportPersonalDuaFromExcelToList(string filePath)
        {
            var output = new List<PersonalDuaModel>();

            using var workbook = new XLWorkbook(filePath);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            IXLRange range = worksheet.RangeUsed();
            if (range == null)
            {
                return output;
            }

            int rowCount = range.RowCount();
            for (int i = 1; i <= rowCount; i++)
            {
                string arabic = range.Cell(i, 1).GetString();
                string persian = range.ColumnCount() >= 2 ? range.Cell(i, 2).GetString() : string.Empty;

                if (!string.IsNullOrWhiteSpace(arabic) || !string.IsNullOrWhiteSpace(persian))
                {
                    output.Add(new PersonalDuaModel
                    {
                        FarazId = i,
                        ArabicText = arabic,
                        PersianText = persian
                    });
                }
            }

            return output;
        }
    }
}
