using System.Windows.Controls;

using UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Func;
using UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Helper;

namespace UMP.Controller.Feature.AnalysisControl.WaveAnalysis
{
  public class PolylineWaveFormVisualization : IVisualizationPlugin
  {
    private readonly PolylineWaveControl polylineWaveFormControl = new PolylineWaveControl();

    public string Name => "Polyline WaveVisualization";

    public UserControl Content => polylineWaveFormControl;

    public void OnMaxCalculated(float min, float max)
    {
      polylineWaveFormControl.AddValue(max, min);
    }

    public void OnFftCalculated(NAudio.Dsp.Complex[] result)
    {
      // nothing to do
    }
  }
}
