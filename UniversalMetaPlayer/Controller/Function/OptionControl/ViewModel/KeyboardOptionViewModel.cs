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
      $"MediaPlayPause, MediaStop, MediaNextTrack, MediaPreviousTrack 버튼을 지원합니다.";
  }
}
