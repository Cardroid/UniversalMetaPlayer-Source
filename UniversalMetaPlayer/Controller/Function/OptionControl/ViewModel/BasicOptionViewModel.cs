using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using UMP.Core.Global;
using UMP.Core.Model.ViewModel;

namespace UMP.Controller.Function.OptionControl.ViewModel
{
  public class BasicOptionViewModel : ViewModelBase
  {
    public BasicOptionViewModel()
    {
      GlobalProperty.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "FileSavePath")
          OnPropertyChanged("FileSavePathToolTip");
        if (e.PropertyName == "PrivateLogging")
          OnPropertyChanged("PrivateLoggingIsChecked");
        if (e.PropertyName == "MediaLoadEngine")
          OnPropertyChanged("MediaLoadEngineSelectedItem");
        if (e.PropertyName == "LyricsWindowActive")
        {
          OnPropertyChanged("IsCheckedLyricsWindowActive_Off");
          OnPropertyChanged("IsCheckedLyricsWindowActive_Auto");
          OnPropertyChanged("IsCheckedLyricsWindowActive_On");
        }
      };

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

      LyricsWindowActiveCommand = new RelayCommand((o) => LyricsWindowActiveChange((Enums.LyricsSettingsType)o));
    }

    #region 저장 경로
    public string FileSavePathToolTip =>
      $"현재 설정 : \"{Path.GetFullPath(GlobalObj.Property.Options.Getter<string>(Enums.ValueName.FileSavePath))}\"\n\n" +

      $"프로그램에서 파일을 보관하는 폴더입니다. 빈 폴더를 권장합니다.";
    #endregion

    #region 세부 로깅 여부
    public bool PrivateLoggingIsChecked
    {
      get => GlobalObj.Property.Options.Getter<bool>(Enums.ValueName.PrivateLogging);
      set => GlobalObj.Property.Options.Setter(Enums.ValueName.PrivateLogging, value.ToString());
    }

    public string PrivateLoggingToolTip =>
      "로그에 더 자세한 사항을 기록합니다.\n" +
      "개인 정보가 포함 될 수 있으나, 모든 정보는 익명으로 저장됩니다.\n" +
      "(로그정보로 사용자를 특정할 수 없음)";
    #endregion

    #region 미디어 로드 엔진
    public int MediaLoadEngineSelectedItem
    {
      get => (int)GlobalObj.Property.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine);
      set
      {
        GlobalObj.Property.Options.Setter(Enums.ValueName.PrivateLogging, MediaLoadEngineTypes[value].Content.ToString());
        OnPropertyChanged("MediaLoadEngineSelectedItem");
      }
    }
    public List<ComboBoxItem> MediaLoadEngineTypes { get; }

    public string MediaLoadEngineToolTip =>
      $"기본값 : {GlobalObj.Property.DefaultValue.GetDefaultValue<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine)}\n" +
      $"클릭후 드래그로 선택해야 합니다\n\n" +

      $"미디어를 불러올 때 사용하는 엔진입니다.";
    #endregion

    #region 가사창
    public RelayCommand LyricsWindowActiveCommand { get; }
    private void LyricsWindowActiveChange(Enums.LyricsSettingsType value) => GlobalObj.Property.Options.Setter(Enums.ValueName.LyricsWindowActive, value.ToString());
    public bool IsCheckedLyricsWindowActive_Off => GlobalObj.Property.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsWindowActive) == Enums.LyricsSettingsType.Off;
    public bool IsCheckedLyricsWindowActive_Auto => GlobalObj.Property.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsWindowActive) == Enums.LyricsSettingsType.Auto;
    public bool IsCheckedLyricsWindowActive_On => GlobalObj.Property.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsWindowActive) == Enums.LyricsSettingsType.On;

    public string LyricsWindowActiveToolTip =>
      $"기본값 : {GlobalObj.Property.DefaultValue.GetDefaultValue<Enums.LyricsSettingsType>(Enums.ValueName.LyricsWindowActive)}\n" +
      $"On = 항상 열기, Auto = 가사가 있는 경우만 열기, Off = 항상 닫기\n\n" +

      $"가사를 볼 수 있는 창을 활성화 합니다.";
    #endregion
  }
}
