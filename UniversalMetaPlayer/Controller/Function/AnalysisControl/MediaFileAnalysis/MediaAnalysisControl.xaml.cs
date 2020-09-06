using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core;
using UMP.Core.Model.Func;
using UMP.Core.Player;
using UMP.Utils.MediaInfoLib;

namespace UMP.Controller.Function.AnalysisControl.MediaFileAnalysis
{
  public partial class MediaAnalysisControl : FunctionControlForm
  {
    public MediaAnalysisControl() : base("분석 - 속성")
    {
      InitializeComponent();
    }
  }
}
