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
using CMP2.Core;

namespace CMP2.Utility
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
    }

    private static InputSource inputSource { get; set; }
    private static KeyboardEventReceiver keyboardReceiver { get; }

    public static void Start()
    {
      var log = new Log(typeof(Hook));
      log.Info("전역 키보드 후킹을 시작하였습니다.");
      if (inputSource == null)
        inputSource = new InputSource(keyboardReceiver);
      inputSource.Listen();
    }

    public static void Dispose()
    {
      var log = new Log(typeof(Hook));
      log.Info("전역 키보드 후킹을 종료하였습니다.");
      inputSource?.Dispose();
      inputSource = null;
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
