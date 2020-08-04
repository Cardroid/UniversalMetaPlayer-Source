using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Option.OptionControl.ViewModel
{
  public class KeyboardOptionViewModel : ViewModelBase
  {
    public bool GlobalKeyboardHookIsChecked
    {
      get => GlobalProperty.GlobalKeyboardHook;
      set
      {
        GlobalProperty.GlobalKeyboardHook = value;
        OnPropertyChanged("GlobalKeyboardHook");
      }
    }
    public string GlobalKeyboardHookToolTip => $"창이 활성화 되지 않아도 컨트롤을 가능하게 합니다.\nMediaPlayPause, MediaStop, MediaNextTrack, MediaPreviousTrack 버튼을 지원합니다.";
  }
}
