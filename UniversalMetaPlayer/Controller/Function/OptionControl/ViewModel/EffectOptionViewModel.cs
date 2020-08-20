using System;
using System.Collections.Generic;
using System.Text;

using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Model.ViewModel;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class EffectOptionViewModel : ViewModelBase
  {
    public EffectOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "FadeEffect")
          OnPropertyChanged("FadeEffectIsChecked");
      };
    }

    public bool FadeEffectIsChecked
    {
      get => GlobalObj.Property.Options.Getter<bool>(Enums.ValueName.IsUseFadeEffect);
      set => GlobalObj.Property.Options.Setter(Enums.ValueName.IsUseFadeEffect, value.ToString());
    }
    public string FadeEffectToolTip =>
      $"흐려짐 효과(Fade Effect)를 사용합니다\n" +
      $"재생, 일시정지 및 정지에 서서히 소리가 작아지는 FadeIn, FadeOut 효과를 넣습니다";

    public string FadeEffectDelayToolTip =>
      $"기본값 : {GlobalObj.Property.DefaultValue.GetDefaultValue<int>(Enums.ValueName.FadeEffectDelay)} (최소 : 1 최대 : 3000)\n" +
      $"*흐려짐 효과를 사용해야 합니다.\n\n" +

      $"흐려짐 효과의 지연시간입니다 (단위 : Milliseconds)\n" +
      $"너무 긴 지연시간은 미디어 컨트롤에 영향을 줄 수 있습니다";
  }
}
