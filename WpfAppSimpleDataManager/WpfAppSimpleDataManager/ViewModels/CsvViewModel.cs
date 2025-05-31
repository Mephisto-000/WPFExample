using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Input;
using WpfAppSimpleDataManager.Services;

namespace WpfAppSimpleDataManager.ViewModels
{
    public class CsvViewModel : INotifyPropertyChanged
    {
        private readonly ICsvService _csvService;

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

        public CsvViewModel()
        {
            // 可改成注入模式 (DI)，此範例直接 new
            _csvService = new CsvService();

            CreateNewCommand = new RelayCommand(_ => CreateNew());
            OpenCommand = new RelayCommand(_ => Open(), _ => true);
            SaveCommand = new RelayCommand(_ => Save(), _ => DataTable.Rows.Count > 0);
            DeleteCommand = new RelayCommand(_ => Delete(), _ => !string.IsNullOrEmpty(CurrentFilePath));
        }

        private void CreateNew()
        {
            // 彈出對話視窗讓使用者輸入欄位數
            var dlgInput = new WpfAppSimpleDataManager.Views.InputDialog();
            if (dlgInput.ShowDialog() == true && dlgInput.Result.HasValue)
            {
                int colCount = dlgInput.Result.Value;

                var dlg = new SaveFileDialog
                {
                    Filter = "CSV Files (*.csv)|*.csv",
                    DefaultExt = ".csv"
                };
                if (dlg.ShowDialog() == true)
                {
                    CurrentFilePath = dlg.FileName;
                    DataTable = _csvService.CreateEmpty(CurrentFilePath, colCount);
                }
            }
        }

        private void Open()
        {
            var dlg = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                DefaultExt = ".csv"
            };
            if (dlg.ShowDialog() == true)
            {
                CurrentFilePath = dlg.FileName;
                DataTable = _csvService.Load(CurrentFilePath);
            }
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(CurrentFilePath))
            {
                MessageBox.Show("請先選擇或建立檔案。");
                return;
            }
            _csvService.Save(DataTable, CurrentFilePath);
            MessageBox.Show("儲存完成。");
        }

        private void Delete()
        {
            if (string.IsNullOrEmpty(CurrentFilePath)) return;
            if (MessageBox.Show($"確定要刪除 '{CurrentFilePath}' 嗎？", "刪除確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _csvService.Delete(CurrentFilePath);
                DataTable = new DataTable(); // 清空畫面
                CurrentFilePath = string.Empty;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
