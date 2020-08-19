using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class KeyboardOptionViewModel : ViewModelBase
  {
    public KeyboardOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "GlobalKeyboardHook")
          OnPropertyChanged("GlobalKeyboardHookIsChecked");
      };
    }
    public bool GlobalKeyboardHookIsChecked
    {
      get => GlobalProperty.Options.GlobalKeyboardHook;
      set => GlobalProperty.Options.GlobalKeyboardHook = value;
    }
    public string GlobalKeyboardHookToolTip =>
      $"창이 활성화 되지 않아도 컨트롤을 가능하게 합니다.\n" +
      $"MediaPlayPause, MediaStop, MediaNextTrack, MediaPreviousTrack 버튼을 지원합니다.";
    public string KeyEventDelayOffsetToolTip =>
      $"[실험적 기능]\n" +
      $"기본값 : {GlobalProperty.Options.DefaultValue.KeyEventDelay} (최소 : 10 최대 : 200)\n\n" +

      $"키 입력이 처리된 후, 다음키보드 입력을 받는데 걸리는 시간 입니다.";
  }
}
