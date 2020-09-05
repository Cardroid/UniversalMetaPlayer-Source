using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using UMP.Core;
using UMP.Core.Global;
using UMP.Core.Model.ViewModel;
using UMP.Utility;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class BasicOptionViewModel : ViewModelBase
  {
    public BasicOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        switch (e.PropertyName)
        {
          case "FileSavePath":
            OnPropertyChanged("FileSavePathToolTip");
            break;
          case "MediaLoadEngine":
            OnPropertyChanged("MediaLoadEngineSelectedItem");
            break;
          case "PrivateLogging":
            OnPropertyChanged("IsCheckedPrivateLogging");
            break;
          case "IsEnableSleepMode":
            OnPropertyChanged("IsCheckedIsEnableSleepMode");
            break;
          case "LyricsSettings":
            OnPropertyChanged("IsCheckedLyricsSettings_Off");
            OnPropertyChanged("IsCheckedLyricsSettings_Auto");
            OnPropertyChanged("IsCheckedLyricsSettings_On");
            break;
        }
      };

      OpenSaveDirectoryDialogCommand = new RelayCommand((o) => OpenSaveDirectoryDialog());

      MediaLoadEngineTypes = new List<ComboBoxItem>()
      {
        {new ComboBoxItem()
        {
          Content = Enums.MediaLoadEngineType.Native,
          ToolTip =
          "기본으로 사용하는 엔진입니다\n" +
          "오류 발생시 기본엔진 사용을 추천합니다\n" +
          "Local 미디어와 Youtube 미디어를 지원합니다"}},
        {new ComboBoxItem()
        {
          Content =Enums. MediaLoadEngineType.YoutubeDL,
          ToolTip =
          "Youtube-dl 를 기반으로한 엔진입니다\n" +
          "지원사이트는 Youtube-dl 홈페이지에서 확인 가능합니다",
          IsEnabled = false
        }}
      };

      LyricsSettingsCommand = new RelayCommand((o) => LyricsSettingsChange(o.ToString()));

      SetDefaultCommand = new RelayCommand((o) => SetDefault_Click());

      Log.LogViewerAppender.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "IsEnable")
          OnPropertyChanged("IsCheckedIsEnableLogViewer");
      };
    }

    #region 저장 경로
    public RelayCommand OpenSaveDirectoryDialogCommand { get; }
    private void OpenSaveDirectoryDialog()
    {
      var result = DialogHelper.OpenDirectoryDialog("저장 폴더 선택", GlobalProperty.Options.Getter<string>(Enums.ValueName.FileSavePath));
      if (result)
        GlobalProperty.Options.Setter(Enums.ValueName.FileSavePath, result.Result);
    }
    public string FileSavePathToolTip =>
      $"현재 설정 : \"{Path.GetFullPath(GlobalProperty.Options.Getter<string>(Enums.ValueName.FileSavePath))}\"\n\n" +

      $"프로그램에서 파일을 보관하는 폴더입니다. 빈 폴더를 권장합니다.";
    #endregion

    #region 세부 로깅 여부
    public bool IsCheckedPrivateLogging
    {
      get => GlobalProperty.Options.Getter<bool>(Enums.ValueName.IsPrivateLogging);
      set => GlobalProperty.Options.Setter(Enums.ValueName.IsPrivateLogging, value.ToString());
    }

    public string PrivateLoggingToolTip =>
      $"기본값 : {GlobalProperty.DefaultValue.GetDefaultValue<bool>(Enums.ValueName.IsPrivateLogging)}\n\n" +

      "로그에 더 자세한 사항을 기록합니다.\n" +
      "프로그램 경로, 미디어 정보등이 기록됩니다.";
    #endregion

    #region 미디어 로드 엔진
    public int MediaLoadEngineSelectedItem
    {
      get => (int)GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine);
      set
      {
        GlobalProperty.Options.Setter(Enums.ValueName.MediaLoadEngine, MediaLoadEngineTypes[value].Content.ToString());
        OnPropertyChanged("MediaLoadEngineSelectedItem");
      }
    }
    public List<ComboBoxItem> MediaLoadEngineTypes { get; }

    public string MediaLoadEngineToolTip =>
      $"기본값 : {GlobalProperty.DefaultValue.GetDefaultValue<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine)}\n" +
      $"(클릭후 드래그로 선택해야 합니다)\n\n" +

      $"미디어를 불러올 때 사용하는 엔진입니다.";
    #endregion

    #region 가사창
    public RelayCommand LyricsSettingsCommand { get; }
    private void LyricsSettingsChange(string value) => GlobalProperty.Options.Setter(Enums.ValueName.LyricsSettings, value);
    public bool IsCheckedLyricsSettings_Off => GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings) == Enums.LyricsSettingsType.Off;
    public bool IsCheckedLyricsSettings_Auto => GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings) == Enums.LyricsSettingsType.Auto;
    public bool IsCheckedLyricsSettings_On => GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings) == Enums.LyricsSettingsType.On;

    public string LyricsSettingsToolTip =>
      $"기본값 : {GlobalProperty.DefaultValue.GetDefaultValue<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings)}\n" +
      $"On = 항상 열기, Auto = 가사가 있는 경우만 열기, Off = 항상 닫기\n\n" +

      $"가사를 볼 수 있는 창을 활성화 합니다.";
    #endregion

    #region 로그 뷰어 활성화
    public bool IsCheckedIsEnableLogViewer
    {
      get => Log.LogViewerAppender.IsEnable;
      set => Log.LogViewerAppender.IsEnable = value;
    }

    public string IsEnableLogViewerToolTip =>
      $"기본값 : {false}\n" +
      "해당 옵션은 저장되지 않습니다.\n\n" +

      "로그기록을 실시간으로 확인할 수 있는 창을 활성화합니다.";
    #endregion

    #region 옵션 설정 값 초기화
    public void SetDispatcher(Dispatcher dispatcher) => ViewDispatcher = dispatcher;
    private Dispatcher ViewDispatcher { get; set; }
    public RelayCommand SetDefaultCommand { get; }
    public Brush SetDefaultButtenForeground { get; set; } = ThemeHelper.IsDarkMode ? Brushes.White : Brushes.Black;

    private bool IsReset
    {
      get => _IsReset;
      set
      {
        _IsReset = value;

        if (_IsReset)
          SetDefaultButtenForeground = Brushes.Red;
        else
          SetDefaultButtenForeground = ThemeHelper.IsDarkMode ? Brushes.White : Brushes.Black;

        OnPropertyChanged("SetDefaultButtenForeground");
      }
    }
    private bool _IsReset = false;

    private Timer IsResetLockTimer;

    private void SetDefault_Click()
    {
      if (IsResetLockTimer == null)
      {
        IsResetLockTimer = new Timer(3000) { AutoReset = true };
        IsResetLockTimer.Elapsed += (_, e) =>
        {
          IsResetLockTimer.Stop();
          ViewDispatcher.Invoke(() => { IsReset = false; });
        };
      }

      if (IsReset)
      {
        IsReset = false;
        GlobalProperty.SetDefault();
      }
      else
      {
        IsReset = true;
        IsResetLockTimer.Start();
      }
    }
    #endregion
  }
}
