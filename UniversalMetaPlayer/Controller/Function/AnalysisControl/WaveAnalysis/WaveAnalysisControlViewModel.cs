using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using UMP.Controller.Function.AnalysisControl.WaveAnalysis;
using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Helper;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player;
using UMP.Core.Player.Plugin;

namespace UMP.Controller.Function.AnalysisControl
{
  public class WaveAnalysisControlViewModel : ViewModelBase
  {
    public WaveAnalysisControlViewModel()
    {
      PolylineWaveFormLeftVisualizationPlugin = new PolylineWaveFormLeftVisualization();
      PolylineWaveFormRightVisualizationPlugin = new PolylineWaveFormRightVisualization();
      SpectrumAnalyzerVisualizationPlugin = new SpectrumAnalyzerVisualization();

      MainMediaPlayer.MaximumCalculated += audioGraph_MaximumCalculated;
      MainMediaPlayer.FftCalculated += audioGraph_FftCalculated;
    }

    public IVisualizationPlugin PolylineWaveFormLeftVisualizationPlugin { get; }
    public IVisualizationPlugin PolylineWaveFormRightVisualizationPlugin { get; }
    public IVisualizationPlugin SpectrumAnalyzerVisualizationPlugin { get; }

    void audioGraph_MaximumCalculated(object sender, MaxSampleEventArgs e)
    {
      this.PolylineWaveFormLeftVisualizationPlugin.OnMaxCalculated(e.MinSample, e.MaxSample, e.Channel);
      this.PolylineWaveFormRightVisualizationPlugin.OnMaxCalculated(e.MinSample, e.MaxSample, e.Channel);
      this.SpectrumAnalyzerVisualizationPlugin.OnMaxCalculated(e.MinSample, e.MaxSample, e.Channel);
    }

    void audioGraph_FftCalculated(object sender, FftEventArgs e)
    {
      this.PolylineWaveFormLeftVisualizationPlugin.OnFftCalculated(e.Result);
      this.PolylineWaveFormRightVisualizationPlugin.OnFftCalculated(e.Result);
      this.SpectrumAnalyzerVisualizationPlugin.OnFftCalculated(e.Result);
    }
  }
}
