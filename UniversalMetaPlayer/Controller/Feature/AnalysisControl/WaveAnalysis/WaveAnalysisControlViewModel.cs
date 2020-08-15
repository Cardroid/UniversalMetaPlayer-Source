﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

using UMP.Controller.Feature.AnalysisControl.WaveAnalysis;
using UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Helper;
using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.Feature.AnalysisControl
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

    private IVisualizationPlugin PolylineWaveVisualizationPlugin { get; }
    private IVisualizationPlugin SpectrumAnalyzerVisualizationPlugin { get; }

    public UserControl PolylineWaveControl => PolylineWaveVisualizationPlugin.Content;
    public UserControl SpectrumAnalyser => SpectrumAnalyzerVisualizationPlugin.Content;

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