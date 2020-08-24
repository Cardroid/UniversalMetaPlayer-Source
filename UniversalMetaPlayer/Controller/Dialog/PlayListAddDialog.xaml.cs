using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using UMP.Core.Global;
using UMP.Core.Player;
using UMP.Utility;

using YoutubeExplode;
using YoutubeExplode.Channels;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;

using Timer = System.Timers.Timer;

namespace UMP.Controller.Dialog
{
  public partial class PlayListAddDialog : UserControl
  {
    public UMP_VoidEventHandler CloseEvent;
    public void Close()
    {
      IsCanceled = true;
      CloseEvent?.Invoke();
    }
    public PlayListAddDialog()
    {
      InitializeComponent();

      this.UserTextBox.MinWidth = SystemParameters.WorkArea.Width / 5;
      this.UserTextBox.MaxWidth = SystemParameters.WorkArea.Width / 1.2;

      this.Loaded += (_, e) =>
      {
        this.KeyDown += (_, e) =>
        {
          if (e.Key == Key.Escape)
            Close();
        };

        this.UserTextBox.GotKeyboardFocus += (_, e) => { GlobalKeyDownEvent.IsEnabled = false; };
        this.UserTextBox.LostKeyboardFocus += (_, e) => { GlobalKeyDownEvent.IsEnabled = true; };
        this.MouseDown += (_, e) => { Keyboard.ClearFocus(); };

        this.AcceptButton.IsEnabled = false;
        this.UserTextBox.TextChanged += UserTextBox_TextChanged;
        this.AcceptButton.Click += AcceptButton_Click;
        this.CancelButton.Click += CancelButton_Click;
        this.OpenFileDialogButton.Click += OpenFileDialogButton_Click;
        this.Loaded += (s, e) => { this.UserTextBox.Focus(); };
        this.UserTextBox.Focus();

        UserTextBox_Empty();

        Timer.Elapsed += Timer_Elapsed;
      };
    }

    delegate void TimerEventFiredDelegate();
    private void Timer_Elapsed(object sender, ElapsedEventArgs e)
    {
      Timer.Stop();
      Dispatcher.Invoke(new Action(async () =>
      {
        await UserTextBox_Changed();
      }));
    }

    private void UserTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      Timer.Stop();
      if (!TextBoxBlock)
        Timer.Start();
    }

    private bool IsCanceled = false;
    private readonly Timer Timer = new Timer(1500) { AutoReset = true };
    private bool TextBoxBlock = false;
    private bool IsWorkDelay = false;
    private readonly string Invalid = $"{new string(Path.GetInvalidPathChars())}\"";
    private string[] SelectFilePaths { get; set; }

    private void UserTextBox_Empty()
    {
      if (Checker.CheckForInternetConnection())
      {
        this.SiteLabel.Content = "[Online]\n(채널, 플레이리스트, 비디오 주소로 추가할 수 있습니다)\n(파일을 직접 선택하려면 찾아보기를 사용하세요)";
        this.MessageLabel.Content = "미디어의 위치를 입력하세요";
      }
      else
      {
        this.SiteLabel.Content = "[Offline]\n(오프라인 모드에서는 비디오만 추가할 수 있습니다)\n(파일을 직접 선택하려면 찾아보기를 사용하세요)";
        this.MessageLabel.Content = "미디어의 위치를 입력하세요";
      }
      this.UserTextBox.Text = string.Empty;
      this.AcceptButton.IsEnabled = false;
    }
    
    private async Task UserTextBox_Changed()
    {
      if (IsWorkDelay)
        return;
      IsWorkDelay = true;
      this.ProgressRing.Visibility = Visibility.Visible;

      if (string.IsNullOrWhiteSpace(this.UserTextBox.Text))
      {
        UserTextBox_Empty();
        this.ProgressRing.Visibility = Visibility.Collapsed;
        IsWorkDelay = false;
        return;
      }
      else
        this.SiteLabel.Visibility = Visibility.Visible;

      // 올바르지 않은 문자 제거
      string text = this.UserTextBox.Text;
      for (int i = 0; i < Invalid.Length; i++)
        text = text.Replace(Invalid[i].ToString(), "");
      this.UserTextBox.Text = text;

      var match = Parser.GetUrlInfo(text);
      if (match.Success)
      {
        if (Checker.CheckForInternetConnection() && GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine) == Enums.MediaLoadEngineType.Native)
        {
          YoutubeClient client = new YoutubeClient();
          object info = null;

          if (info == null)
            // 비디오 정보 요청
            try { info = await client.Videos.GetAsync(text); }
            catch { info = null; }

          if (info == null)
            // 플레이리스트 정보 요청
            try { info = await client.Playlists.GetAsync(text); }
            catch { info = null; }

          if (info == null)
            // 채널 정보 요청 (ID)
            try { info = await client.Channels.GetAsync(text); }
            catch { info = null; }

          if (info is Video)
          {
            SelectFilePaths = new string[] { text };
            this.SiteLabel.Content = $"[{match.Domain}]\n[Video]";
            this.MessageLabel.Content = "온라인 경로입니다";
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
            this.SiteLabel.Content = $"[{match.Domain}]\n[PlayList]\n[{playlist.Title}] (감지된 파일 : {SelectFilePaths.Length} 개)";
            this.MessageLabel.Content = $"온라인 경로입니다";
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
            this.SiteLabel.Content = $"[{match.Domain}]\n[Channel]\n[{channel.Title}] (감지된 파일 : {SelectFilePaths.Length}개)";
            this.MessageLabel.Content = $"온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
          else
          {
            this.AcceptButton.IsEnabled = false;
            this.SiteLabel.Content = $"[{match.Domain}]\n[Error]";
            this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
          }
        }
        else
        {
          if (GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine) == Enums.MediaLoadEngineType.Native)
          {
            try
            {
              VideoId videoId = new VideoId(text);
              SelectFilePaths = new string[] { text };
              this.SiteLabel.Content = $"[Unconfirmed]\n[ID : {videoId.Value}]";
              this.MessageLabel.Content = $"온라인 경로입니다";
              this.AcceptButton.IsEnabled = true;
            }
            catch
            {
              this.AcceptButton.IsEnabled = false;
              this.SiteLabel.Content = $"[Error]";
              this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
            }
          }
          else
          {
            SelectFilePaths = new string[] { text };
            this.SiteLabel.Content = $"[Unconfirmed]";
            this.MessageLabel.Content = "온라인 경로입니다";
            this.AcceptButton.IsEnabled = true;
          }
        }
      }
      else
      {
        if (Checker.IsLocalPath(text) && File.Exists(text))
        {
          SelectFilePaths = new string[] { text };
          this.SiteLabel.Content = $"[Local]";
          this.MessageLabel.Content = "파일이 확인되었습니다";
          this.AcceptButton.IsEnabled = true;
        }
        else
        {
          this.SiteLabel.Content = $"[Error]";
          this.AcceptButton.IsEnabled = false;
          this.MessageLabel.Content = "미디어를 확인 할 수 없습니다";
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

      var filePaths = DialogHelper.OpenFileDialog("로컬 미디어 파일열기", "Music File|*.mp3;*.flac|All File|*.*", true, defaultPath);
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
      if (SelectFilePaths != null)
      {
        for (int i = 0; i < SelectFilePaths.Length; i++)
        {
          if (IsCanceled)
            break;
          this.MessageLabel.Content = $"[{i + 1}/{SelectFilePaths.Length}]번째 추가 중...";
          await MainMediaPlayer.PlayList.Add(SelectFilePaths[i]);
        }
      }
      this.ProgressRing.Visibility = Visibility.Collapsed;
      GlobalMessageEvent.Invoke($"미디어 [{SelectFilePaths.Length}]개 추가 완료", true);
      Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();
  }
}
