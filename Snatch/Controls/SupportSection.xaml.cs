using System.Windows.Controls;
using System.Windows.Input;
using Snatch.Properties;

namespace Snatch.Controls
{
  public partial class SupportSection : UserControl
  {
    public SupportSection()
    {
      InitializeComponent();
    }

    private void HideDonationSection(object sender, MouseButtonEventArgs e)
    {
      Settings.Default.ShowDonateInfo = false;
      Settings.Default.HasClosedDonateInfo = true;
      Settings.Default.Save();
    }
  }
}
