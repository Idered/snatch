using System;
using System.Windows.Input;

namespace Snatch
{
  public class CustomCommand : ICommand
  {
    public event EventHandler<object> Executed;

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      Executed?.Invoke(this, parameter);
    }

    public event EventHandler CanExecuteChanged;
  }
}
