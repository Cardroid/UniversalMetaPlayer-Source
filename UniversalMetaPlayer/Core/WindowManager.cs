using System;
using System.Collections.Generic;
using System.ComponentModel;
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
      MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged_LyricsWindow;
    }

    private static void MainMediaPlayer_PropertyChanged_LyricsWindow(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "MainPlayerInitialized" && GlobalProperty.Options.Getter<Enums.LyricsSettingsType>(Enums.ValueName.LyricsWindowActive) == Enums.LyricsSettingsType.Auto)
      {
        // 가사 창 모드가 Auto일 경우 가사 존재 유무에 따라 창이 열리고 닫힘
        if (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInformation.Tags[MediaInfoType.Lyrics]))
          LyricsWindowOpen();
        else
          LyricsWindowClose();
      }
    }

    public static void CloseAll()
    {
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
  }
}
