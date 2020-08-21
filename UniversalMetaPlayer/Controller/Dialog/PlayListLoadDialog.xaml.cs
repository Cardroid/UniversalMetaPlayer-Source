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
using Microsoft.Win32;
using UMP.Utility;
using UMP.Core;
using UMP.Core.Player;
using UMP.Core.Global;

namespace UMP.Controller.Dialog
{
  /// <summary>
  /// PlayListAddDialog.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class PlayListLoadDialog : UserControl
  {
    public UMP_VoidEventHandler Close;
    public PlayListLoadDialog()
    {
      InitializeComponent();

      this.KeyDown += (_, e) =>
      {
        if (e.Key == Key.Escape)
          Close?.Invoke();
      };

      this.AcceptButton.IsEnabled = false;
      this.AcceptButton.Click += AcceptButton_Click;
      this.CancelButton.Click += CancelButton_Click;
      this.UserTextBox.TextChanged += MediaLocationTextBox_TextChanged;
      this.OpenFileDialogButton.Click += OpenFileDialogButton_Click;
      this.MouseDown += (s, e) => { this.UserTextBox.Focus(); };
      this.UserTextBox.Focus();

      this.SaveCurrentPlayList.IsChecked = TempProperty.SaveCurrentPlayList;
      this.LoadContinue.IsChecked = TempProperty.LoadContinue;

      this.SaveCurrentPlayList.Click += CheckBox_Click_Save;
      this.LoadContinue.Click += CheckBox_Click_Save;
    }

    private void CheckBox_Click_Save(object sender, RoutedEventArgs e)
    {
      if (sender is CheckBox checkBox)
      {
        switch (checkBox.Name)
        {
          case "SaveCurrentPlayList":
            TempProperty.SaveCurrentPlayList = checkBox.IsChecked.GetValueOrDefault();
            break;
          case "LoadContinue":
            TempProperty.LoadContinue = checkBox.IsChecked.GetValueOrDefault();
            break;
        }
      }
    }

    private string PlayListFilePath { get; set; }
    private bool IsWorkDelay = false;
    private readonly string Invalid = $"{new string(Path.GetInvalidPathChars())}\"";

    private void MediaLocationTextBox_TextChanged(object sender, TextChangedEventArgs e)
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

      // 올바르지 않은 문자 제거
      string text = this.UserTextBox.Text;
      for (int i = 0; i < Invalid.Length; i++)
        text = text.Replace(Invalid[i].ToString(), "");
      this.UserTextBox.Text = text;

      if (File.Exists(text))
      {
        var result = Path.GetExtension(text);
        switch (result.ToLower())
        {
          case ".m3u8":
            this.AcceptButton.IsEnabled = true;
            this.MessageLabel.Content = $"[{result.ToLower()}] 타입이 확인 되었습니다";
            PlayListFilePath = text;
            this.AcceptButton.IsEnabled = true;
            break;
          default:
            this.AcceptButton.IsEnabled = false;
            this.MessageLabel.Content = "지원하지 않는 파일 입니다";
            PlayListFilePath = string.Empty;
            this.AcceptButton.IsEnabled = false;
            break;
        }
      }
      else
      {
        this.MessageLabel.Content = "존재하지 않는 파일 입니다";
        PlayListFilePath = string.Empty;
        this.AcceptButton.IsEnabled = false;
      }

      this.ProgressRing.Visibility = Visibility.Collapsed;
      IsWorkDelay = false;
    }

    private void OpenFileDialogButton_Click(object sender, RoutedEventArgs e)
    {
      string defaultPath = Path.Combine(Environment.CurrentDirectory, GlobalProperty.Options.Getter<string>(Enums.ValueName.FileSavePath), "PlayList");
      if (!Directory.Exists(defaultPath))
        defaultPath = string.Empty;

      var filepath = DialogHelper.OpenFileDialog("플레이 리스트 파일열기", "PlayList File | *.m3u8", false, defaultPath);
      if (filepath)
      {
        this.UserTextBox.Text = filepath.Result[0];
        this.AcceptButton.IsEnabled = true;
      }
      else
        this.AcceptButton.IsEnabled = false;
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
      this.ProgressRing.Visibility = Visibility.Visible;
      this.UserTextBox.IsEnabled = false;
      this.AcceptButton.IsEnabled = false;
      this.OpenFileDialogButton.IsEnabled = false;
      this.CancelButton.IsEnabled = false;

      if (this.SaveCurrentPlayList.IsChecked.GetValueOrDefault())
        await MainMediaPlayer.PlayList.Save();

      if (!this.LoadContinue.IsChecked.GetValueOrDefault())
        MainMediaPlayer.PlayList.Clear();

      if (!string.IsNullOrWhiteSpace(PlayListFilePath))
        await MainMediaPlayer.PlayList.Load(PlayListFilePath, !this.LoadContinue.IsChecked.GetValueOrDefault());

      this.ProgressRing.Visibility = Visibility.Collapsed;
      Close.Invoke();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close.Invoke();
  }
}
