using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using UMP.Controller.Function.AnalysisControl.WaveAnalysis;
using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Helper;
using UMP.Core.Model;
using UMP.Core.Player;

namespace UMP.Controller.Function.AnalysisControl
{
  public class WaveAnalysisControlViewModel : ViewModelBase
  {
    public WaveAnalysisControlViewModel()
    {
      PolylineWaveVisualizationPlugin = new PolylineWaveFormVisualization();
      SpectrumAnalyzerVisualizationPlugin = new SpectrumAnalyzerVisualization();

      MainMediaPlayer.MaximumCalculated += audioGraph_MaximumCalculated;
      MainMediaPlayer.FftCalculated += audioGraph_FftCalculated;
    }

    public IVisualizationPlugin PolylineWaveVisualizationPlugin { get; }
    public IVisualizationPlugin SpectrumAnalyzerVisualizationPlugin { get; }

    void audioGraph_FftCalculated(object sender, FftEventArgs e)
    {
      this.PolylineWaveVisualizationPlugin.OnFftCalculated(e.Result);
      this.SpectrumAnalyzerVisualizationPlugin.OnFftCalculated(e.Result);
    }

    void audioGraph_MaximumCalculated(object sender, MaxSampleEventArgs e)
    {
      this.PolylineWaveVisualizationPlugin.OnMaxCalculated(e.MinSample, e.MaxSample);
      this.SpectrumAnalyzerVisualizationPlugin.OnMaxCalculated(e.MinSample, e.MaxSample);
    }
  }
}
