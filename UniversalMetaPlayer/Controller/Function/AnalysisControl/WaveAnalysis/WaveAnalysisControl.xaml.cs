using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using UMP.Core.Global;
using UMP.Core.Model.Func;

namespace UMP.Controller.Function.AnalysisControl
{
  public partial class WaveAnalysisControl : FunctionControlForm
  {
    public WaveAnalysisControl() : base("분석 - 그래프")
    {
      InitializeComponent();
    }
  }
}
