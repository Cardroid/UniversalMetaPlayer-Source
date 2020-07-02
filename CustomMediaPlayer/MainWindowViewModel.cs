using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CustomMediaPlayer.Option;

using MahApps.Metro.IconPacks;

using static CustomMediaPlayer.MainMediaPlayer;

namespace CustomMediaPlayer
{
  /// <summary>
  /// 메인 윈도우 뷰 모델
  /// </summary>
  public class MainWindowViewModel : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    private void Notify(string propName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); 

    private MainWindow mainWindow => (MainWindow)System.Windows.Application.Current.MainWindow; // 메인윈도우 참조 (코드 가독성을 위함)

    // 전채 재생시간 (단일 파일)
    public bool DurationViewStatus => MainWindow.Optioncore.DurationViewStatus;
    private TimeSpan _TotalTime;
    public TimeSpan TotalTime
    {
      get => _TotalTime;
      set
      {
        _TotalTime = value;
        Notify("TotalTimestring");
      }
    }

    public string TotalTimestring
    {
      get
      {
        if (NowPlayAudioStream == null) // 오류 방지용
          return Utility.Utility.TimeSpanStringConverter(TimeSpan.Zero);
        if (DurationViewStatus)
          return Utility.Utility.TimeSpanStringConverter(TotalTime); // 전채 시간
        else
          return "-" + Utility.Utility.TimeSpanStringConverter(TotalTime - _Currentpostion); // 남은 시간
      }
    }

    // 현재 재생위치
    private TimeSpan _Currentpostion;
    public TimeSpan CurrentPostion
    {
      get => _Currentpostion;
      set
      {
        _Currentpostion = value;
        Notify("CurrentPostiondouble");
        Notify("CurrentPostionstring");
      }
    }
    public double CurrentPostiondouble
    {
      get
      {
        if (NowPlayAudioStream == null) // 오류 방지용
          return 0;
        else
          return _Currentpostion.TotalMilliseconds;
      }
      set => NowPlayAudioStream.CurrentTime = TimeSpan.FromMilliseconds(value);
    }
    public string CurrentPostionstring
    {
      get
      {
        if (NowPlayAudioStream == null) // 오류 방지용
          return Utility.Utility.TimeSpanStringConverter(TimeSpan.Zero);
        else
          return Utility.Utility.TimeSpanStringConverter(_Currentpostion);
      }
    }

    // 반복재생
    public PackIconBase RepeatPlayOptionIcon { get; private set; } = new PackIconControl { Width = 20, Height = 20, Kind = PackIconMaterialKind.RepeatOff };
    private int _RepeatPlayOption;
    public int RepeatPlayOption
    {
      get => _RepeatPlayOption;
      set
      {
        if (value < 0)
          _RepeatPlayOption = 2;
        else if (value > 3)
          _RepeatPlayOption = 0;
        else
          _RepeatPlayOption = value;
        // 반복 아이콘 설정
        var RepeatIcon = new PackIconControl() { Width = 20, Height = 20 };
        if (_RepeatPlayOption == 0) // 반복 안함
        { RepeatIcon.Kind = PackIconMaterialKind.RepeatOff; }
        else if (_RepeatPlayOption == 1) // 한 곡 반복
        { RepeatIcon.Kind = PackIconMaterialKind.RepeatOnce; }
        else if (_RepeatPlayOption == 2) // 전채 반복
        { RepeatIcon.Kind = PackIconMaterialKind.Repeat; }
        RepeatPlayOptionIcon = RepeatIcon;
        Notify("RepeatPlayOptionIcon");
      }
    }

    // 볼륨
    private int _Volume = 0;
    public int BeforeVolume { get; private set; } = 1;
    public string VolumeString { get => _Volume.ToString() + "%"; }
    public int Volume
    {
      get => _Volume;
      set
      {
        _Volume = Math.Max(0, Math.Min(100, value)); // 오류 방지용
        if (_Volume != 0)
          BeforeVolume = _Volume;
        MainMediaPlayer.Volume = _Volume / 100f;
        VolumeMuteButtonIcon = VolumeMuteButtonIconChanger();
        Notify("Volume");
        Notify("VolumeString");
      }
    }

    // 볼륨 뮤트 버튼 아이콘 관련
    private PackIconBase _VolumeMuteButtonIcon;
    public PackIconBase VolumeMuteButtonIcon
    {
      get => _VolumeMuteButtonIcon ?? VolumeMuteButtonIconChanger();
      set
      {
        _VolumeMuteButtonIcon = value;
        Notify("VolumeMuteButtonIcon");
      }
    }
    public PackIconBase VolumeMuteButtonIconChanger()
    {
      var Icon = new PackIconControl() { Width = 15, Height = 15 };
      if (_Volume > 75)
        Icon.Kind = PackIconMaterialKind.VolumeHigh;
      else if (_Volume > 35)
        Icon.Kind = PackIconMaterialKind.VolumeMedium;
      else if (_Volume > 0)
        Icon.Kind = PackIconMaterialKind.VolumeLow;
      else if (_Volume == 0)
        Icon.Kind = PackIconMaterialKind.VolumeOff;
      return Icon;
    }

    // 배경색 관련 이벤트와 프로퍼티 정의
    #region 배경색 관련
    public delegate void BackgroundColorChangedHandler(Brush brush);
    public event BackgroundColorChangedHandler BackgroundColorChanged;
    private Brush _BackgroundBrush;
    public Brush BackgroundBrush
    {
      get =>_BackgroundBrush;
      set
      {
        _BackgroundBrush = value;
        if (BackgroundColorChanged != null)
          BackgroundColorChanged.Invoke(_BackgroundBrush);
        Notify("BackgroundBrush");
      }
    }
    #endregion
  }
}
