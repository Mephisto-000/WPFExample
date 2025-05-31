using System.Data;

namespace WpfAppSimpleDataManager.Services
{
    public interface ICsvService
    {
        DataTable CreateEmpty(string path, int colCount);
        DataTable Load(string path);
        void Save(DataTable table, string path);
        void Delete(string path);
    }
}