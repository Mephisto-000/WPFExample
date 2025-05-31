using System.Data;
using System.IO;

namespace WpfAppSimpleDataManager.Services
{
    public class CsvService : ICsvService
    {
        public DataTable CreateEmpty(string path, int colCount)
        {
            var dt = new DataTable();
            for (int i = 1; i <= colCount; i++)
                dt.Columns.Add($"Col{i}");

            using (var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(string.Join(",", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName)));
            }
            return dt;
        }

        public DataTable Load(string path)
        {
            var dt = new DataTable();
            using (var sr = new StreamReader(path, System.Text.Encoding.UTF8))
            {
                string? headerLine = sr.ReadLine();
                if (headerLine == null)
                    return dt;

                // 假設以逗號分隔
                var headers = headerLine.Split(',');
                foreach (var h in headers)
                    dt.Columns.Add(h);

                while (!sr.EndOfStream)
                {
                    var line = sr.ReadLine();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    var row = dt.NewRow();
                    for (int i = 0; i < headers.Length && i < parts.Length; i++)
                        row[i] = parts[i];
                    dt.Rows.Add(row);
                }
            }
            return dt;
        }

        public void Save(DataTable table, string path)
        {
            using (var sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
            {
                // 先寫表頭
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    sw.Write(table.Columns[i].ColumnName);
                    if (i < table.Columns.Count - 1) sw.Write(",");
                }
                sw.WriteLine();

                // 再寫每一列
                foreach (DataRow row in table.Rows)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        sw.Write(row[i]?.ToString());
                        if (i < table.Columns.Count - 1) sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
        }

        public void Delete(string path)
        {
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}