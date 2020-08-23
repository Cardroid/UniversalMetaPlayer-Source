using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows;

using UMP.Controller.Function.Lyrics;
using UMP.Controller.WindowHelper;
using UMP.Core.Global;
using UMP.Core.Model.Media;
using UMP.Core.Player;

namespace UMP.Core
{
  public static class WindowManager
  {
    static WindowManager()
    {
      GlobalProperty.PropertyChanged += GlobalProperty_PropertyChanged_LyricsWindow;
    }

    public static void DeactivateAll()
    {
      LyricsWindowClose();
      MainMediaPlayer.PropertyChanged -= MainMediaPlayer_PropertyChanged_LyricsWindow;
      IsActiveLyricsWindow = false;
    }
    
    public static void CloseAll()
    {
      LyricsWindowClose();
    }

    #region 가사설정
    public static bool IsActiveLyricsWindow { get; private set; } = false;

    private static void GlobalProperty_PropertyChanged_LyricsWindow(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "LyricsSettings" || e.PropertyName == "Loaded")
      {
        var settingValue = GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsSettings);
        if (settingValue == Enums.LyricsSettingsType.Off)
        {
          if (IsActiveLyricsWindow)
          {
            MainMediaPlayer.PropertyChanged -= MainMediaPlayer_PropertyChanged_LyricsWindow;
            IsActiveLyricsWindow = false;
          }
        }
        else if (!IsActiveLyricsWindow)
        {
          MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged_LyricsWindow;
          IsActiveLyricsWindow = true;
        }
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

    private static void LyricsWindowOpen()
    {
      if (LyricsWindow != null)
      {
        LyricsWindow.Visibility = Visibility.Visible;
        LyricsWindow.Activate();
      }
      else
      {
        LyricsWindow = new UserWindow(new LyricsControl(), "UMP - Lyrics") { WindowStartupLocation = WindowStartupLocation.CenterOwner };
        LyricsWindow.Show();
        LyricsWindow.Closed += (_, e) => { LyricsWindow = null; };
      }
    }

    private static void LyricsWindowClose()
    {
      if (LyricsWindow != null)
        LyricsWindow.Close();
      LyricsWindow = null;
    }

    private static UserWindow LyricsWindow
    {
      get => _LyricsWindow.IsAlive ? (UserWindow)_LyricsWindow.Target : null;
      set => _LyricsWindow = new WeakReference(value);
    }
    private static WeakReference _LyricsWindow = new WeakReference(null);
    #endregion
  }
}
