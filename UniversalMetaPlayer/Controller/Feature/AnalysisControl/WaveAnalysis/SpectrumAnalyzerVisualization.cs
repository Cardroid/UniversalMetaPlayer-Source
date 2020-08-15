using System.Windows.Controls;
using UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Func;
using UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Helper;

namespace UMP.Controller.Feature.AnalysisControl.WaveAnalysis
{
    class SpectrumAnalyzerVisualization : IVisualizationPlugin
    {
        private readonly SpectrumAnalyser spectrumAnalyser = new SpectrumAnalyser();

        public string Name => "Spectrum Analyser";

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
