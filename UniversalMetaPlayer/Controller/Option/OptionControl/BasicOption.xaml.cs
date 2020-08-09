using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using MaterialDesignThemes.Wpf;

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
      this.SetDefault.Click += SetDefault_Click;
      SetDefaultForeground = this.SetDefault.Foreground;
    }

    private void OpenDirectoryDialogButton_Click(object sender, RoutedEventArgs e)
    {
      var result = DialogHelper.OpenDirectoryDialog("저장 폴더 선택", GlobalProperty.Options.FileSavePath);
      if (result)
        GlobalProperty.Options.FileSavePath = result.Result;
    }

    #region SetDefaultButton

    private readonly Brush SetDefaultForeground;

    private bool IsReset
    {
      get => _IsReset;
      set
      {
        _IsReset = value;
        if (value)
        {
          this.SetDefault.Foreground = Brushes.Red;
          IsResetLock();
        }
        else
          this.SetDefault.Foreground = SetDefaultForeground;
      }
    }
    private bool _IsReset = false;

    private async void IsResetLock()
    {
      await Task.Delay(3000);
      IsReset = false;
    }

    private void SetDefault_Click(object sender, RoutedEventArgs e)
    {
      if (!IsReset)
      {
        IsReset = true;
      }
      else
      {
        GlobalProperty.SetDefault();
        GlobalEvent.GlobalMessageEventInvoke("설정이 초기화 되었습니다.", true);
        IsReset = false;
      }
    }
    #endregion
  }
}
