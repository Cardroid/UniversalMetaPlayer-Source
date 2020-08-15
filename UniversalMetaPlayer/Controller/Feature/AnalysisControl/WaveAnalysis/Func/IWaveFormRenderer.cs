using System;
using System.Linq;

namespace UMP.Controller.Feature.AnalysisControl.WaveAnalysis.Func
{
    public interface IWaveFormRenderer
    {
        void AddValue(float maxValue, float minValue);
    }
}
