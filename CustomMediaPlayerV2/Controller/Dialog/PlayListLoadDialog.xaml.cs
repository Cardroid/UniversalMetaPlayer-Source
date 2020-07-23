using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

using CMP2.Core.Model;
using CMP2.Utility;

namespace CMP2.Controller.Dialog
{
  /// <summary>
  /// PlayListAddDialog.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListLoadDialog : UserControl
  {
    public PlayListLoadDialog()
    {
      InitializeComponent();

      this.AcceptButton.IsEnabled = false;
      this.UserTextBox.TextChanged += MediaLocationTextBox_TextChanged;
      this.MouseDown += (s, e) => { this.UserTextBox.Focus(); };
    }

    private bool IsWorkDelay = false;

    private async void MediaLocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        this.MessageLabel.Content = "플레이 리스트의 위치";
        return;
      }

      if (IsWorkDelay)
        return;
      IsWorkDelay = true;
      this.ProgressRing.Visibility = Visibility.Visible;

      await Task.Delay(500);

      var result = Path.GetExtension(this.UserTextBox.Text);
      switch (result.ToLower())
      {
        case ".m3u8":
          this.AcceptButton.IsEnabled = true;
          this.MessageLabel.Content = $"[m3u8] 타입이 확인 되었습니다.";
          break;
        default:
          this.AcceptButton.IsEnabled = false;
          this.MessageLabel.Content = "지원하지 않는 파일 입니다.";
          break;
      }
      
      this.ProgressRing.Visibility = Visibility.Collapsed;
      IsWorkDelay = false;
    }

    public Dictionary<string, string> GetResult()
    {
      var result = new Dictionary<string, string>();

      result.Add("PlayListFilePath", this.UserTextBox.Text);
      return result;
    }
  }
}
