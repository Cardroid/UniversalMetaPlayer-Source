using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
using UMP.Core.Player;
using UMP.Utility;

using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

using Timer = System.Timers.Timer;

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

      this.Loaded += (_, e) => 
      {
        this.KeyDown += (_, e) =>
        {
          if (e.Key == Key.Escape)
            Close?.Invoke();
        };

        this.AcceptButton.IsEnabled = false;
        this.UserTextBox.TextChanged += UserTextBox_TextChanged;
        this.AcceptButton.Click += AcceptButton_Click;
        this.CancelButton.Click += CancelButton_Click;
        this.OpenFileDialogButton.Click += OpenFileDialogButton_Click;
        this.Loaded += (s, e) => { this.UserTextBox.Focus(); };
        this.UserTextBox.Focus();

        if (Checker.CheckForInternetConnection())
          this.MessageLabel.Content = "미디어의 위치를 입력하세요";
        else
          this.MessageLabel.Content = "[오프라인]\n(비디오 주소만 추가할 수 있습니다)\n미디어의 위치를 입력하세요";

        Timer.Elapsed += Timer_Elapsed;
      };
    }

    delegate void TimerEventFiredDelegate();
    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      Timer.Stop();
      Dispatcher.Invoke(new Action(async () => {
        await UserTextBox_Changed();
      }));
    }

    private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!TextBoxBlock)
      {
        Timer.Stop();
        Timer.Start();
      }
    }

    private readonly Timer Timer = new Timer(1000) { AutoReset = true };
    private bool TextBoxBlock = false;
    private bool IsWorkDelay = false;
    private readonly string Invalid = $"{new string(Path.GetInvalidPathChars())}\"";
    private string[] SelectFilePaths { get; set; }

    private async Task UserTextBox_Changed()
    {
      if (IsWorkDelay)
        return;
      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        if (Checker.CheckForInternetConnection())
          this.MessageLabel.Content = "미디어의 위치를 입력하세요";
        else
          this.MessageLabel.Content = "[오프라인]\n(비디오 주소만 추가할 수 있습니다)\n미디어의 위치를 입력하세요";
        this.AcceptButton.IsEnabled = false;
        return;
      }

      IsWorkDelay = true;
      this.ProgressRing.Visibility = Visibility.Visible;

      // 올바르지 않은 문자 제거
      string text = this.UserTextBox.Text;
      for (int i = 0; i < Invalid.Length; i++)
        text = text.Replace(Invalid[i].ToString(), "");
      this.UserTextBox.Text = text;

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
          this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
        }
      }
      else
      {
        if (Checker.CheckForInternetConnection() && GlobalProperty.Options.MediaLoadEngine == GlobalProperty.Options.Enums.MediaLoadEngineType.Native)
        {
          YoutubeClient client = new YoutubeClient();
          object info = null;

          if (info == null)
            try
            {
              info = await client.Videos.GetAsync(text);
            }
            catch
            {
              // 비디오 정보 받아오기 실패
              info = null;
            }

          if (info == null)
            try
            {
              info = await client.Playlists.GetAsync(text);
            }
            catch
            {
              // 플레이리스트 정보 받아오기 실패
              info = null;
            }

          if (info == null)
            try
            {
              info = await client.Channels.GetAsync(text);
            }
            catch
            {
              try
              {
                info = await client.Channels.GetByUserAsync(text);
              }
              catch
              {
                // 채널 정보 받아오기 실패
                info = null;
              }
            }

          if (info is Video)
          {
            SelectFilePaths = new string[] { text };
            this.MessageLabel.Content = "[Video]\n온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
          else if (info is Playlist playlist)
          {
            var videos = await client.Playlists.GetVideosAsync(playlist.Url);
            string vid = string.Empty;
            for (int i = 0; i < videos.Count; i++)
            {
              vid += $"{videos[i].Url},";
            }
            vid = vid[0..^1];
            SelectFilePaths = vid.Split(',');
            this.MessageLabel.Content = $"[PlayList]\n[{playlist.Title}]\n({SelectFilePaths.Length} 개)\n온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
          else if (info is Channel channel)
          {
            var videos = await client.Channels.GetUploadsAsync(channel.Url);
            string vid = string.Empty;
            for (int i = 0; i < videos.Count; i++)
            {
              vid += $"{videos[i].Url},";
            }
            vid = vid[0..^1];
            SelectFilePaths = vid.Split(',');
            this.MessageLabel.Content = $"[Channel]\n[{channel.Title}]\n({SelectFilePaths.Length} 개)\n온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
          else
          {
            this.AcceptButton.IsEnabled = false;
            this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
          }
        }
        else
        {
          if(GlobalProperty.Options.MediaLoadEngine == GlobalProperty.Options.Enums.MediaLoadEngineType.Native)
          {
            try
            {
              VideoId videoId = new VideoId(text);
              SelectFilePaths = new string[] { text };
              this.MessageLabel.Content = $"[확인되지 않음]\n[ID : {videoId.Value}]\n온라인 경로입니다";
              this.AcceptButton.IsEnabled = true;
            }
            catch
            {
              this.AcceptButton.IsEnabled = false;
              this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
            }
          }
          else
          {
            SelectFilePaths = new string[] { text };
            this.MessageLabel.Content = "[확인되지 않음]\n온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
        }
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
      if (filePaths)
      {
        this.ProgressRing.Visibility = Visibility.Visible;

        // 텍스트 입력을 비활성화
        TextBoxBlock = true;
        this.UserTextBox.Focusable = false;
        this.UserTextBox.IsHitTestVisible = false;
        this.UserTextBox.IsReadOnly = true;
        this.UserTextBox.IsReadOnlyCaretVisible = false;

        SelectFilePaths = filePaths.Result;
        this.UserTextBox.Text = $"[{filePaths.Result.Length}] 개의 파일이 확인되었습니다";
        this.ProgressRing.Visibility = Visibility.Collapsed;
        this.AcceptButton.IsEnabled = true;
      }
    }

    private async void AcceptButton_Click(object sender, RoutedEventArgs e)
    {
      this.ProgressRing.Visibility = Visibility.Visible;
      TextBoxBlock = true;
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
      GlobalEvent.GlobalMessageEventInvoke($"미디어 [{SelectFilePaths.Length}]개 추가 완료", true);
      Close.Invoke();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close.Invoke();
  }
}
