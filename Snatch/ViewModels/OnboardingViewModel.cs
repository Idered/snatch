
using System.Windows.Input;

namespace Snatch
{
  public class OnboardingViewModel: System.ComponentModel.INotifyPropertyChanged
  {
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }


    private bool isPressingCtrl = false;
    public bool IsPressingCtrl
    {
      get { return isPressingCtrl; }
      set
      {
        if (value != isPressingCtrl)
        {
          isPressingCtrl = value;
          NotifyPropertyChanged();
        }
      }
    }


    private bool isPressingShift = false;
    public bool IsPressingShift
    {
      get { return isPressingShift; }
      set
      {
        if (value != isPressingShift)
        {
          isPressingShift = value;
          NotifyPropertyChanged();
        }
      }
    }


    private bool isPressingV = false;
    public bool IsPressingV
    {
      get { return isPressingV; }
      set
      {
        if (value != isPressingV)
        {
          isPressingV = value;
          NotifyPropertyChanged();
        }
      }
    }
    public OnboardingViewModel()
    {
      System.Windows.Application.Current.MainWindow.PreviewKeyUp += HandleWindowKeyUp;
      System.Windows.Application.Current.MainWindow.PreviewKeyDown += HandleWindowKeyDown;
    }

    private void HandleWindowKeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.LeftCtrl)
      {
        IsPressingCtrl = true;
      }
      if (e.Key == Key.LeftShift)
      {
        IsPressingShift = true;
      }
      if (e.Key == Key.V)
      {
        IsPressingV = true;
      }
    }

    private void HandleWindowKeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.LeftCtrl)
      { 
        IsPressingCtrl = false;
      }
      if (e.Key == Key.LeftShift)
      {
        IsPressingShift = false;
      }
      if (e.Key == Key.V)
      {
        IsPressingV = false;
      }
    }
  }
}
