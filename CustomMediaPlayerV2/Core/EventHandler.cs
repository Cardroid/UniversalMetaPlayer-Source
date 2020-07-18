﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CMP2.Core
{
  public delegate void CMP_PropertyChangedEventHandler(string propertyname);
  public delegate void CMP_KeyDownEventHandler(KeyEventArgs e);

  public static class GlobalEvent
  {
    private static bool KeyDownEventHandled = false;
    /// <summary>
    /// 전역 키 누름 이벤트
    /// </summary>
    public static event CMP_KeyDownEventHandler KeyDownEvent;

    /// <summary>
    /// 전역 키 누름 이벤트 호출
    /// </summary>
    public static async void KeyDownEventInvoke(KeyEventArgs e)
    {
      if (KeyDownEventHandled)
        return;
      KeyDownEventHandled = true;
      KeyDownEvent?.Invoke(e);
      await Task.Delay(GlobalProperty.KeyEventDelay);
      KeyDownEventHandled = false;
    }
  }
}
