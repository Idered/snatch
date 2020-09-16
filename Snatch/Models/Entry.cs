using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Snatch
{
  [Table("entry")]
  public class Entry : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    private int _id;
    public int Id {
       get
      {
        return _id;
      }
      set
      {
        _id = value;
        NotifyPropertyChanged();
      }
    }

    private string _content = String.Empty;
    public string Content
    {
      get
      {
        return _content;
      }

      set
      {
        if (value != _content)
        {
          _content = value;
          NotifyPropertyChanged();
        }
      }
    }

    private bool _isPinned = false;
    public bool IsPinned
    {
      get
      {
        return _isPinned;
      }

      set
      {
        if (value != _isPinned)
        {
          _isPinned = value;
          NotifyPropertyChanged();
        }
      }
    }

    private bool _isVisible = true;
    [NotMapped]
    public bool IsVisible
    {
      get
      {
        return _isVisible;
      }

      set
      {
        if (value != _isVisible)
        {
          _isVisible = value;
          NotifyPropertyChanged();
        }
      }
    }

    public string DisplayName
    {
      get
      {
        Regex regex = new Regex("[ ]{2,}", RegexOptions.None);
        string withoutNewline = this.Content.Replace("\r\n", "\n").Replace("\n", " ");

        return regex.Replace(withoutNewline, " ").TrimStart();
      }
    }
  }
}
