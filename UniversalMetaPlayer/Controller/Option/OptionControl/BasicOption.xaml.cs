using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using UMP.Controller.Option.OptionControl.ViewModel;
using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.Option.OptionControl
{
  public partial class BasicOption : UserControl
  {
    public BasicOption()
    {
      InitializeComponent();

      this.OpenDirectoryDialogButton.Click += OpenDirectoryDialogButton_Click;
    }

    private void OpenDirectoryDialogButton_Click(object sender, RoutedEventArgs e)
    {
      var result = DialogHelper.OpenDirectoryDialog("저장 폴더 선택", GlobalProperty.FileSavePath);
      if (result)
        GlobalProperty.FileSavePath = result.Result;
    }
  }
}
