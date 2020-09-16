using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;

namespace Snatch
{
  public partial class App : Application
  {
    private TaskbarIcon trayIcon;

    private App()
    {
      Deactivated += Application_Deactivated;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      trayIcon = (TaskbarIcon)FindResource("TrayIcon");
    }

    protected override void OnExit(ExitEventArgs e)
    {
      trayIcon.Dispose();
      base.OnExit(e);
    }

    private void Application_Deactivated(object sender, System.EventArgs e)
    {
      Application.Current.MainWindow.Visibility = Visibility.Hidden;
    }
  }
}
