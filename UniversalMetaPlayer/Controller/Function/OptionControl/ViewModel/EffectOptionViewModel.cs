using System;
using System.Collections.Generic;
using System.Text;

using UMP.Controller.WindowHelper;
using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player.Plugin.Control;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class EffectOptionViewModel : ViewModelBase
  {
    public EffectOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "IsUseFadeEffect")
          OnPropertyChanged("FadeEffectIsChecked");
      };

      VarispeedChangerParameterControlOpen = new RelayCommand((o) => { WindowManager.VarispeedChangerParameterControlWindowOpen(); });
    }

    #region 흐려짐 효과
    public bool FadeEffectIsChecked
    {
      get => GlobalProperty.Options.Getter<bool>(Enums.ValueName.IsUseFadeEffect);
      set => GlobalProperty.Options.Setter(Enums.ValueName.IsUseFadeEffect, value.ToString());
    }

    public string FadeEffectToolTip =>
      $"흐려짐 효과(Fade Effect)를 사용합니다.\n" +
      $"재생, 일시정지 및 정지에 서서히 소리가 변화하는 FadeIn, FadeOut 효과를 넣습니다.";

    public string FadeEffectDelayToolTip =>
      $"기본값 : {GlobalProperty.DefaultValue.GetDefaultValue<int>(Enums.ValueName.FadeEffectDelay)} (최소 : 1 최대 : 3000)\n" +
      $"*흐려짐 효과를 사용해야 합니다.\n\n" +

      $"흐려짐 효과의 지연시간입니다. (단위 : Milliseconds)\n" +
      $"너무 긴 지연시간은 미디어 컨트롤에 영향을 줄 수 있습니다.";
    #endregion

    #region 오디오 효과
    public RelayCommand VarispeedChangerParameterControlOpen { get; }
    public string VarispeedChangerToolTip =>
      $"오디오 효과(속도, 템포, 피치)를 조절합니다.";
    #endregion
  }
}
