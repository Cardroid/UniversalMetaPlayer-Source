using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UMP.Core.Global;
using UMP.Core.Model.ViewModel;

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
      get => GlobalProperty.Options.Getter<bool>(Enums.ValueName.GlobalKeyboardHook);
      set => GlobalProperty.Options.Setter(Enums.ValueName.GlobalKeyboardHook, value.ToString());
    }
    public string GlobalKeyboardHookToolTip =>
      $"창이 활성화 되지 않아도 컨트롤을 가능하게 합니다.\n" +
      $"관리자 권한으로 실행된 프로그램의 키 입력은, 본 프로그램에 관리자 권한이 없으면 캡쳐하지 못할 수 있습니다.\n" +
      $"MediaPlayPause, MediaStop, MediaNextTrack, MediaPreviousTrack 버튼을 지원합니다.";

    public bool HotKeyIsChecked
    {
      get => GlobalProperty.Options.Getter<bool>(Enums.ValueName.HotKey);
      set => GlobalProperty.Options.Setter(Enums.ValueName.HotKey, value.ToString());
    }
    public string HotKeyToolTip =>
      $"단축키 사용을 활성화 합니다.";
  }
}
