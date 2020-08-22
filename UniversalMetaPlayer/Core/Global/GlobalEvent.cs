using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UMP.Utility;

namespace UMP.Core.Global
{
  public delegate void UMP_VoidEventHandler();
  public delegate void UMP_KeyDownEventHandler(KeyEventArgs e);
  public delegate void UMP_ThemeEventHandler(ThemeHelper.ThemeProperty e);

  public static class GlobalMessageEvent
  {
    static GlobalMessageEvent()
    {
      MessageCloseTimer = new System.Timers.Timer(3000);
      MessageCloseTimer.Elapsed += (_, e) => { MessageCloseEvent?.Invoke(); };
    }

    public delegate void UMP_GlobalMessageEventHandler(string message);
    private static System.Timers.Timer MessageCloseTimer { get; }

    public static event UMP_VoidEventHandler MessageCloseEvent;
    public static event UMP_GlobalMessageEventHandler MessageEvent;

    public static void Invoke(string message, bool autoClose = false)
    {
      MessageEvent?.Invoke(message);
      MessageCloseTimer.Stop();
      if (autoClose)
        MessageCloseTimer.Start();
    }
  }

  public static class GlobalKeyDownEvent
  {
    private static object LockObject = new object();
    public static bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 전역 키 누름 이벤트
    /// </summary>
    public static event UMP_KeyDownEventHandler KeyDownEvent;

    /// <summary>
    /// 전역 키 누름 이벤트 호출
    /// </summary>
    public static void Invoke(KeyEventArgs e)
    {
      if (IsEnabled)
        lock (LockObject)
        {
          KeyDownEvent?.Invoke(e);
        }
    }
  }
}
