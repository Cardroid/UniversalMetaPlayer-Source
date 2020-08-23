using System.Windows.Controls;

using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Func;
using UMP.Controller.Function.AnalysisControl.WaveAnalysis.Helper;

namespace UMP.Controller.Function.AnalysisControl.WaveAnalysis
{
  public class PolylineWaveFormLeftVisualization : IVisualizationPlugin
  {
    private readonly PolylineWaveControl polylineWaveFormControl = new PolylineWaveControl();

    public string Name => "폴리라인 파동 시각화 (Left)";

    public UserControl Content => polylineWaveFormControl;

    public void OnMaxCalculated(float min, float max, int channel)
    {
      if (channel == 0)
        polylineWaveFormControl.AddValue(max, min);
    }

    public void OnFftCalculated(NAudio.Dsp.Complex[] result)
    {
      // nothing to do
    }
  }
  public class PolylineWaveFormRightVisualization : IVisualizationPlugin
  {
    private readonly PolylineWaveControl polylineWaveFormControl = new PolylineWaveControl();

    public string Name => "폴리라인 파동 시각화 (Right)";

    public UserControl Content => polylineWaveFormControl;

    public void OnMaxCalculated(float min, float max, int channel)
    {
      if (channel == 1)
        polylineWaveFormControl.AddValue(max, min);
    }

    public void OnFftCalculated(NAudio.Dsp.Complex[] result)
    {
      // nothing to do
    }
  }
}
