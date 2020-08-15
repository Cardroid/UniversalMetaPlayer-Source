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
using UMP.Controller.Feature.AnalysisControl;
using UMP.Controller.Feature.OptionControl;
using UMP.Core;

namespace UMP.Controller.Feature
{
  public partial class MainFeatureControl : UserControl
  {
    private MainFeatureControlViewModel ViewModel { get; }

    public MainFeatureControl()
    {
      InitializeComponent();
      ViewModel = (MainFeatureControlViewModel)this.DataContext;

      GlobalProperty.PropertyChanged += (e) =>
      {
        if (e == "SetDefault")
          ViewModel.FeaturePanel = new BasicOption();
      };
    }

    private void MainOptionControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is TreeViewItem item)
        ViewModel.FeaturePanel = item.Name switch
        {
          "Basic" => new BasicOption(),
          "Keyboard" => new KeyboardOption(),
          "Theme" => new ThemeOption(),
          "Information" => new InformationOption(),
          "Graph" => new WaveAnalysisControl(),
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
