using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Snatch.Controls
{
  public partial class Onboarding : UserControl
  {
    public Onboarding()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
      Snatch.Properties.Settings.Default.OnboardingStep = 0;
    }
  }
}
