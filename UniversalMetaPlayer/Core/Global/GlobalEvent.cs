﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using UMP.Utility;

namespace UMP.Core.Global
{
  public delegate void UMP_VoidEventHandler();
  public delegate void UMP_GlobalMessageEventHandler(string message, bool autoClose);
  public delegate void UMP_KeyDownEventHandler(KeyEventArgs e);
  public delegate void UMP_ThemeEventHandler(ThemeHelper.ThemeProperty e);

  public static class GlobalEvent
  {
    public static event UMP_GlobalMessageEventHandler GlobalMessageEvent;

    public static void GlobalMessageEventInvoke(string message, bool autoClose = false) => GlobalMessageEvent?.Invoke(message, autoClose);

    public static bool KeyDownEventHandled { get; set; } = false;
    /// <summary>
    /// 전역 키 누름 이벤트
    /// </summary>
    public static event UMP_KeyDownEventHandler KeyDownEvent;

    /// <summary>
    /// 전역 키 누름 이벤트 호출
    /// </summary>
    public static async void KeyDownEventInvoke(KeyEventArgs e)
    {
      if (KeyDownEventHandled)
        return;
      KeyDownEventHandled = true;
      KeyDownEvent?.Invoke(e);
      await Task.Delay(GlobalProperty.Options.Getter<int>(Enums.ValueName.KeyEventDelay));
      KeyDownEventHandled = false;
    }
  }
}