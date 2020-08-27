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

using UMP.Controller.Function.AnalysisControl;
using UMP.Controller.Function.OptionControl;
using UMP.Controller.Function.Etc;
using UMP.Controller.Function.AnalysisControl.AudioFileAnalysis;
using UMP.Core.Global;
using UMP.Utility;

namespace UMP.Controller.Function
{
  public partial class FunctionControl : UserControl
  {
    private FunctionControlViewModel ViewModel { get; }

    public FunctionControl()
    {
      InitializeComponent();
      ViewModel = (FunctionControlViewModel)this.DataContext;

      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "SetDefault")
          ViewModel.FunctionPanel = new BasicOption();
      };
    }

    private void MainOptionControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is TreeViewItem item)
        ViewModel.FunctionPanel = item.Name switch
        {
          // 일반
          "Basic" => new BasicOption(),
          "Keyboard" => new KeyboardOption(),
          "Theme" => new ThemeOption(),
          "AudioEffect" => new EffectOption(),

          // 분석
          "Graph" => new WaveAnalysisControl(),
          "AudioProperty" => new AudioWaveFormatAnalysisControl(),

          // 정보
          "Information" => new InformationOption(),

          // 기능 준비 중
          _ => new ErrorPageControl(),
        };
    }

    private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is TreeViewItem item)
        item.IsExpanded = !item.IsExpanded;
    }
  }
}
