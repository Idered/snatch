using System;
using System.Timers;

namespace Snatch
{
  public static class ProcessWatcher
  {
    public static void StartWatch()
    {
      _timer = new Timer(100);
      _timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
      _timer.Start();
    }

    static void timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      SetLastActive();
    }

    public static IntPtr LastHandle
    {
      get
      {
        return _previousToLastHandle;
      }
    }

    public static void BringMainWindowToFront()
    {
      //// check if the window is hidden / minimized
      if (LastHandle == IntPtr.Zero)
      {
        // the window is hidden so try to restore it before setting focus.
        NativeApi.ShowWindow(LastHandle, ShowWindowEnum.Restore);
      }

      // set user the focus to the window
      NativeApi.SetForegroundWindow(LastHandle);
    }

    private static void SetLastActive()
    {
      IntPtr currentHandle = NativeApi.GetForegroundWindow();
      if (currentHandle != _previousHandle)
      {
        _previousToLastHandle = _previousHandle;
        _previousHandle = currentHandle;
      }
    }

    private static Timer _timer;
    private static IntPtr _previousHandle = IntPtr.Zero;
    private static IntPtr _previousToLastHandle = IntPtr.Zero;
  }

}
