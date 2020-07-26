using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using System.Windows;
using NeatInput.Windows;
using NeatInput.Windows.Events;
using System.Windows.Threading;
using UMP.Core;

namespace UMP.Utility
{
  /// <summary>
  /// 전역 마우스 키보드 후킹을 통한 원격 조종 설정
  /// </summary>
  public static class Hook
  {
    static Hook()
    {
      keyboardReceiver = new KeyboardEventReceiver();
      inputSource = new InputSource(keyboardReceiver);
      Started = false;
    }

    private static InputSource inputSource { get; set; }
    private static KeyboardEventReceiver keyboardReceiver { get; }
    private static bool Started;

    public static void Start()
    {
      if (Started)
        return;

      var log = new Log(typeof(Hook));
      if (inputSource == null)
        inputSource = new InputSource(keyboardReceiver);
      inputSource.Listen();
      Started = true;
      log.Info("전역 키보드 후킹을 시작하였습니다.");
    }

    public static void Dispose()
    {
      if (!Started)
        return;

      var log = new Log(typeof(Hook));
      inputSource?.Dispose();
      inputSource = null;
      Started = false;
      log.Info("전역 키보드 후킹을 종료하였습니다.");
    }

    public static event KeyboardEventHandler KeyboardEvent
    {
      add => keyboardReceiver.KeyboardEvent += value;
      remove => keyboardReceiver.KeyboardEvent -= value;
    }
    public delegate void KeyboardEventHandler(KeyboardEvent e);

    private class KeyboardEventReceiver : IKeyboardEventReceiver
    {
      public event KeyboardEventHandler KeyboardEvent;
      public void Receive(KeyboardEvent e) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => KeyboardEvent?.Invoke(e)));
    }
  }
}
