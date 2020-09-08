using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;

using UMP.Controller.Function.Lyrics;
using UMP.Controller.WindowHelper;
using UMP.Core.Global;
using UMP.Core.Model.Control;
using UMP.Core.Model.Media;
using UMP.Core.Player;
using UMP.Core.Player.Plugin.Control;

namespace UMP.Core
{
  public static class WindowManager
  {
    static WindowManager()
    {
      Controller = new WindowController();
      GlobalProperty.PropertyChanged += GlobalProperty_PropertyChanged_LyricsWindow;
    }

    public static void CloseAll()
    {
      Controller.CloseAll();
    }

    public static WindowController Controller { get; }

    #region 가사 창 설정
    public static bool IsActiveLyricsWindow
    {
      get => _IsActiveLyricsWindow;
      private set
      {
        if(_IsActiveLyricsWindow != value)
        {
          _IsActiveLyricsWindow = value;

          if (!_IsActiveLyricsWindow)
            MainMediaPlayer.PropertyChanged -= MainMediaPlayer_PropertyChanged_LyricsWindow;
          else
            MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged_LyricsWindow;
        }
      }
    }
    private static bool _IsActiveLyricsWindow = false;

    private static void GlobalProperty_PropertyChanged_LyricsWindow(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "LyricsSettings" || e.PropertyName == "Loaded")
      {
        var settingValue = GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings);
        if (settingValue == Enums.LyricsSettingsType.Off)
            IsActiveLyricsWindow = false;
        else
          IsActiveLyricsWindow = true;
        LyricsWindow_RunProcess(settingValue);
      }
    }

    private static void MainMediaPlayer_PropertyChanged_LyricsWindow(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "MainPlayerInitialized")
        LyricsWindow_RunProcess(GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings));
    }

    private static void LyricsWindow_RunProcess(Enums.LyricsSettingsType settingValue)
    {
      if (settingValue == Enums.LyricsSettingsType.Auto)
      {
        // 가사 창 모드가 Auto일 경우 가사 존재 유무에 따라 창이 열리고 닫힘
        if (MainMediaPlayer.MediaLoadedCheck && !string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInformation.Tags[MediaInfoType.Lyrics]))
          LyricsWindowOpen();
        else
          LyricsWindowClose();
      }
      else if (settingValue == Enums.LyricsSettingsType.On)
        LyricsWindowOpen();
      else if (settingValue == Enums.LyricsSettingsType.Off)
        LyricsWindowClose();
    }

    private static void LyricsWindowOpen() => Controller.Open(WindowKind.Lyrics);

    private static void LyricsWindowClose() => Controller.Close(WindowKind.Lyrics);
    #endregion
  }
}
