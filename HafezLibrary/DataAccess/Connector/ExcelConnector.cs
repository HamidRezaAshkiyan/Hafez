﻿using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using HafezLibrary.Models;


namespace HafezLibrary.DataAccess.Connector
{

    // This is build with Excel COM but you can do it with a package office.interop...
    // Find a better library
    public static class ExcelConnector
    {
        //TODO find better library to work with Excel
        //some library i found: NPOI free
        //IRONXL best but license, gemsheet and etc
        public static void ExportFromListToExcel(List<NotificationModel> input, string saveFilePath)
        {
            try
            {
                var xlApp = new Application();
                var hWnd = xlApp.Application.Hwnd;
                var xlWorkbook = xlApp.Workbooks.Add();
                var xlWorksheet = (Worksheet)xlWorkbook.ActiveSheet;
                xlWorksheet.Name = "خروجی اطلاعات";

                // var array = new List<object> { xlApp, xlWorkbook, xlWorksheet };

                var rowsCount = input.Count;

                for ( var i = 0; i < rowsCount; i++ )
                {
                    xlWorksheet.Cells[i + 1, 1] = input[i].SortId;
                    xlWorksheet.Cells[i + 1, 2] = input[i].Description;
                    xlWorksheet.Cells[i + 1, 3] = input[i].Name;
                }

                xlWorkbook.SaveAs(saveFilePath, AccessMode: XlSaveAsAccessMode.xlExclusive);

                GetWindowThreadProcessId((IntPtr) hWnd, out var processId);
                Process.GetProcessById((int) processId).Kill();

                // xlWorkbook.Close();
                // xlApp.Quit();
                // array.ReleaseResources();

            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
            }
        }

        public static List<NotificationModel> ImportFromExcelToList(string filePath)
        {
            var output = new List<NotificationModel>();
            
            try
            {
                var xlApp = new Application();
                var hWnd = xlApp.Application.Hwnd;
                // var xlWorkbook = xlApp.Workbooks.Open(filePath);
                var xlWorkbooks = xlApp.Workbooks;
                var xlWorkbook = xlWorkbooks.Open(filePath); // Fixed
                var xlWorksheet = xlWorkbook.Sheets[1];
                var xlRange = xlWorksheet.UsedRange;
                //var array = new List<object> {  xlRange, xlWorksheet, xlWorkbook, xlApp};
            
                int colCount = xlRange.Columns.Count;
                int rowCount = xlRange.Rows.Count;
                switch ( colCount )
                {
                    case 1:
                        {
                            for ( var i = 1; i <= rowCount; i++ )
                            {
                                var notificationModel = new NotificationModel
                                {
                                    SortId = 0,
                                    Description = xlRange.Cells[i, 1].Value.ToString(),
                                };
            
                                output.Add(notificationModel);
                            }
            
                            break;
                        }
                    case 2:
                        {
                            for ( var i = 1; i <= rowCount; i++ )
                            {
                                var notificationModel = new NotificationModel
                                {
                                    SortId = xlRange.Cells[i, 1].Value == null
                                        ? 0
                                        : Convert.ToInt32(xlRange.Cells[i, 1].Value.ToString()),
                                    Description = xlRange.Cells[i, 2].Value.ToString(),
                                };
            
                                output.Add(notificationModel);
                            }
            
                            break;
                        }
                    case 3:
                        {
                            for ( var i = 1; i <= rowCount; i++ )
                            {
                                var notificationModel = new NotificationModel
                                {
                                    SortId = xlRange.Cells[i, 1].Value == null
                                        ? 0
                                        : Convert.ToInt32(xlRange.Cells[i, 1].Value.ToString()),
            
                                    Description = xlRange.Cells[i, 2].Value.ToString(),
            
                                    Name = xlRange.Cells[i, 3].Value == null
                                        ? string.Empty
                                        : xlRange.Cells[i, 3].Value.ToString()
                                };
            
                                output.Add(notificationModel);
                            }
            
                            break;
                        }
                }
            
                // xlWorkbook.Close();
                // xlWorkbooks.Close();
                // xlApp.Quit();
            
                GetWindowThreadProcessId((IntPtr) hWnd, out var processId);
                Process.GetProcessById((int) processId).Kill();
            
                // xlWorkbook.Close(0);
                // xlApp.Quit();
                // array.ReleaseResources();
            
                // Marshal.ReleaseComObject(xlWorkbook);
                // Marshal.ReleaseComObject(xlWorkbooks);
                // Marshal.ReleaseComObject(xlApp);
            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
            }
            
            return output;
        }

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #region Personal Dua

        public static List<PersonalDuaModel> ImportPersonalDuaFromExcelToList(string filePath)
        {
            var output = new List<PersonalDuaModel>();

            try
            {
                var xlApp = new Application();
                var hWnd = xlApp.Application.Hwnd;
                var xlWorkbook = xlApp.Workbooks.Open(filePath);
                var xlWorksheet = xlWorkbook.Sheets[1];
                var xlRange = xlWorksheet.UsedRange;
                // var array = new List<object> { xlApp, xlWorkbook, xlWorksheet, xlRange };

                // int colCount = xlRange.Columns.Count;
                int rowCount = xlRange.Rows.Count;

                for ( var i = 1; i <= rowCount; i++ )
                {
                    var personalDua = new PersonalDuaModel
                    {
                        ArabicText = xlRange.Cells[i, 1].Value.ToString(),
                        PersianText = xlRange.Cells[i, 2].Value.ToString()
                    };

                    output.Add(personalDua);
                }

                // xlWorkbook.Close();
                // xlApp.Quit();
                // array.ReleaseResources();

                GetWindowThreadProcessId((IntPtr) hWnd, out var processId);
                Process.GetProcessById((int) processId).Kill();
            }
            catch ( Exception exception )
            {
                Console.WriteLine(exception);
                //todo move serilog to library and call here
            }

            return output;
        }

        #endregion

        private static void ReleaseResources(this IEnumerable<object> input)
        {
            //cleanup

            //release com objects to fully kill excel process from running in the background
            foreach ( var o in input )
                Marshal.ReleaseComObject(o);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}