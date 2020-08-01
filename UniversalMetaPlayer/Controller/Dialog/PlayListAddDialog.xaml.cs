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
using UMP.Core;
using UMP.Core.Model;
using UMP.Utility;

using MaterialDesignThemes.Wpf;

namespace UMP.Controller.Dialog
{
  /// <summary>
  /// PlayListAddDialog.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListAddDialog : UserControl
  {
    public UMP_VoidEventHandler Close;
    public PlayListAddDialog()
    {
      InitializeComponent();

      this.AcceptButton.IsEnabled = false;
      this.UserTextBox.TextChanged += UserTextBox_TextChanged;
      this.AcceptButton.Click += AcceptButton_Click;
      this.CancelButton.Click += CancelButton_Click;
      this.OpenFileDialogButton.Click += OpenFileDialogButton_Click;
      this.Loaded += (s, e) => { this.UserTextBox.Focus(); };
      this.UserTextBox.Focus();
    }

    private bool IsWorkDelay = false;
    private readonly string Invalid = $"{new string(Path.GetInvalidPathChars())}\"";
    private string[] SelectFilePaths { get; set; }

    private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        this.MessageLabel.Content = "미디어의 위치를 입력하세요";
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

      if (!text.ToLower().StartsWith("http"))
      {
        if (File.Exists(text))
        {
          SelectFilePaths = new string[] { text };
          this.MessageLabel.Content = "파일이 확인되었습니다";
          this.AcceptButton.IsEnabled = true;
        }
        else
        {
          this.AcceptButton.IsEnabled = false;
          this.MessageLabel.Content = "파일을 확인 할 수 없습니다";
        }
      }
      else
      {
        this.UserTextBox.Text = text;
        SelectFilePaths = new string[] { text };
        this.MessageLabel.Content = "온라인 경로가 확인되었습니다";
        this.AcceptButton.IsEnabled = true;
      }

      this.ProgressRing.Visibility = Visibility.Collapsed;
      IsWorkDelay = false;
    }

    private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
    {
      string defaultPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
      if (!Directory.Exists(defaultPath))
        defaultPath = string.Empty;

      var filePaths = DialogHelper.OpenFileDialog("로컬 미디어 파일열기", "Music File | *.mp3;*.flac", true, defaultPath);
      if(filePaths != null)
      {
        this.ProgressRing.Visibility = Visibility.Visible;

        // 텍스트 입력을 비활성화
        this.UserTextBox.TextChanged -= UserTextBox_TextChanged;
        this.UserTextBox.Focusable = false;
        this.UserTextBox.IsHitTestVisible = false;
        this.UserTextBox.IsReadOnly = true;
        this.UserTextBox.IsReadOnlyCaretVisible = false;

        SelectFilePaths = filePaths;
        this.UserTextBox.Text = $"[{filePaths.Length}] 개의 파일이 확인되었습니다";
        this.ProgressRing.Visibility = Visibility.Collapsed;
      }
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
      this.ProgressRing.Visibility = Visibility.Visible;
      this.UserTextBox.IsEnabled = false;
      this.AcceptButton.IsEnabled = false;
      this.OpenFileDialogButton.IsEnabled = false;
      this.CancelButton.IsEnabled = false;
      if (SelectFilePaths != null)
      {
        for (int i = 0; i < SelectFilePaths.Length; i++)
        {
          this.MessageLabel.Content = $"{i + 1}번째 추가 중...";
          await MainMediaPlayer.PlayList.Add(SelectFilePaths[i]);
        }
      }
      this.ProgressRing.Visibility = Visibility.Collapsed;
      Close.Invoke();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
      Close.Invoke();
    }
  }
}
