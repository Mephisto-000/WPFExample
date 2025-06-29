using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfToDoList.Models; // 這邊會找 Tasks 類別
using Microsoft.Data.SqlClient;
using System.Windows;

namespace WpfToDoList.ViewModels
{
    /// <summary>
    /// ViewModel：負責 UI 的資料繫結、資料存取、與新增功能
    /// </summary>
    public class TaskViewModel : INotifyPropertyChanged
    {
        // 用於 DataGrid 綁定的任務清單（注意這裡型別是 Tasks）
        private ObservableCollection<Tasks> _tasks;
        public ObservableCollection<Tasks> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        // 下拉選單的 Priority 清單（五個選項）
        public ObservableCollection<string> PriorityList { get; set; }
            = new ObservableCollection<string> { "Priority 1", "Priority 2", "Priority 3", "Priority 4", "Priority 5" };

        // ComboBox 選定的 Priority
        private string? _selectedPriority;
        public string? SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        // 新任務內容的 TextBox 綁定屬性
        private string? _newTaskContent;
        public string? NewTaskContent
        {
            get => _newTaskContent;
            set { _newTaskContent = value; OnPropertyChanged(); }
        }

        // 資料庫連線字串
        private readonly string connStr;

        /// <summary>
        /// 建構子：初始化資料來源、設定預設值
        /// </summary>
        public TaskViewModel()
        {
            // 直接指定 mdf 絕對路徑（不用 AppDomain.CurrentDomain.BaseDirectory）
            string dbPath = @"C:\Github\WPFExample\WpfToDoList\WpfToDoList\ToDoListDB.mdf";
            connStr = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbPath};Integrated Security=True;Connect Timeout=30";

            // 載入資料庫內容
            Tasks = LoadTasks();

            // 預設選第一個 Priority
            SelectedPriority = PriorityList[0];
        }

        /// <summary>
        /// 從資料庫載入所有 Tasks 資料
        /// </summary>
        public ObservableCollection<Tasks> LoadTasks()
        {
            var tasks = new ObservableCollection<Tasks>();
            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "SELECT Id, Content, Priority, Date FROM TbToDoList";
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tasks.Add(new Tasks
                            {
                                Id = reader.GetInt32(0),
                                Content = reader.IsDBNull(1) ? null : reader.GetString(1),
                                Priority = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Date = reader.GetDateTime(3)
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"讀取資料庫失敗: {ex.Message}");
                MessageBox.Show("讀取資料庫失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tasks;
        }

        /// <summary>
        /// 新增一筆任務到資料庫，並刷新 Tasks
        /// </summary>
        public void AddTask()
        {
            // 檢查輸入值
            if (string.IsNullOrWhiteSpace(NewTaskContent) || string.IsNullOrWhiteSpace(SelectedPriority))
            {
                MessageBox.Show("請先選擇優先順序並輸入內容！", "提醒", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    // 不要給 Id，讓資料庫自動編號
                    string sql = "INSERT INTO TbToDoList (Content, Priority, Date) VALUES (@Content, @Priority, @Date)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Content", NewTaskContent);
                        cmd.Parameters.AddWithValue("@Priority", SelectedPriority);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                // 新增後即時刷新 DataGrid
                Tasks = LoadTasks();
                // 清空文字方塊
                NewTaskContent = "";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"新增資料失敗: {ex.Message}");
                MessageBox.Show("新增資料失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region INotifyPropertyChanged 實作
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
