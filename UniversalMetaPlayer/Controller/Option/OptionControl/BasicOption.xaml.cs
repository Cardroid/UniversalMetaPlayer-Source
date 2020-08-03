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

using UMP.Core;
using UMP.Utility;

namespace UMP.Controller.Option.OptionControl
{
  public partial class BasicOption : UserControl
  {
    public BasicOption()
    {
      InitializeComponent();

      OptionSync();
      GlobalProperty.PropertyChanged += (s) =>
      {
        OptionSync();
      };

      this.OpenDirectoryDialogButton.Click += OpenDirectoryDialogButton_Click;
      this.PrivateLogging.Click += ToggleButton_Click;
    }

    private void OpenDirectoryDialogButton_Click(object sender, RoutedEventArgs e)
    {
      var result = DialogHelper.OpenDirectoryDialog("저장 폴더 선택", GlobalProperty.FileSavePath);
      if (result)
        GlobalProperty.FileSavePath = result.Result;
    }

    private void ToggleButton_Click(object sender, RoutedEventArgs e)
    {
      if (sender is ToggleButton button)
      {
        switch (button.Name)
        {
          case "PrivateLogging":
            GlobalProperty.PrivateLogging = !GlobalProperty.PrivateLogging;
            break;
        }
      }
    }

    /// <summary>
    /// 옵션 - UI 동기화
    /// </summary>
    private void OptionSync()
    {
      this.PrivateLogging.IsChecked = GlobalProperty.PrivateLogging;
      this.FileSavePathPanel.ToolTip = $"현재 설정 : \"{Path.GetFullPath(GlobalProperty.FileSavePath)}\"";
    }
  }
}
