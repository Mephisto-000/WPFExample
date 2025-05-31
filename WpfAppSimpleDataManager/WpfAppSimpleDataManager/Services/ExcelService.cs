// Services/ExcelService.cs
using ClosedXML.Excel;
using System.Data;
using System.IO;

namespace WpfAppSimpleDataManager.Services
{
    public class ExcelService : IExcelService
    {
        public DataTable CreateEmpty(string path, int colCount)
        {
            var dt = new DataTable();
            for (int i = 1; i <= colCount; i++)
                dt.Columns.Add($"Col{i}");

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Sheet1");
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    ws.Cell(1, i + 1).Value = dt.Columns[i].ColumnName;
                }
                wb.SaveAs(path);
            }
            return dt;
        }

        public DataTable Load(string path)
        {
            var dt = new DataTable();
            using (var wb = new XLWorkbook(path))
            {
                var ws = wb.Worksheet(1);
                bool firstRow = true;
                foreach (var row in ws.RowsUsed())
                {
                    if (firstRow)
                    {
                        // 讀取表頭
                        foreach (var cell in row.Cells())
                        {
                            dt.Columns.Add(cell.GetString());
                        }
                        firstRow = false;
                    }
                    else
                    {
                        var dr = dt.NewRow();
                        int i = 0;
                        foreach (var cell in row.Cells(1, dt.Columns.Count))
                        {
                            dr[i++] = cell.Value.ToString();
                        }
                        dt.Rows.Add(dr);
                    }
                }
            }
            return dt;
        }

        public void Save(DataTable table, string path)
        {
            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Sheet1");
                // 先寫表頭
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    ws.Cell(1, i + 1).Value = table.Columns[i].ColumnName;
                }
                // 再寫內容
                for (int r = 0; r < table.Rows.Count; r++)
                {
                    for (int c = 0; c < table.Columns.Count; c++)
                    {
                        ws.Cell(r + 2, c + 1).Value = table.Rows[r][c]?.ToString() ?? string.Empty;
                    }
                }
                wb.SaveAs(path);
            }
        }

        public void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}