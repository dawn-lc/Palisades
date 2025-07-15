using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Palisades.Helpers;
using Sentry;

namespace Palisades
{
    public partial class App : Application
    {
        private TaskbarIcon? taskbar;

        public App()
        {
            SettingsManager.Instance.ApplyLanguage();

            SetupSentry();

            PalisadesManager.LoadPalisades();
            if (PalisadesManager.palisades.Count == 0)
            {
                PalisadesManager.CreatePalisade();
            }

            Exit += App_Exit;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            taskbar = (TaskbarIcon)FindResource("Taskbar");
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            // 释放所有Palisade窗口资源
            foreach (Window window in Windows.Cast<Window>().ToList())
            {
                if (window.DataContext is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }

            // 释放托盘图标
            if (taskbar != null)
            {
                taskbar.Dispose();
                taskbar = null;
            }
        }

        private void SetupSentry()
        {
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            SentrySdk.Init(o =>
            {
                o.Dsn = "https://b6cda744e340ad237cf8d10022ee6daf@o687854.ingest.sentry.io/4509671838908416";
                o.Debug = PEnv.IsDev();
                o.TracesSampleRate = 1;
            });
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            SentrySdk.CaptureException(e.Exception);
            e.Handled = true;
        }
    }
}
