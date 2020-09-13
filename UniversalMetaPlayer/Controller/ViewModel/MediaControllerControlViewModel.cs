using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;
using MaterialDesignColors.ColorManipulation;

using NAudio.Wave;

using UMP.Core.Player;
using UMP.Core.Model.ViewModel;
using UMP.Core.Global;
using UMP.Controller.Function;
using UMP.Controller.WindowHelper;
using UMP.Utils;
using UMP.Core;
using System.Timers;

namespace UMP.Controller.ViewModel
{
  public class MediaControllerControlViewModel : ViewModelBase
  {
    public MediaControllerControlViewModel()
    {
      MainMediaPlayer.PlayStateChanged += MainMediaPlayer_PlayStateChanged;
      MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged;
      MainMediaPlayer.TickEvent += MainMediaPlayer_TickEvent;
      MainMediaPlayer.Option.PropertyChanged += PlayerOption_PropertyChanged;
      MainMediaPlayer.PlayList.Field_PropertyChanged += PlayList_Field_PropertyChanged;
      ThemeHelper.ThemeChangedEvent += ThemeHelper_ThemeChangedEvent;

      GlobalKeyDownEvent.KeyDownEvent += GlobalKeyDownEvent_KeyDownEvent;

      PlayPauseCommand = new RelayCommand((o) => PlayPause());
      StopCommand = new RelayCommand((o) => Stop());
      NextCommand = new RelayCommand((o) => Next());
      PreviousCommand = new RelayCommand((o) => Previous());
      RepeatCommand = new RelayCommand((o) => Repeat());
      ShuffleCommand = new RelayCommand((o) => Shuffle());
    }

    #region Volume
    private float _BeforeVolume = 80;
    public float BeforeVolume
    {
      get => _BeforeVolume;
      private set => _BeforeVolume = Math.Clamp(value, 10, 100);
    }
    public float Volume
    {
      get => MainMediaPlayer.Volume * 100;
      set
      {
        MainMediaPlayer.Volume = Math.Clamp(value / 100f, 0f, 1f); // 오류 방지용
        if (MainMediaPlayer.Volume != 0)
          BeforeVolume = Volume;
      }
    }

    // 볼륨 버튼 관련
    public PackIconKind VolumeMuteButtonIcon => VolumeMuteButtonIconChanger();
    private PackIconKind VolumeMuteButtonIconChanger()
    {
      if (Volume > 70)
        return PackIconKind.VolumeHigh;
      else if (Volume > 25)
        return PackIconKind.VolumeMedium;
      else if (Volume > 0)
        return PackIconKind.VolumeLow;
      else
        return PackIconKind.VolumeOff;
    }
    #endregion

    #region Play / Pause
    public RelayCommand PlayPauseCommand { get; }

    private void PlayPause()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        if (MainMediaPlayer.PlaybackState == PlaybackState.Playing)
          MainMediaPlayer.ReserveCommand(PlaybackState.Paused);
        else
          MainMediaPlayer.ReserveCommand(PlaybackState.Playing);
      }
    }

    public PackIcon PlayPauseStateIcon
    {
      get
      {
        if (MainMediaPlayer.PlaybackState == PlaybackState.Playing)
          return new PackIcon { Width = 50, Height = 50, Kind = PackIconKind.Pause };
        else
          return new PackIcon { Width = 50, Height = 50, Kind = PackIconKind.Play };
      }
    }
    #endregion

    #region Stop
    public RelayCommand StopCommand { get; }
    private void Stop() => MainMediaPlayer.ReserveCommand(PlaybackState.Stopped);
    #endregion

    #region Next
    public RelayCommand NextCommand { get; }
    private void Next() => _ = MainMediaPlayer.Next();
    #endregion

    #region Previous
    public RelayCommand PreviousCommand { get; }
    private void Previous() => _ = MainMediaPlayer.Previous();
    #endregion

    #region Repeat
    public RelayCommand RepeatCommand { get; }
    private void Repeat() => MainMediaPlayer.Option.RepeatPlayOption++;
    public PackIcon RepeatPlayOptionIcon
    {
      get
      {
        var RepeatIcon = new PackIcon() { Width = 30, Height = 30 };
        if (MainMediaPlayer.Option.RepeatPlayOption == 0) // 반복 안함
        { RepeatIcon.Kind = PackIconKind.RepeatOff; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 1) // 한 곡 반복
        { RepeatIcon.Kind = PackIconKind.RepeatOnce; }
        else if (MainMediaPlayer.Option.RepeatPlayOption == 2) // 전채 반복
        { RepeatIcon.Kind = PackIconKind.Repeat; }
        return RepeatIcon;
      }
    }
    #endregion

    #region Shuffle
    public RelayCommand ShuffleCommand { get; }
    private void Shuffle() => MainMediaPlayer.Option.Shuffle = !MainMediaPlayer.Option.Shuffle;
    public PackIcon ShuffleIcon
    {
      get
      {
        if (MainMediaPlayer.Option.Shuffle)
          return new PackIcon { Width = 30, Height = 30, Kind = PackIconKind.ShuffleVariant };
        else
          return new PackIcon { Width = 40, Height = 40, Kind = PackIconKind.ShuffleDisabled };
      }
    }
    #endregion

    #region FunctionControl
    public bool IsCheckedFunctionToggleButton
    {
      get => _IsCheckedFunctionToggleButton;
      set
      {
        WindowManager.Controller.Close(Core.Model.Control.WindowKind.Function);
        _IsCheckedFunctionToggleButton = value;
        if (_IsCheckedFunctionToggleButton)
        {
          var windowProperty = WindowManager.Controller[Core.Model.Control.WindowKind.Function];
          if (windowProperty == null || windowProperty.IsClosed)
            FunctionWindowInit();
          WindowManager.Controller.Open(Core.Model.Control.WindowKind.Function);
        }
        OnPropertyChanged("IsCheckedFunctionToggleButton");
      }
    }
    private bool _IsCheckedFunctionToggleButton = false;

    private void FunctionWindowInit()
    {
      var windowProperty = WindowManager.Controller.Create(Core.Model.Control.WindowKind.Function);
      windowProperty.Window.Closing += (_, e) =>
      {
        _IsCheckedFunctionToggleButton = false;
        OnPropertyChanged("IsCheckedFunctionToggleButton");
      };
    }
    #endregion

    #region PlayListControl
    public bool IsCheckedPlayListToggleButton
    {
      get => _IsCheckedPlayListToggleButton;
      set
      {
        WindowManager.Controller.Close(Core.Model.Control.WindowKind.PlayList);
        _IsCheckedPlayListToggleButton = value;
        if (_IsCheckedPlayListToggleButton)
        {
          var windowProperty = WindowManager.Controller[Core.Model.Control.WindowKind.PlayList];
          if (windowProperty == null || windowProperty.IsClosed)
            PlayListWindowInit();
          WindowManager.Controller.Open(Core.Model.Control.WindowKind.PlayList);
        }
        OnPropertyChanged("IsCheckedPlayListToggleButton");
      }
    }
    private bool _IsCheckedPlayListToggleButton = false;

    private bool PlayListCloseCount
    {
      get => _PlayListCloseCount;
      set
      {
        _PlayListCloseCount = value;

        if(_PlayListCloseCount)
        {
          if(PlayListCloseCountTimer == null)
          {
            PlayListCloseCountTimer = new Timer(3000) { AutoReset = true };
            PlayListCloseCountTimer.Elapsed += (_, e) =>
            {
              _PlayListCloseCount = false;
              PlayListCloseCountTimer.Stop();
            };
          }
          PlayListCloseCountTimer.Start();
        }
      }
    }
    private bool _PlayListCloseCount = false;

    private Timer PlayListCloseCountTimer;

    private void PlayListWindowInit()
    {
      var windowProperty = WindowManager.Controller.Create(Core.Model.Control.WindowKind.PlayList);
      windowProperty.Window.Closing += (_, e) =>
      {
        if (!PlayListCloseCount && ((PlayListControlViewModel)windowProperty.Window.ViewModel.UserControl.DataContext).PlayList.NeedSave)
        {
          GlobalMessageEvent.Invoke("플래이 리스트에 변경사항이 있습니다 (저장 필요)\n(무시하고 닫으려면 다시 시도하세요)", true);
          PlayListCloseCount = true;
          e.Cancel = true;
          return;
        }
        _IsCheckedPlayListToggleButton = false;
        OnPropertyChanged("IsCheckedPlayListToggleButton");
      };
    }
    #endregion

    #region 플레이 시간 UI

    // 재생길이 (단일 파일)
    public TimeSpan DurationTime =>
      MainMediaPlayer.MediaLoadedCheck
      ? MainMediaPlayer.MediaInformation.Duration
      : TimeSpan.Zero;

    // 전채 재생길이 (복수 파일)
    public TimeSpan TotalDuration =>
      MainMediaPlayer.PlayList != null
      ? MainMediaPlayer.PlayList.TotalDuration
      : TimeSpan.Zero;
    
    public string DurationTimestring
    {
      get
      {
        if (MainMediaPlayer.Option.DurationViewStatus)
          return Converter.TimeSpanStringConverter(DurationTime); // 전채 시간
        else
          return "-" + Converter.TimeSpanStringConverter(DurationTime - CurrentPostion); // 남은 시간
      }
    }

    // 현재 재생위치
    public TimeSpan CurrentPostion =>
      MainMediaPlayer.MediaLoadedCheck
      ? MainMediaPlayer.AudioCurrentTime
      : TimeSpan.Zero;
    public string CurrentPostionstring => Converter.TimeSpanStringConverter(CurrentPostion);
    #endregion

    #region ToggleButton
    public Brush ToggleButtonBackground => new SolidColorBrush(ThemeHelper.PrimaryColor.Darken(3));
    #endregion

    #region HotKey
    /// <summary>
    /// 컨트롤 키보드 이벤트 처리 (내부 이벤트)
    /// </summary>
    private void GlobalKeyDownEvent_KeyDownEvent(KeyEventArgs e)
    {
      if (GlobalProperty.State.IsControllable)
      {
        switch (GlobalProperty.Options.HotKey.Getter(e.Key))
        {
          case GlobalProperty.Options.HotKey.ControlTarget.PlayPause:
            if (PlayPauseCommand.CanExecute(null))
              PlayPauseCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Stop:
            if (StopCommand.CanExecute(null))
              StopCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Previous:
            if (PreviousCommand.CanExecute(null))
              PreviousCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Next:
            if (NextCommand.CanExecute(null))
              NextCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Mute:
            if (Volume > 0)
              Volume = 0;
            else
              Volume = BeforeVolume;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Repeat:
            if (RepeatCommand.CanExecute(null))
              RepeatCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.Shuffle:
            if (ShuffleCommand.CanExecute(null))
              ShuffleCommand.Execute(null);
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionForward:
            MediaPositionChanger(TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.MediaPositionBack:
            MediaPositionChanger(-TimeSpan.FromSeconds(5));
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeUp:
            Volume += 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.VolumeDown:
            Volume -= 5;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.PlayListWindow:
            IsCheckedPlayListToggleButton = !IsCheckedPlayListToggleButton;
            break;
          case GlobalProperty.Options.HotKey.ControlTarget.FunctionWindow:
            IsCheckedFunctionToggleButton = !IsCheckedFunctionToggleButton;
            break;
          default:
            break;
        }
      }
    }

    private void MediaPositionChanger(TimeSpan appendtime)
    {
      if (!MainMediaPlayer.MediaLoadedCheck || appendtime == TimeSpan.Zero)
        return;

      if (appendtime > TimeSpan.Zero)
      {
        // 양수
        if (MainMediaPlayer.AudioTotalTime > (MainMediaPlayer.AudioCurrentTime + appendtime))
          MainMediaPlayer.AudioCurrentTime += appendtime;
        else
          MainMediaPlayer.AudioCurrentTime = MainMediaPlayer.AudioTotalTime;
      }
      else
      {
        // 음수
        if (MainMediaPlayer.AudioCurrentTime + appendtime > TimeSpan.Zero)
          MainMediaPlayer.AudioCurrentTime += appendtime;
        else
          MainMediaPlayer.AudioCurrentTime = TimeSpan.Zero;
      }
      ApplyUI(false);
    }
    #endregion

    #region 동기화 메소드
    private void PlayerOption_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "DurationViewStatus":
        OnPropertyChanged("DurationTimestring");
          break;
        case "RepeatPlayOption":
        OnPropertyChanged("RepeatPlayOptionIcon");
          break;
        case "Shuffle":
        OnPropertyChanged("ShuffleIcon");
          break;
      }
    }
    private void MainMediaPlayer_PlayStateChanged(PlaybackState state) => ApplyUI();
    private void MainMediaPlayer_TickEvent(object sender, EventArgs e) => ApplyUI(false);
    private void MainMediaPlayer_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "MainPlayerInitialized":
          ApplyUI();
          break;
        case "Volume":
          OnPropertyChanged("Volume");
          OnPropertyChanged("VolumeString");
          OnPropertyChanged("VolumeMuteButtonIcon");
          break;
      }
    }
    private void PlayList_Field_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "TotalDuration")
        OnPropertyChanged("TotalDuration");      // 플레이리스트 재생길이 총합 적용
    }
    private void ThemeHelper_ThemeChangedEvent(ThemeHelper.ThemeProperty e)
    {
      OnPropertyChanged("ToggleButtonBackground"); // 테마 색 변경 적용
    }

    /// <summary>
    /// 현재 플레이어 상태를 UI에 적용합니다 (UI 갱신)
    /// </summary>
    /// <param name="fullapply">모든 정보를 적용할지의 여부</param>
    public void ApplyUI(bool fullapply = true)
    {
      if (!MainMediaPlayer.Option.DurationViewStatus || fullapply)
        OnPropertyChanged("DurationTimestring"); // 라벨 총 제생시간 적용
      if (fullapply)
      {
        OnPropertyChanged("DurationTime");       // 슬라이드 바 최대 길이 적용
        OnPropertyChanged("PlayPauseStateIcon"); // Play/Pause 버튼 아이콘 새로고침 적용
      }
      OnPropertyChanged("CurrentPostion");
      OnPropertyChanged("CurrentPostiondouble"); // 슬라이드 바 현재 재생위치 적용
      OnPropertyChanged("CurrentPostionstring"); // 라벨 현재 재생위치 적용
    }
    #endregion
  }
}
