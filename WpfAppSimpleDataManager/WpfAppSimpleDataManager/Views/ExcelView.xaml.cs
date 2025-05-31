using System.Windows.Controls;
using Wpf.Ui.Controls;
using WpfAppSimpleDataManager.ViewModels;

namespace WpfAppSimpleDataManager.Views
{
    public partial class ExcelView : UserControl
    {
        public ExcelView()
        {
            InitializeComponent();
            // 在這裡設定 DataContext
            this.DataContext = new ExcelViewModel();
        }
    }
}
