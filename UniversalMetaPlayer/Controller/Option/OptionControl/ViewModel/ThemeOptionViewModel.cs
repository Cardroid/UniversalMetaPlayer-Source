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
      get => GlobalProperty.Options.IsAverageColorTheme;
      set
      {
        GlobalProperty.Options.IsAverageColorTheme = value;
        OnPropertyChanged("IsAverageColorThemeIsChecked");
      }
    }

    public string IsAverageColorThemeToolTip => "엘범 커버 이미지의 픽셀 평균색을 추출하여 자동으로 테마를 변경합니다.";
    public string AverageColorProcessingOffsetToolTip => "기본값 : 30 (최소 : 1 최대 : 500)\n*적응형 색 테마를 사용해야 합니다.\n\n픽셀 평균색을 추출할때 건너뛰는 픽셀 갯수입니다.\n낮을 수록 정확하지만 오래걸립니다.\n값이 1 이라면 모든 픽셀의 값을 가져옵니다.";
  }
}
