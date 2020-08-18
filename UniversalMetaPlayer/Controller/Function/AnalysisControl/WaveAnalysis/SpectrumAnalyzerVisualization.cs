using System.Windows.Controls;

using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Func;
using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Helper;

namespace UMP.Controller.Function.AnalysisControl.WaveAnalysis
{
  class SpectrumAnalyzerVisualization : IVisualizationPlugin
  {
    private readonly SpectrumAnalyser spectrumAnalyser = new SpectrumAnalyser();

    public string Name => "스펙트럼 분석기";

    public UserControl Content => spectrumAnalyser;

    public void OnMaxCalculated(float min, float max)
    {
      // nothing to do
    }

    public void OnFftCalculated(NAudio.Dsp.Complex[] result)
    {
      spectrumAnalyser.Update(result);
    }
  }
}
