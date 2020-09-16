using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snatch.Controls
{
  public partial class Header : UserControl
  {
    public Header()
    {
      InitializeComponent();
      Application.Current.MainWindow.Activated += HandleWindowActivated;
    }

    private void HandleWindowActivated(object sender, EventArgs e)
    {
      QueryInput.Focus();
    }
  }
}
