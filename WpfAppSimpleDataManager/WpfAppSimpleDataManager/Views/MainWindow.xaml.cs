using System;
using System.Windows;
using System.Windows.Input;
using Wpf.Ui.Controls;
using WpfAppSimpleDataManager.ViewModels;

namespace WpfAppSimpleDataManager.Views
{
    public partial class MainWindow : FluentWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            // 設定 MainWindow 的 DataContext
            this.DataContext = new MainWindowViewModel();
        }
    }
}