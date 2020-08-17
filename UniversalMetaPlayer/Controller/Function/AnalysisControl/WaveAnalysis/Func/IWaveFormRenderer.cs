using System;
using System.Linq;

namespace UMP.Controller.Function.AnalysisControl.WaveAnalysis.Func
{
    public interface IWaveFormRenderer
    {
        void AddValue(float maxValue, float minValue);
    }
}
