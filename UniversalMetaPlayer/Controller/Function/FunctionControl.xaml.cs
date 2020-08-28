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

namespace UMP.Controller.Function
{
  public partial class FunctionControl : UserControl
  {
    private FunctionControlViewModel ViewModel { get; }

    public FunctionControl()
    {
      InitializeComponent();
      ViewModel = (FunctionControlViewModel)this.DataContext;

      this.PreviewMouseDown += (_, e) => { System.Windows.Input.Keyboard.ClearFocus(); };
    }

    private void MainOptionControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is TreeViewItem item)
        ViewModel.FunctionControlName = item.Name;
    }

    private void TreeViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sender is TreeViewItem item)
        item.IsExpanded = !item.IsExpanded;
    }
  }
}
