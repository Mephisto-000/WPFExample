using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WpfToDoList.Models;
using Microsoft.Data.SqlClient;
using System.Windows;

namespace WpfToDoList.ViewModels
{
    public class TaskViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Tasks> _tasks;
        public ObservableCollection<Tasks> Tasks
        {
            get => _tasks;
            set { _tasks = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> PriorityList { get; set; }
            = new ObservableCollection<string> { "Priority 1", "Priority 2", "Priority 3", "Priority 4", "Priority 5" };

        private string? _selectedPriority;
        public string? SelectedPriority
        {
            get => _selectedPriority;
            set { _selectedPriority = value; OnPropertyChanged(); }
        }

        private string? _newTaskContent;
        public string? NewTaskContent
        {
            get => _newTaskContent;
            set { _newTaskContent = value; OnPropertyChanged(); }
        }

        // 綁定 DataGrid SelectedItem
        private Tasks? _selectedTask;
        public Tasks? SelectedTask
        {
            get => _selectedTask;
            set { _selectedTask = value; OnPropertyChanged(); }
        }

        // 連線字串：改成你的專案絕對路徑
        private readonly string connStr = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Github\WPFExample\WpfToDoList\WpfToDoList\ToDoListDB.mdf;Integrated Security=True;Connect Timeout=30";

        public TaskViewModel()
        {
            Tasks = LoadTasks();
            SelectedPriority = PriorityList[0];
        }

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
                MessageBox.Show("讀取資料庫失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return tasks;
        }

        public void AddTask()
        {
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
                    string sql = "INSERT INTO TbToDoList (Content, Priority, Date) VALUES (@Content, @Priority, @Date)";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Content", NewTaskContent);
                        cmd.Parameters.AddWithValue("@Priority", SelectedPriority);
                        cmd.Parameters.AddWithValue("@Date", DateTime.Now);
                        cmd.ExecuteNonQuery();
                    }
                }
                Tasks = LoadTasks();
                NewTaskContent = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("新增資料失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void DeleteTask()
        {
            if (SelectedTask == null)
            {
                MessageBox.Show("請先選取要刪除的任務", "提醒", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                using (var conn = new SqlConnection(connStr))
                {
                    conn.Open();
                    string sql = "DELETE FROM TbToDoList WHERE Id = @Id";
                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Id", SelectedTask.Id);
                        cmd.ExecuteNonQuery();
                    }
                }
                Tasks = LoadTasks();
                SelectedTask = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("刪除資料失敗：" + ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
