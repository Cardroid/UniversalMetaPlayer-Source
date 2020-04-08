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

namespace CustomMediaPlayer
{
    public partial class MainWindow : MetroWindow
    {
        private DispatcherTimer ticks = new DispatcherTimer();
        public static ImageSource LogoImage = new BitmapImage(new Uri(@"Resources\IconCustomMusicPlayer.png", UriKind.Relative));
        public static Utility.Utility Utility = new Utility.Utility();
        public OptionWindow Optionwindow;
        public MainWindowViewModel viewModel;

        // 느슨한 참조로 초기화 (메모리 누수 방지)
        #region 미디어 정보 창 MediaInfoWindow
        private static WeakReference weakreferencemediaInfoWindow;
        public static Controllers.MediaInfoWindow MediaInfowindow
        {
            get { return (weakreferencemediaInfoWindow != null) ? weakreferencemediaInfoWindow.Target as Controllers.MediaInfoWindow : null; }
            set { weakreferencemediaInfoWindow = new WeakReference(value); }
        }
        #endregion

        public static string MediaSavePath = @"MediaCache\"; // 다운로드 미디어 저장 폴더

        public MainWindow()
        {
            InitializeComponent();

            // 저장 클래스 초기화 및 저장 값 로드
            var optionSaveLoad = new OptionSaveLoad();
            optionSaveLoad.Load();

            #region 초기설정
            // DataContext 설정
            viewModel = (MainWindowViewModel)this.DataContext;
            //this.DataContext = new MainWindowViewModel();

            // 옵션창 숨김
            Optionwindow = new OptionWindow();
            Optionwindow.Visibility = Visibility.Collapsed;

            // 미구현 기능 차단
            PreviousButton.IsEnabled = false;
            NextButton.IsEnabled = false;
            ShuffleButton.IsEnabled = false;
            MediaListButton.IsEnabled = false;
            TotalDurationLabel.IsEnabled = false;
            TotalDurationLabel.Visibility = Visibility.Collapsed;

            // 팝업 화면 종료이벤트 발생시 초기화
            MainPopup.Closed += (s, e) => { MainPopup.Child = null; };

            // 단축키 설정
            this.KeyDown += (s, e) =>
            {
                switch (e.Key)
                {
                    case Key.Space:
                        MediaController_Click(PlayPauseButton,new RoutedEventArgs());
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
                        viewModel.Volume += 4;
                    viewModel.Volume += 1;
                }
                else if (e.Delta < 0)
                {
                    if (Math.Abs(e.Delta) > 120)
                        viewModel.Volume -= 4;
                    viewModel.Volume -= 1;
                }
            };

            // 볼륨 뮤트 버튼
            VolumeMuteButton.Click += (s, e) =>
            {
                if (viewModel.Volume > 0)
                    viewModel.Volume = 0;
                else
                    viewModel.Volume = viewModel.BeforeVolume;
            };

            // 미디어 이미지
            MediaImage.Source = LogoImage;

            // 남은 시간 <-> 전체 시간 전환
            TotalTimeLabel.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Left)
                { viewModel.DurationViewStatus = !viewModel.DurationViewStatus; }
            };

            // 커서 설정
            PreviousButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            PlayPauseButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            StopButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            NextButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            ShuffleButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            RepeatButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            SettingButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            MediaInfoButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };
            MediaListButton.MouseEnter += (s, e) => { this.Cursor = Cursors.Hand; };

            PreviousButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            PlayPauseButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            StopButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            NextButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            ShuffleButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            RepeatButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            SettingButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
            MediaInfoButton.MouseLeave += (s, e) => { this.Cursor = Cursors.Arrow; };
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
            MediaInfoButton.Focusable = false;
            MediaListButton.Focusable = false;

            ProgressSlider.Focusable = false;
            VolumeSlider.Focusable = false;
            VolumeMuteButton.Focusable = false;
            #endregion

            #region 이벤트 연결
            // 메인 윈도우
            this.Closing += MediaPlayer_Closing;

            // 미디어 플래이어
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
            MediaInfoButton.Click += MediaController_Click;
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
            MediaInfoButton.ToolTip = "Media Info / 미디어 정보";
            MediaListButton.ToolTip = "Media List / 미디어 목록";

            // 볼륨
            VolumeSlider.ToolTip = "Volume / 볼륨";
            #endregion

            this.Loaded += (Ls, Le) =>
            {
                if (Environment.GetCommandLineArgs().Length == 2)
                    NowPlayFile = new FileInfo(Environment.GetCommandLineArgs()[1]);
                //else
                //    NowAudioFile = new FileInfo(@"D:\Dif\Music\달의하루-염라_Karma.mp3");
            };
        }

        #region 미디어 관련
        // 파일 열림 상태 변경 이벤트
        private void MediaPlayer_OpenStateChange(FileInfo File)
        {
            #region 컨트롤러 초기화
            ProgressSlider.Maximum = NowPlayMediaInfo.Duration.TotalMilliseconds;
            //TotalTimeLabel.Content = nowPlayInfo.Duration.ToString(@"mm\:ss");
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += Ticks_Tick;
            ticks.Start();

            MediaImage.Source = NowPlayMediaInfo.AlbumImage;

            SongTitleLabel.Content = NowPlayMediaInfo.Title;
            AlbumTitleLabel.Content = NowPlayMediaInfo.AlbumTitle;
            ArtistNameLabel.Content = NowPlayMediaInfo.ArtistName;
            #endregion
        }

        // 재생 종료 이벤트
        private void MediaPlayer_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            NowPlayStream.CurrentTime = TimeSpan.Zero;
            if (viewModel.RepeatPlayOption == (int)RepeatStatus.Once)
                mediaPlayer.Play(); // 한곡 반복 설정
            MediaPlayer_PlayStateChange();
        }

        // 상태 따른 UI 업데이트
        public void MediaPlayer_PlayStateChange()
        {
            // 플래이 버튼 아이콘 설정
            var PlayPauseIcon = new PackIconControl() { Width = 30, Height = 30 };
            if (mediaPlayer.PlaybackState == PlaybackState.Playing)
            {
                PlayPauseButton.ToolTip = "Pause / 일시정지";
                PlayPauseIcon.Kind = PackIconMaterialKind.Pause;
                this.Title = "CustomMediaPlayer - NowPlaying";
            }
            else
            {
                PlayPauseButton.ToolTip = "Play / 재생";
                PlayPauseIcon.Kind = PackIconMaterialKind.Play;
                this.Title = "CustomMediaPlayer";
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
            MediaInfoButton,
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
        private void MediaController_Click(object sender, RoutedEventArgs e)
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
            else if (_button == MediaInfoButton)
                button = MediaControl.MediaInfoButton;
            else if (_button == MediaListButton)
                button = MediaControl.MediaListButton;
            #endregion

            if (NowPlayStream == null && !(button == MediaControl.SettingButton) && !(button == MediaControl.MediaListButton))
            {
                // 미디어가 안 열렸을 경우
                MainPopup.Child = new NotExistMediaPopupPage();
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
                    mediaPlayer.Pause();
                    NowPlayStream.CurrentTime = TimeSpan.Zero;
                    break;
                case MediaControl.RepeatButton:
                    if (viewModel.RepeatPlayOption == (int)RepeatStatus.Off)
                    { viewModel.RepeatPlayOption = (int)RepeatStatus.Once; }
                    else if (viewModel.RepeatPlayOption == (int)RepeatStatus.Once)
                    { viewModel.RepeatPlayOption = (int)RepeatStatus.All; }
                    else if (viewModel.RepeatPlayOption == (int)RepeatStatus.All)
                    { viewModel.RepeatPlayOption = (int)RepeatStatus.Off; }
                    break;
                case MediaControl.SettingButton:
                    Optionwindow.Visibility = Visibility.Visible;
                    Optionwindow.WindowState = WindowState.Normal;
                    Optionwindow.Activate();
                    Optionwindow.Closing += (SWs, SWe) => { var optionSaveLoad = new OptionSaveLoad(); optionSaveLoad.Save(); Optionwindow.Visibility = Visibility.Visible; };
                    break;
                case MediaControl.MediaInfoButton:
                    if (NowPlayMediaInfo == null)
                    {
                        MainPopup.Child = new NotExistMediaPopupPage();
                        MainPopup.IsOpen = true;
                        return;
                    }
                    if (MediaInfowindow != null)
                    {
                        MediaInfowindow.Activate();
                        MediaInfowindow.WindowState = WindowState.Normal;
                        return;
                    }
                    if (MediaInfowindow == null)
                    {
                        MediaInfowindow = new Controllers.MediaInfoWindow();
                        MediaInfowindow.Show();
                        MediaInfowindow.Closing += (MIs, MIe) =>
                        { MediaInfowindow = null; };
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
            if (NowPlayStream != null)
            {
                viewModel.currentPostion = NowPlayStream.CurrentTime;
                viewModel.totalTime = NowPlayMediaInfo.Duration;
            }
        }

        // 진행 슬라이더 값 변경 이벤트 처리
        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
                NowPlayStream.CurrentTime = new TimeSpan(0, 0, 0, 0, (int)e.NewValue);
                ProgressLabel.Content = Utility.TimeSpanStringConverter(NowPlayStream.CurrentTime);
            }
        }
        #endregion

        #region 윈도우 이벤트
        // 메인 윈도우
        private void MediaPlayer_Closing(object sender, CancelEventArgs e)
        {
            OptionWindow.hooking.Stop();
            mediaPlayer.Stop();
            var optionSaveLoad = new OptionSaveLoad();
            optionSaveLoad.Save();
            Application.Current.Shutdown();
        }
        #endregion
    }
}
