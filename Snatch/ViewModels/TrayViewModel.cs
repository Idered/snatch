using System.Windows;
using System.Windows.Input;

namespace Snatch
{
  public class TrayViewModel
  {
    public ICommand ShowApplicationCommand
    {
      get
      {
        return new DelegateCommand()
        {
          CommandAction = () =>
          {
            Application.Current.MainWindow.Show();
            Application.Current.MainWindow.Activate();
          }
        };
      }
    }

    public ICommand ExitApplicationCommand
    {
      get
      {
        return new DelegateCommand()
        {
          CommandAction = () => Application.Current.Shutdown()
        };
      }
    }
  }
}
