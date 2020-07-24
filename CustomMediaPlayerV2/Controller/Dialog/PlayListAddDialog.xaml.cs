using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using CMP2.Core;
using CMP2.Core.Model;
using CMP2.Utility;

using MaterialDesignThemes.Wpf;

namespace CMP2.Controller.Dialog
{
  /// <summary>
  /// PlayListAddDialog.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListAddDialog : UserControl
  {
    public CMP_VoidEventHandler Close;
    public PlayListAddDialog()
    {
      InitializeComponent();

      Log = new Log(typeof(PlayListAddDialog));

      this.AcceptButton.IsEnabled = false;
      this.UserTextBox.TextChanged += UserTextBox_TextChanged;
      this.OpenFileDialogButton.Click += OpenFileDialogButton_Click;
      this.MouseDown += (s, e) => { this.UserTextBox.Focus(); };
      this.UserTextBox.Focus();
    }
    private Log Log;

    private bool IsWorkDelay = false;
    private readonly string Invalid = $"{new string(Path.GetInvalidPathChars())}\"";

    private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        this.MessageLabel.Content = "미디어의 위치";
        return;
      }

      if (IsWorkDelay)
        return;
      IsWorkDelay = true;
      this.ProgressRing.Visibility = Visibility.Visible;

      // 올바르지 않은 문자 제거
      string text = this.UserTextBox.Text;
      for (int i = 0; i < Invalid.Length; i++)
        text = text.Replace(Invalid[i].ToString(), "");
      this.UserTextBox.Text = text;

      if (File.Exists(text))
      {
        var result = Checker.MediaTypeChecker(text);
        if (result != MediaType.NotSupport)
        {
          this.AcceptButton.IsEnabled = true;
          this.MessageLabel.Content = $"미디어 타입 : {result}";
        }
        else
        {
          this.AcceptButton.IsEnabled = false;
          this.MessageLabel.Content = "지원하지 않는 미디어 타입입니다.";
        }
      }
      else
        this.MessageLabel.Content = "존재하지 않는 파일 입니다.";

      this.ProgressRing.Visibility = Visibility.Collapsed;
      IsWorkDelay = false;
    }

    private async void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
    {
      string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
      if (!Directory.Exists(defaultPath))
        defaultPath = string.Empty;

      var filepath = DialogHelper.OpenFileDialog("로컬 미디어 파일열기", "Music File | *.mp3;*.flac", true, defaultPath);
      if (filepath != null)
      {
        if (filepath.Length == 1)
          this.UserTextBox.Text = filepath[0];
        else
        {
          Close?.Invoke();
          string errorFilePath = string.Empty;
          MediaType type;
          for (int i = 0; i < filepath.Length; i++)
          {
            type = Checker.MediaTypeChecker(filepath[i]);
            if (type != MediaType.NotSupport)
              await MainMediaPlayer.PlayList.Add(new Media(type, filepath[i]));
            else
              errorFilePath += $"{filepath[i]}\n";
          }
          if (!string.IsNullOrWhiteSpace(errorFilePath))
            Log.Warn($"미디어 추가 완료\n하나 이상의 오류가 있습니다.\nPath Count : [{filepath.Length}]\n-----Path-----\n{errorFilePath}--------------");
          else
            Log.Info($"미디어 추가 완료\nPath Count : [{filepath.Length}]");
        }
      }
    }

    public Dictionary<string, string> GetResult()
    {
      var result = new Dictionary<string, string>();

      result.Add("MediaLocation", this.UserTextBox.Text);
      return result;
    }
  }
}
