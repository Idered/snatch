using System;
using System.Windows;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using WK.Libraries.SharpClipboardNS;
using System.ComponentModel;
using System.Collections.Generic;
using System.Configuration;
using Snatch.Properties;
using System.Diagnostics;

namespace Snatch
{
  public partial class MainWindow : Window, INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool ClipboardInitialized = false;
    private readonly SharpClipboard clipboard = new SharpClipboard();
    private readonly DbClient db = new DbClient();

    public MainWindow()
    {
      this.DataContext = this;

      LoadEntries();
      InitializeComponent();
      RegisterHotkey();

      clipboard.ClipboardChanged += ClipboardChanged;
      ProcessWatcher.StartWatch();

      if (Settings.Default.ShowOnboarding)
      {
        this.Visibility = Visibility.Visible;
      }

      HeaderRef.QueryInput.TextChanged += QueryInput_TextChanged;
      HeaderRef.QueryInput.PreviewKeyDown += QueryInput_PreviewKeyDown;
      Activated += HandleWindowActivated;
    }

    private void LoadEntries()
    {
      this.Entries = new ObservableCollection<Entry>(db.Entries.ToList());
      this.HasVisibleEntries = this.Entries.Count() > 0;
    }

    private void RegisterHotkey()
    {
      KeyGesture ToggleGesture = new KeyGesture(Key.V, ModifierKeys.Control | ModifierKeys.Shift);
      HotkeyManager.Current.AddOrReplace("Toggle", ToggleGesture, HandleToggle);
    }

    private ObservableCollection<Entry> _entries = new ObservableCollection<Entry>();
    public ObservableCollection<Entry> Entries
    {
      get
      {
        return _entries;
      }

      set
      {
        if (value != _entries)
        {
          _entries = value;
          NotifyPropertyChanged();
        }
      }
    }

    private bool _hasVisibleEntries = true;
    public bool HasVisibleEntries
    {
      get
      {
        return _hasVisibleEntries;
      }

      set
      {
        if (value != _hasVisibleEntries)
        {
          _hasVisibleEntries = value;
          NotifyPropertyChanged();
        }
      }
    }

    private void HandleToggle(object sender, HotkeyEventArgs e)
    {
      if (Settings.Default.OnboardingStep == 0)
      {
        Settings.Default.OnboardingStep += 1;
        Activate();
        HeaderRef.QueryInput.Focus();
        return;
      }
      if (Settings.Default.OnboardingStep == 1)
      {
        Settings.Default.OnboardingStep += 1;
        Settings.Default.ShowOnboarding = false;
        Settings.Default.Save();
        Activate();
        HeaderRef.QueryInput.Focus();
        return;
      }
      if (this.Visibility == Visibility.Visible)
      {
        this.Visibility = Visibility.Hidden;
      }
      else
      {
        this.Visibility = Visibility.Visible;
        Activate();
        HeaderRef.QueryInput.Focus();
        Settings.Default.ToggleCount += 1;
        if (Settings.Default.ToggleCount >= 50 && Settings.Default.HasClosedDonateInfo == false)
        {
          Settings.Default.ShowDonateInfo = true;
        }
        Settings.Default.Save();
      }
    }

    private void ClipboardChanged(object sender, SharpClipboard.ClipboardChangedEventArgs e)
    {
      if (ClipboardInitialized == false)
      {
        ClipboardInitialized = true;
        return;
      }

      if (e.ContentType == SharpClipboard.ContentTypes.Text)
      {
        bool alreadyExists = this.Entries.Where((item) => item.Content == clipboard.ClipboardText).Any();

        if (clipboard.ClipboardText.Length > 0 && !alreadyExists)
        {
          Entry entry = new Entry() { Content = clipboard.ClipboardText };
          db.Add(entry);
          this.Entries.Insert(0, entry);
          db.SaveChanges();
        }
      }
    }

    private void OnItemMouseDown(object sender, MouseButtonEventArgs e)
    {
      HandleSelectedItem(sender);
    }

    private void OnItemKeyUp(object sender, KeyEventArgs e)
    {
      if (Key.Return != e.Key)
      {
        return;
      }

      HandleSelectedItem(sender);
    }

    private void HandleSelectedItem(object sender)
    {
      ListView view = sender as ListView;
      Entry selected = view.SelectedItem as Entry;

      if (selected != null && selected.IsVisible)
      {
        Settings.Default.CopyCount += 1;
        Settings.Default.Save();
        clipboard.ClipboardChanged -= ClipboardChanged;
        Clipboard.SetText(selected.Content);
        clipboard.ClipboardChanged += ClipboardChanged;
        ProcessWatcher.BringMainWindowToFront();
        System.Windows.Forms.SendKeys.SendWait("^v");
        this.Visibility = Visibility.Hidden;
      }
    }

    public void QuitKeybinding(object sender, object e)
    {
      Application.Current.Shutdown();
    }

    public void CloseWindowKeybinding(object sender, object e)
    {
      this.Visibility = Visibility.Hidden;
    }

    public void TogglePinAllKeybinding(object sender, object e)
    {
      if (!this.Entries.Any())
      {
        return;
      }

      Entry first = this.Entries.First();
      bool synced = this.Entries.Where(item => item.IsPinned == first.IsPinned).Count() == this.Entries.Count;
      bool isPinned = !synced || !first.IsPinned;

      if (first != null)
      {
        foreach (Entry item in this.Entries)
        {
          item.IsPinned = isPinned;
          db.Entries.Update(item);
        }

        db.SaveChanges();
      }
    }


    public void ToggleCheatsheetKeybinding(object sender, object e)
    {
      Settings.Default.ShowShortcuts = !Settings.Default.ShowShortcuts;
      Settings.Default.Save();
    }

    public void PinKeybinding(object sender, object e)
    {
      Entry entry = uiItems.SelectedItem as Entry;
      if (entry == null)
      {
        return;
      }

      entry.IsPinned = !entry.IsPinned;
      db.Entries.Update(entry);
      db.SaveChanges();
    }

    private void ClearList()
    {
      List<Entry> entriesToRemove = this.Entries.Where(item => item.IsPinned == false).ToList();
      Utils.RemoveAll(this.Entries, entriesToRemove);
      db.Entries.RemoveRange(entriesToRemove);
      db.SaveChanges();
      uiItems.SelectedIndex = 0;
    }

    public void ClearKeybinding(object sender, object e)
    {
      ClearList();
    }

    private void QueryInput_TextChanged(object sender, TextChangedEventArgs e)
    {
      int visibleCount = 0;

      foreach (Entry entry in this.Entries)
      {
        if (HeaderRef.QueryInput.Text.Trim() == "")
        {
          visibleCount++;
          entry.IsVisible = true;
          continue;
        }
        entry.IsVisible = entry.Content.IndexOf(HeaderRef.QueryInput.Text, StringComparison.OrdinalIgnoreCase) >= 0;
        visibleCount = entry.IsVisible ? visibleCount + 1 : visibleCount;
      }

      this.HasVisibleEntries = visibleCount > 0;
    }

    private void QueryInput_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (this.Entries.Count() == 0)
      {
        return;
      }

      Entry selected = uiItems.SelectedItem as Entry;
      IList<Entry> visibleEntries = this.Entries.Where(item => item.IsVisible == true).ToList();
      int selectedVisibleIndex = visibleEntries.IndexOf(selected);

      switch (e.Key)
      {
        case Key.Delete:
          if (Keyboard.IsKeyDown(Key.LeftCtrl))
          {
            int lastIndex = selectedVisibleIndex;
            db.Entries.Remove(selected);
            db.SaveChangesAsync();
            this.Entries.RemoveAt(uiItems.SelectedIndex);
            uiItems.SelectedItem = visibleEntries.ElementAt(Math.Min(lastIndex+1, visibleEntries.Count() - 1));
            uiItems.ScrollIntoView(uiItems.SelectedItem);
          }
          break;
        case Key.PageDown:
          uiItems.SelectedItem = visibleEntries.ElementAt(Math.Min(selectedVisibleIndex + 7, visibleEntries.Count() - 1));
          uiItems.ScrollIntoView(uiItems.SelectedItem);
          break;
        case Key.PageUp:
          uiItems.SelectedItem = visibleEntries.ElementAt(Math.Max(selectedVisibleIndex - 7, 0));
          uiItems.ScrollIntoView(uiItems.SelectedItem);
          break;
        case Key.Down:
          uiItems.SelectedItem = visibleEntries.ElementAt((selectedVisibleIndex + 1) % visibleEntries.Count());
          uiItems.ScrollIntoView(uiItems.SelectedItem);
          break;
        case Key.Up:
          int v = (selectedVisibleIndex - 1);
          uiItems.SelectedItem = visibleEntries.ElementAt(v < 0 ? visibleEntries.Count() - 1 : v); ;
          uiItems.ScrollIntoView(uiItems.SelectedItem);
          break;
        case Key.Enter:
          HandleSelectedItem(uiItems);
          break;
        default:
          break;
      }
    }

    private void HandleWindowActivated(object sender, EventArgs e)
    {
      if (this.Entries.Count() == 0)
      {
        return;
      }

      if (uiItems.SelectedItem == null)
      {
        uiItems.SelectedIndex = 0;
      }

      uiItems.ScrollIntoView(uiItems.SelectedItem);
    }
  }
}
