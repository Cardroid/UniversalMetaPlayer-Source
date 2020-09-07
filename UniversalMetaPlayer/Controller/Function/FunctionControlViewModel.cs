using System;

using UMP.Controller.Function.AnalysisControl;
using UMP.Controller.Function.AnalysisControl.MediaFileAnalysis;
using UMP.Controller.Function.Info;
using UMP.Controller.Function.OptionControl;

using UMP.Core.Global;
using UMP.Core.Model.Control;
using UMP.Core.Model.ViewModel;

namespace UMP.Controller.Function
{
  public class FunctionControlViewModel : ViewModelBase
  {
    public FunctionControlViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "SetDefault")
          FunctionControlRefresh();
      };
    }

    public string Header => FunctionPanel != null ? FunctionPanel.FunctionName : "기능 패널";
    
    public string FunctionControlName
    {
      get => _FunctionControlName;
      set
      {
        _FunctionControlName = value;
        FunctionControlRefresh();
      }
    }
    private string _FunctionControlName = "Basic";

    private void FunctionControlRefresh()
    {
      FunctionPanel = FunctionControlName switch
      {
        // 일반
        "Basic" => new BasicOption(),
        "Player" => new PlayerOption(),
        "Keyboard" => new KeyboardOption(),
        "Theme" => new ThemeOption(),
        "AudioEffect" => new EffectOption(),

        // 분석
        "Graph" => new WaveAnalysisControl(),
        "AudioProperty" => new MediaAnalysisControl(),

        // 정보
        "Information" => new InformationOption(),

        // 기능 준비 중
        _ => new ErrorPageControl(),
      };
    }

    public FunctionControlForm FunctionPanel
    {
      get => _FunctionPanel.IsAlive ? (FunctionControlForm)_FunctionPanel.Target : null;
      set
      {
        if (value == null)
          _FunctionPanel = new WeakReference(new ErrorPageControl());
        else
          _FunctionPanel = new WeakReference(value);

        OnPropertyChanged("FunctionPanel");
        OnPropertyChanged("Header");
      }
    }
    private WeakReference _FunctionPanel = new WeakReference(new BasicOption());
  }
}
