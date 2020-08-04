using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Option.OptionControl.ViewModel
{
  public class BasicOptionViewModel : ViewModelBase
  {
    public BasicOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (e) =>
      {
        if (e == "FileSavePath") OnPropertyChanged("FileSavePathToolTip");
      };
    }

    public string FileSavePathToolTip => $"프로그램에서 저장하는 파일을 보관하는 폴더입니다.\n빈 폴더를 권장합니다.\n현재 설정 : \"{Path.GetFullPath(GlobalProperty.FileSavePath)}\"";

    public bool PrivateLoggingIsChecked
    {
      get => GlobalProperty.PrivateLogging;
      set
      {
        GlobalProperty.PrivateLogging = value;
        OnPropertyChanged("PrivateLoggingIsChecked");
      }
    }

    public string PrivateLoggingToolTip => "로그에 더 자세한 사항을 기록합니다.\n개인 정보가 포함 될 수 있으나, 모든 정보는 익명으로 저장됩니다.";
  }
}
