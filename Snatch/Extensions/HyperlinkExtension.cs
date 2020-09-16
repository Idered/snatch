using System.Windows;

namespace Snatch
{
  public static class HyperlinkExtension
  {
    public static bool GetIsExternal(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsExternalProperty);
    }

    public static void SetIsExternal(DependencyObject obj, bool value)
    {
      obj.SetValue(IsExternalProperty, value);
    }
    public static readonly DependencyProperty IsExternalProperty =
        DependencyProperty.RegisterAttached("IsExternal", typeof(bool), typeof(HyperlinkExtension), new UIPropertyMetadata(false, OnIsExternalChanged));

    private static void OnIsExternalChanged(object sender, DependencyPropertyChangedEventArgs args)
    {
      System.Windows.Documents.Hyperlink hyperlink = sender as System.Windows.Documents.Hyperlink;

      if ((bool)args.NewValue)
      {
        hyperlink.RequestNavigate += Hyperlink_RequestNavigate;
      }
      else
      {
        hyperlink.RequestNavigate -= Hyperlink_RequestNavigate;
      }
    }

    private static void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
      e.Handled = true;
    }
  }
}
