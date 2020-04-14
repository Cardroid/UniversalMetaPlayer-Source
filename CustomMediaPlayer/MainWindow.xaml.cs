using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

using CustomMediaPlayer.Option;
using CustomMediaPlayer.Utility;
using MahApps.Metro;
using MahApps.Metro.Controls;
using MahApps.Metro.IconPacks;
using NYoutubeDL;
using NYoutubeDL.Models;
using NAudio.Wave;
using static CustomMediaPlayer.MainMediaPlayer;
using CustomMediaPlayer.Option.Popup;
using System.Globalization;
using CustomMediaPlayer.Controllers.PlayList;
using CustomMediaPlayer.Core;

namespace CustomMediaPlayer
{
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ticks = new DispatcherTimer();
        public static OptionCore Optioncore = new OptionCore();
        public static PlayListCore PlayList = new PlayListCore();
        public MainWindowViewModel ViewModel;

        private bool StopButtonActive = false;

        // 느슨한 참조로 초기화 (메모리 누수 방지)
        #region 옵션 창 Optionwindow
        private static WeakReference weakreferenceoptionwindow;
        public OptionWindow Optionwindow
        {
            get { return (weakreferenceoptionwindow != null) ? weakreferenceoptionwindow.Target as OptionWindow : null; }
            set { weakreferenceoptionwindow = new WeakReference(value); }
        }
        #endregion
        #region 플레이리스트 창 PlayListwindow
        private static WeakReference weakreferenceplaylistwindow;
        public PlayListWindow PlayListwindow
        {
            get { return (weakreferenceplaylistwindow != null) ? weakreferenceplaylistwindow.Target as PlayListWindow : null; }
            set { weakreferenceplaylistwindow = new WeakReference(value); }
        }
        #endregion

        // public static string MediaSavePath = @"MediaCache\"; // 다운로드 미디어 저장 폴더

        public MainWindow()
        {
            InitializeComponent();

            #region 초기설정
            // DataContext 설정
            ViewModel = (MainWindowViewModel)this.DataContext;

            // 타이틀 설정
#if DEBUG
            this.Title = "TestVersion - CustomMediaPlayer";

            // 디버그 모드일 경우
            MainPopup.Child = new MainWindowPopupPage(PopupContents.DebugMode);
            MainPopup.IsOpen = true;
#else
            this.Title = "CustomMediaPlayer";
#endif

            // 미구현 기능 차단
            PreviousButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;
            TotalDurationLabel.IsEnabled = false;
            TotalDurationLabel.Visibility = Visibility.Collapsed;

            // 팝업 화면 종료이벤트 발생시 초기화
            MainPopup.Closed += (s, e) => { MainPopup.Child = null; };

            // 단축키 설정
            this.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Escape:
                        this.Close();
                        break;
                    case Key.Space:
                        MediaController_Click(PlayPauseButton, new RoutedEventArgs());
                        break;
                    case Key.P:
                        MediaController_Click(StopButton, new RoutedEventArgs());
                        break;
                    case Key.Down:
                    case Key.VolumeDown:
                        ViewModel.Volume--;
                        break;
                    case Key.Up:
                    case Key.VolumeUp:
                        ViewModel.Volume++;
                        break;
                    case Key.M:
                    case Key.VolumeMute:
                        if (ViewModel.Volume == 0)
                            ViewModel.Volume = ViewModel.BeforeVolume;
                        else
                            ViewModel.Volume = 0;
                        break;
                    case Key.Right:
                        ProgressSlider.Value += ProgressSlider.Maximum / 100;
                        break;
                    case Key.Left:
                        ProgressSlider.Value -= ProgressSlider.Maximum / 100;
                        break;
                }
            };

            // 볼륨
            VolumeSlider.Maximum = 100.0;
            VolumeSlider.Minimum = 0.0;

            // 마우스 휠을 사용한 볼륨조절
            this.MouseWheel += (s, e) =>
            {
                if (e.Delta > 0)
                {
                    if (Math.Abs(e.Delta) > 120)
                        ViewModel.Volume += 4;
                    ViewModel.Volume += 1;
                }
                else if (e.Delta < 0)
                {
                    if (Math.Abs(e.Delta) > 120)
                        ViewModel.Volume -= 4;
                    ViewModel.Volume -= 1;
                }
            };

            // 볼륨 뮤트 버튼
            VolumeMuteButton.Click += (s, e) =>
            {
                if (ViewModel.Volume > 0)
                    ViewModel.Volume = 0;
                else
                    ViewModel.Volume = ViewModel.BeforeVolume;
            };

            // 미디어 이미지
            MediaImage.Source = Utility.Utility.LogoNoteImage;

            // 남은 시간 <-> 전체 시간 전환
            TotalTimeLabel.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                { Optioncore.DurationViewStatus = !Optioncore.DurationViewStatus; }
            };

            // 저장 클래스 초기화 및 저장 값 로드
            this.Loaded += (s, e) =>
            {
                var optionSaveLoad = new OptionSaveLoad();
                optionSaveLoad.Load();
            };

            // 커서 설정
            PreviousButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            PlayPauseButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            StopButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            NextButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            ShuffleButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            RepeatButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            SettingButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            MediaListButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };

            PreviousButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            PlayPauseButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            StopButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            NextButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            ShuffleButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            RepeatButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            SettingButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            MediaListButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };

            TotalTimeLabel.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            TotalTimeLabel.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };

            // 포커스 설정
            PreviousButton.Focusable = false;
            PlayPauseButton.Focusable = false;
            StopButton.Focusable = false;
            NextButton.Focusable = false;
            ShuffleButton.Focusable = false;
            RepeatButton.Focusable = false;
            SettingButton.Focusable = false;
            MediaListButton.Focusable = false;

            ProgressSlider.Focusable = false;
            VolumeSlider.Focusable = false;
            VolumeMuteButton.Focusable = false;
            #endregion

            #region 이벤트 연결
            // 메인 윈도우
            this.Closing += MediaPlayer_Closing;

            // 미디어 플레이어
            mediaPlayer.PlaybackStopped += MediaPlayer_PlaybackStopped;
            AudioFileOpen += MediaPlayer_OpenStateChange;

            // 미디어 컨트롤러
            ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;

            // 버튼 클릭 이벤트 연결
            PreviousButton.Click += MediaController_Click;
            PlayPauseButton.Click += MediaController_Click;
            StopButton.Click += MediaController_Click;
            NextButton.Click += MediaController_Click;
            ShuffleButton.Click += MediaController_Click;
            RepeatButton.Click += MediaController_Click;
            SettingButton.Click += MediaController_Click;
            MediaListButton.Click += MediaController_Click;
            #endregion

            #region 툴팁 설정

            // 버튼
            PreviousButton.ToolTip = "Previous / 이전 곡";
            PlayPauseButton.ToolTip = "Play / 재생";
            StopButton.ToolTip = "Stop / 정지";
            NextButton.ToolTip = "Next / 다음 곡";
            ShuffleButton.ToolTip = "Shuffle / 셔플 (무작위 재생)";
            RepeatButton.ToolTip = "Repeat / 반복";
            SettingButton.ToolTip = "Option / 설정";
            MediaListButton.ToolTip = "Media List / 미디어 목록";

            // 볼륨
            VolumeSlider.ToolTip = "Volume / 볼륨";
            #endregion

            this.Loaded += (Ls, Le) =>
            {
                if (Environment.GetCommandLineArgs().Length > 1)
                {
                    foreach (var media in Environment.GetCommandLineArgs())
                    {
                        PlayList.Playlist.Add(new Core.MediaInfo(media));
                    }
                }
                //else
                //    NowPlayMedia = new Core.MediaInfo(new FileInfo(@"D:\Dif\Music\달의하루-염라_Karma.mp3"));
            };
        }

        #region 미디어 관련
        // 파일 열림 상태 변경 이벤트
        private void MediaPlayer_OpenStateChange(Core.MediaFullInfo File)
        {
            #region 컨트롤러 초기화
            ProgressSlider.Maximum = NowPlayMedia.Duration.TotalMilliseconds;
            //TotalTimeLabel.Content = nowPlayInfo.Duration.ToString(@"mm\:ss");
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += Ticks_Tick;
            ticks.Start();

            MediaImage.Source = NowPlayMedia.AlbumImage;

            SongTitleLabel.Content = NowPlayMedia.Title;
            AlbumTitleLabel.Content = NowPlayMedia.AlbumTitle;
            ArtistNameLabel.Content = NowPlayMedia.ArtistName;
            #endregion
        }

        // 재생 종료 이벤트
        private void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            NowPlayAudioStream.CurrentTime = TimeSpan.Zero;
            if (ViewModel.RepeatPlayOption == (int)RepeatStatus.Once && !StopButtonActive)
            {
                mediaPlayer.Play(); // 한곡 반복 설정
                StopButtonActive = false;
            }
            MediaPlayer_PlayStateChange();
        }

        // 상태 따른 UI 업데이트
        public void MediaPlayer_PlayStateChange()
        {
            // 플레이 버튼 아이콘 설정
            var PlayPauseIcon = new PackIconControl() { Width = 30, Height = 30 };
            if (mediaPlayer.PlaybackState == PlaybackState.Playing)
            {
                PlayPauseButton.ToolTip = "Pause / 일시정지";
                PlayPauseIcon.Kind = PackIconMaterialKind.Pause;
#if DEBUG
                this.Title = "TestVersion - CustomMediaPlayer - NowPlaying";
#else
                this.Title = "CustomMediaPlayer - NowPlaying";
#endif
            }
            else
            {
                PlayPauseButton.ToolTip = "Play / 재생";
                PlayPauseIcon.Kind = PackIconMaterialKind.Play;
#if DEBUG
                this.Title = "TestVersion - CustomMediaPlayer";
#else
                this.Title = "CustomMediaPlayer";
#endif
            }
            PlayPauseButton.Content = PlayPauseIcon;
        }
        #endregion

        #region 미디어 컨트롤러 관련
        #region 컨트롤 버튼
        #region enum 상수
        /// <summary>
        /// 미디어 컨트롤러 버튼 상수
        /// </summary>
        public enum MediaControl
        {
            Null,
            PlayPauseButton,
            StopButton,
            PreviousButton,
            NextButton,
            ShuffleButton,
            RepeatButton,
            SettingButton,
            MediaListButton
        }

        /// <summary>
        /// 반복 재생 상태 상수
        /// </summary>
        public enum RepeatStatus
        {
            Off,
            Once,
            All
        }
        #endregion

        // 미디어 컨트롤러 버튼 처리
        public void MediaController_Click(object sender, RoutedEventArgs e)
        {
            var button = MediaControl.Null;
            var _button = (Button)sender;

            #region _button을 상수(enum)으로 변환
            if (_button == PlayPauseButton)
                button = MediaControl.PlayPauseButton;
            else if (_button == StopButton)
                button = MediaControl.StopButton;
            else if (_button == PreviousButton)
                button = MediaControl.PreviousButton;
            else if (_button == NextButton)
                button = MediaControl.NextButton;
            else if (_button == ShuffleButton)
                button = MediaControl.ShuffleButton;
            else if (_button == RepeatButton)
                button = MediaControl.RepeatButton;
            else if (_button == SettingButton)
                button = MediaControl.SettingButton;
            else if (_button == MediaListButton)
                button = MediaControl.MediaListButton;
            #endregion

            if (NowPlayAudioStream == null && !(button == MediaControl.SettingButton) && !(button == MediaControl.MediaListButton))
            {
                // 미디어가 안 열렸을 경우
                MainPopup.Child = new MainWindowPopupPage(PopupContents.NowMediaFileNotToExist);
                MainPopup.IsOpen = true;
                return;
            }

            switch (button)
            {
                case MediaControl.Null:
                    return;
                case MediaControl.PlayPauseButton:
                    if (mediaPlayer.PlaybackState == PlaybackState.Playing)
                    { mediaPlayer.Pause(); }
                    else
                    { mediaPlayer.Play(); }
                    break;
                case MediaControl.StopButton:
                    StopButtonActive = true;
                    NowPlayAudioStream.CurrentTime = TimeSpan.Zero;
                    mediaPlayer.Stop();
                    break;
                case MediaControl.RepeatButton:
                    if (ViewModel.RepeatPlayOption == (int)RepeatStatus.Off)
                    { ViewModel.RepeatPlayOption = (int)RepeatStatus.Once; }
                    else if (ViewModel.RepeatPlayOption == (int)RepeatStatus.Once)
                    { ViewModel.RepeatPlayOption = (int)RepeatStatus.All; }
                    else if (ViewModel.RepeatPlayOption == (int)RepeatStatus.All)
                    { ViewModel.RepeatPlayOption = (int)RepeatStatus.Off; }
                    break;
                case MediaControl.SettingButton:
                    if (Optionwindow == null)
                        Optionwindow = new OptionWindow();
                    Optionwindow.Show();
                    Optionwindow.WindowState = WindowState.Normal;
                    Optionwindow.Activate();
                    Optionwindow.Closing += (SWs, SWe) => { Optionwindow = null; };
                    break;
                case MediaControl.MediaListButton:
                    if (PlayListwindow != null)
                    {
                        PlayListwindow.Activate();
                        PlayListwindow.WindowState = WindowState.Normal;
                        break;
                    }
                    if (PlayListwindow == null)
                    {
                        PlayListwindow = new PlayListWindow();
                        PlayListwindow.Show();
                        PlayListwindow.Closing += (MIs, MIe) =>
                        { PlayListwindow = null; };
                    }
                    break;
            }
            // UI 업데이트
            MediaPlayer_PlayStateChange();
        }
        #endregion

        // 틱 마다 미디어 진행 상황 동기화
        private void Ticks_Tick(object sender, EventArgs e)
        {
            if (NowPlayAudioStream != null)
            {
                ViewModel.CurrentPostion = NowPlayAudioStream.CurrentTime;
                ViewModel.totalTime = NowPlayMedia.Duration;
            }
        }

        // 진행 슬라이더 값 변경 이벤트 처리
        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
                NowPlayAudioStream.CurrentTime = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
                ProgressLabel.Content = Utility.Utility.TimeSpanStringConverter(NowPlayAudioStream.CurrentTime);
            }
        }
        #endregion

        #region 윈도우 관련
        // 메인 윈도우
        private void MediaPlayer_Closing(object sender, CancelEventArgs e)
        {
            OptionWindow.hooking.Stop();
            mediaPlayer.Stop();
            var optionSaveLoad = new OptionSaveLoad();
            optionSaveLoad.Save();
            PlayListSave playListSave = new PlayListSave(PlayList.Playlist);
            playListSave.Save();
            System.Windows.Application.Current.Shutdown();
        }

        private void FullScreenMode()
        {
            this.WindowState = WindowState.Maximized;
            this.UseNoneWindowStyle = true;
            this.IgnoreTaskbarOnMaximize = true;
        }

        private void NormalScreenmode()
        {
            this.WindowState = WindowState.Normal;
            this.UseNoneWindowStyle = false;
            this.ShowTitleBar = true; // 무조건 true로 변환 (중요)
            this.IgnoreTaskbarOnMaximize = false;
        }
        #endregion
    }
}
