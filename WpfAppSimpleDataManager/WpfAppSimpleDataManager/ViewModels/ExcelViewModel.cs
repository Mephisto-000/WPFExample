using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using WpfAppSimpleDataManager.Services;

namespace WpfAppSimpleDataManager.ViewModels
{
    public class ExcelViewModel : INotifyPropertyChanged
    {
        private readonly IExcelService _excelService;

        private DataTable _dataTable = new DataTable();
        public DataTable DataTable
        {
            get => _dataTable;
            set { _dataTable = value; RaisePropertyChanged(nameof(DataTable)); }
        }

        private string _currentFilePath = string.Empty;
        public string CurrentFilePath
        {
            get => _currentFilePath;
            set { _currentFilePath = value; RaisePropertyChanged(nameof(CurrentFilePath)); }
        }

        public ICommand CreateNewCommand { get; }
        public ICommand OpenCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }

        public ExcelViewModel()
        {
            _excelService = new ExcelService();

            CreateNewCommand = new RelayCommand(_ => CreateNew());
            OpenCommand = new RelayCommand(_ => Open(), _ => true);
            SaveCommand = new RelayCommand(_ => Save(), _ => DataTable.Rows.Count > 0);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => !string.IsNullOrEmpty(CurrentFilePath));
        }

        private void CreateNew()
        {
            // 1. 先詢問要建立多少個欄位
            var dlgInput = new WpfAppSimpleDataManager.Views.InputDialog();
            if (dlgInput.ShowDialog() == true && dlgInput.Result.HasValue)
            {
                int colCount = dlgInput.Result.Value;

                // 2. 選擇儲存檔案位置
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    DefaultExt = ".xlsx"
                };
                if (dlg.ShowDialog() == true)
                {
                    CurrentFilePath = dlg.FileName;

                    // 3. 呼叫 ExcelService 產生空白檔（指定欄位數）
                    DataTable = _excelService.CreateEmpty(CurrentFilePath, colCount);
                }
            }
        }

        private void Open()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx)|*.xlsx",
                DefaultExt = ".xlsx"
            };
            if (dlg.ShowDialog() == true)
            {
                CurrentFilePath = dlg.FileName;
                DataTable = _excelService.Load(CurrentFilePath);
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                MessageBox.Show("請先選擇或建立檔案。");
                return;
            }
            _excelService.Save(DataTable, CurrentFilePath);
            MessageBox.Show("儲存完成。");
        }

        private void Delete()
        {
            if (string.IsNullOrEmpty(CurrentFilePath)) return;
            if (MessageBox.Show($"確定要刪除 '{CurrentFilePath}' 嗎？", "刪除確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _excelService.Delete(CurrentFilePath);
                DataTable = new DataTable();
                CurrentFilePath = string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
