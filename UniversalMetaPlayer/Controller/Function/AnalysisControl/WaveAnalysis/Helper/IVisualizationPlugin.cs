using System;
using System.Linq;
using System.Windows.Controls;

using NAudio.Dsp;

namespace UMP.Controller.Function.AnalysisControl.WaveAnalysis.Helper
{
  public interface IVisualizationPlugin
  {
    public string Name { get; }
    public UserControl Content { get; }

    // n.b. not great design, need to refactor so visualizations can attach to the playback graph and measure just what they need
    public void OnMaxCalculated(float min, float max);
    public void OnFftCalculated(Complex[] result);
  }
}
