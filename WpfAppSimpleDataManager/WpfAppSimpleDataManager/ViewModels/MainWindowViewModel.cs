using System.ComponentModel;

namespace WpfAppSimpleDataManager.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // 如有需要，可以在此統一管理兩個子 ViewModel 的共享資源
        // 本範例不特別實作，僅保留後續擴充空間

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void RaisePropertyChanged(string propName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
