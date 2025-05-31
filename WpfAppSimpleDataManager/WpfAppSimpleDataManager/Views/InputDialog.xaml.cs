using System.Windows;
using Wpf.Ui.Controls;

namespace WpfAppSimpleDataManager.Views
{
    public partial class InputDialog : FluentWindow
    {
        public int? Result { get; private set; }
        public InputDialog()
        {
            InitializeComponent();
            txtInput.Focus();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtInput.Text, out int value) && value > 0)
            {
                Result = value;
                DialogResult = true;
            }
            else
            {
                System.Windows.MessageBox.Show("請輸入正整數。");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
