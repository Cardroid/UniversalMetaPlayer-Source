using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UMP.Core.Global;
using UMP.Core.Model.Func;
using UMP.Core.Player;

namespace UMP.Controller.Function.AnalysisControl
{
  public partial class WaveAnalysisControl : FunctionControlForm
  {
    public WaveAnalysisControl() : base("분석 - 그래프")
    {
      InitializeComponent();
      this.Loaded += (_, e) => MainMediaPlayer.IsAnalyzerEnabled = true;
      this.Unloaded += (_, e) => MainMediaPlayer.IsAnalyzerEnabled = false;
    }
  }
}
