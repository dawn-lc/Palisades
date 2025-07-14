using System.Windows;
using System.Windows.Input;
using Palisades.Helpers;
using System.ComponentModel;

namespace Palisades.ViewModel
{
    public class NotifyIconViewModel : INotifyPropertyChanged
    {
        public ICommand ShowWindowCommand => new RelayCommand(ShowSettings);
        public ICommand ExitApplicationCommand => new RelayCommand(ExitApplication);

        public static string SettingsText => Loc.Get("TraySettings") ?? "设置";
        public static string ExitText => Loc.Get("TrayExit") ?? "退出";

        public NotifyIconViewModel()
        {
            Loc.LanguageChanged += (s, e) =>
            {
                OnPropertyChanged(nameof(SettingsText));
                OnPropertyChanged(nameof(ExitText));
            };
        }

        private void ShowSettings()
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                var settingsWindow = new View.Settings
                {
                    Owner = mainWindow
                };
                settingsWindow.ShowDialog();
            }
        }

        private void ExitApplication()
        {
            // 关闭所有窗口，这会触发资源清理
            foreach (Window window in Application.Current.Windows)
            {
                window.Close();
            }
            // 关闭应用程序
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}