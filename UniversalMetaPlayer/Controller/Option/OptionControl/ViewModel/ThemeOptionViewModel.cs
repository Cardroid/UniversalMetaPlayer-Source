using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Option.OptionControl.ViewModel
{
  public class ThemeOptionViewModel : ViewModelBase
  {
    public bool IsAverageColorThemeIsChecked
    {
      get => GlobalProperty.IsAverageColorTheme;
      set
      {
        GlobalProperty.IsAverageColorTheme = value;
        OnPropertyChanged("IsAverageColorThemeIsChecked");
      }
    }

    public string IsAverageColorThemeToolTip => "엘범 커버 이미지의 픽셀 평균색을 추출하여 자동으로 테마를 변경합니다.";
    public string AverageColorProcessingOffsetToolTip => "엘범 커버 이미지의 픽셀 평균색을 추출하여 자동으로 테마를 변경합니다.\n기본값 : 30";
  }
}
