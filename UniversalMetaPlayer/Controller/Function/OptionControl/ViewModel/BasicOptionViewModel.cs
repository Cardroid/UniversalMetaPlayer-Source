﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

using UMP.Controller.Function.Lyrics;
using UMP.Controller.WindowHelper;
using UMP.Core;
using UMP.Core.Model;

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
          Content = GlobalProperty.Options.Enums.MediaLoadEngineType.Native,
          ToolTip =
          "기본으로 사용하는 엔진입니다\n" +
          "오류 발생시 기본엔진 사용을 추천합니다\n" +
          "Local 미디어와 Youtube 미디어를 지원합니다"}},
        {new ComboBoxItem()
        {
          Content = GlobalProperty.Options.Enums.MediaLoadEngineType.YoutubeDL,
          ToolTip =
          "Youtube-dl 를 기반으로한 엔진입니다\n" +
          "지원사이트는 Youtube-dl 홈페이지에서 확인 가능합니다",
          IsEnabled = false
        }}
      };

      LyricsWindowActiveCommand = new RelayCommand((o) => LyricsWindowActiveChange((GlobalProperty.Options.Enums.LyricsSettingsType)o));
    }

    #region 저장 경로
    public string FileSavePathToolTip =>
      $"현재 설정 : \"{Path.GetFullPath(GlobalProperty.Options.FileSavePath)}\"\n\n" +

      $"프로그램에서 파일을 보관하는 폴더입니다. 빈 폴더를 권장합니다.";
    #endregion

    #region 세부 로깅 여부
    public bool PrivateLoggingIsChecked
    {
      get => GlobalProperty.Options.PrivateLogging;
      set => GlobalProperty.Options.PrivateLogging = value;
    }

    public string PrivateLoggingToolTip =>
      "로그에 더 자세한 사항을 기록합니다.\n" +
      "개인 정보가 포함 될 수 있으나, 모든 정보는 익명으로 저장됩니다.\n" +
      "(로그정보로 사용자를 특정할 수 없음)";
    #endregion

    #region 미디어 로드 엔진
    public int MediaLoadEngineSelectedItem
    {
      get => (int)GlobalProperty.Options.MediaLoadEngine;
      set
      {
        if (MediaLoadEngineTypes[value].IsEnabled)
          GlobalProperty.Options.MediaLoadEngine = (GlobalProperty.Options.Enums.MediaLoadEngineType)MediaLoadEngineTypes[value].Content;
        OnPropertyChanged("MediaLoadEngineSelectedItem");
      }
    }
    public List<ComboBoxItem> MediaLoadEngineTypes { get; }

    public string MediaLoadEngineToolTip =>
      $"기본값 : {GlobalProperty.Options.DefaultValue.MediaLoadEngine}\n" +
      $"클릭후 드래그로 선택해야 합니다\n\n" +

      $"미디어를 불러올 때 사용하는 엔진입니다.";
    #endregion

    #region 가사창
    public RelayCommand LyricsWindowActiveCommand { get; }
    private void LyricsWindowActiveChange(GlobalProperty.Options.Enums.LyricsSettingsType value) => GlobalProperty.Options.LyricsWindowActive = value;
    public bool IsCheckedLyricsWindowActive_Off => GlobalProperty.Options.LyricsWindowActive == GlobalProperty.Options.Enums.LyricsSettingsType.Off;
    public bool IsCheckedLyricsWindowActive_Auto => GlobalProperty.Options.LyricsWindowActive == GlobalProperty.Options.Enums.LyricsSettingsType.Auto;
    public bool IsCheckedLyricsWindowActive_On => GlobalProperty.Options.LyricsWindowActive == GlobalProperty.Options.Enums.LyricsSettingsType.On;

    public string LyricsWindowActiveToolTip =>
      $"기본값 : {GlobalProperty.Options.DefaultValue.LyricsWindowActive}\n" +
      $"On = 항상 열기, Auto = 가사가 있는 경우만 열기, Off = 항상 닫기\n\n" +

      $"가사를 볼 수 있는 창을 활성화 합니다.";
    #endregion
  }
}
