using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace CMP2.Core
{
  public delegate void CMP_PropertyChangedEventHandler(string propertyname);
  public delegate void CMP_KeyDownEventHandler(KeyEventArgs e);

  public static class GlobalEvent
  {
    /// <summary>
    /// 전역 키 누름 이벤트
    /// </summary>
    public static event CMP_KeyDownEventHandler KeyDownEvent;

    /// <summary>
    /// 전역 키 누름 이벤트 호출
    /// </summary>
    public static void KeyDownEventInvoke(KeyEventArgs e) => KeyDownEvent?.Invoke(e);
  }
}
