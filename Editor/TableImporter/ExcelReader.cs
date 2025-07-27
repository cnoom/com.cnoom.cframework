using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace FrameWork.Editor.TableImporter
{
    public class ExcelReader
    {
        
        public static string GetFirstSheetName(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                return workbook.GetSheetName(0);
            }
        }
        
        public static TableData Read(string filePath)
        {
            var data = new TableData
            {
                headers = new List<string>(),
                types = new List<string>(),
                rows = new List<List<string>>()
            };

            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = new XSSFWorkbook(fs);
                ISheet sheet = workbook.GetSheetAt(0);

                IRow headerRow = sheet.GetRow(0);
                for (int i = 0; i < headerRow.LastCellNum; i++)
                    data.headers.Add(headerRow.GetCell(i)?.ToString() ?? "");

                IRow typeRow = sheet.GetRow(1);
                for (int i = 0; i < typeRow.LastCellNum; i++)
                    data.types.Add(typeRow.GetCell(i)?.ToString() ?? "string");

                for (int i = 2; i <= sheet.LastRowNum; i++)
                {
                    var row = sheet.GetRow(i);
                    if (row == null) continue;
                    var rowData = new List<string>();
                    for (int j = 0; j < data.headers.Count; j++)
                        rowData.Add(row.GetCell(j)?.ToString() ?? "");
                    data.rows.Add(rowData);
                }
            }
            return data;
        }
    }
}