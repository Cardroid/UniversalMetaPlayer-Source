using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UMP.Core.Global;
using UMP.Core.Model.Func;
using UMP.Core.Player;
using UMP.Core.Player.Plugin;
using UMP.Core.Player.Plugin.Effect;

namespace UMP.Controller.Function.AnalysisControl
{
  public partial class WaveAnalysisControl : FunctionControlForm
  {
    public WaveAnalysisControl() : base("분석 - 그래프")
    {
      InitializeComponent();

      ChangeAnalyserActivation();
      MainMediaPlayer.PropertyChanged += (_, e) => 
      {
        if (e.PropertyName == "MainPlayerInitialized")
          ChangeAnalyserActivation();
      };
    }

    public void ChangeAnalyserActivation()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        var sampleAnalyzer = MainMediaPlayer.Call<SampleAnalyzer>(PluginName.SampleAnalyzer);
        if (sampleAnalyzer != null)
        {
          this.Loaded += (_, e) => sampleAnalyzer.IsEnabled = true;
          this.Unloaded += (_, e) => sampleAnalyzer.IsEnabled = false;
        }
      }
    }
  }
}
