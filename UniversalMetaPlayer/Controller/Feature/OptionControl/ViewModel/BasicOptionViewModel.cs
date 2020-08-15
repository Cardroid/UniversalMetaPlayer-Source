using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Feature.OptionControl.ViewModel
{
  public class BasicOptionViewModel : ViewModelBase
  {
    public BasicOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (e) =>
      {
        if (e == "FileSavePath") OnPropertyChanged("FileSavePathToolTip");
      };

      MediaLoadEngineTypes = new List<GlobalProperty.Options.Enums.MediaLoadEngineType>
      {
        GlobalProperty.Options.Enums.MediaLoadEngineType.Native,
        //GlobalProperty.Options.Enums.MediaLoadEngineType.YoutubeDL
      };
    }

    public string FileSavePathToolTip => $"현재 설정 : \"{Path.GetFullPath(GlobalProperty.Options.FileSavePath)}\"\n\n프로그램에서 파일을 보관하는 폴더입니다. 빈 폴더를 권장합니다.";

    public bool PrivateLoggingIsChecked
    {
      get => GlobalProperty.Options.PrivateLogging;
      set
      {
        GlobalProperty.Options.PrivateLogging = value;
        OnPropertyChanged("PrivateLoggingIsChecked");
      }
    }

    public GlobalProperty.Options.Enums.MediaLoadEngineType SelectedItem
    {
      get => _SelectedItem;
      set
      {
        _SelectedItem = value;
        GlobalProperty.Options.MediaLoadEngine = _SelectedItem;
        OnPropertyChanged("SelectedItem");
      }
    }
    private GlobalProperty.Options.Enums.MediaLoadEngineType _SelectedItem = GlobalProperty.Options.MediaLoadEngine;

    public List<GlobalProperty.Options.Enums.MediaLoadEngineType> MediaLoadEngineTypes { get; }

    public string MediaLoadEngineToolTip => $"기본값 : {GlobalProperty.Options.DefaultValue.DefaultMediaLoadEngineType}\n\n미디어를 불러올 때 사용하는 엔진입니다.";
    public string PrivateLoggingToolTip => "로그에 더 자세한 사항을 기록합니다.\n개인 정보가 포함 될 수 있으나, 모든 정보는 익명으로 저장됩니다.\n(사용자를 특정할 수 없음)";
  }
}
