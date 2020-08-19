using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

using TagLib.Matroska;

using UMP.Controller.Function.Lyrics;
using UMP.Controller.WindowHelper;
using UMP.Core.Model;
using UMP.Core.Player;

namespace UMP.Core
{
  public static class WindowManager
  {
    public static event PropertyChangedEventHandler PropertyChanged;
    private static void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(null, new PropertyChangedEventArgs(propertyName));

    static WindowManager()
    {
      MainMediaPlayer.PropertyChanged += MainMediaPlayer_PropertyChanged_LyricsWindow;
    }

    private static void MainMediaPlayer_PropertyChanged_LyricsWindow(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "MainPlayerInitialized" && GlobalProperty.Options.LyricsWindowActive == GlobalProperty.Options.Enums.LyricsSettingsType.Auto)
      {
        // 가사 창 모드가 Auto일 경우 가사 존재 유무에 따라 창이 열리고 닫힘
        if (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInformation.Tags[MediaInfoType.Lyrics]))
          IsLyricsWindowActive = true;
        else
          IsLyricsWindowActive = false;
      }
    }

    /// <summary>
    /// 가사창 활성화 여부
    /// </summary>
    public static bool IsLyricsWindowActive
    {
      get => _IsLyricsWindowActive;
      set
      {
        _IsLyricsWindowActive = value;
        if (_IsLyricsWindowActive)
          LyricsWindowOpen();
        else
          LyricsWindowClose();
      }
    }
    private static bool _IsLyricsWindowActive = false;

    private static void LyricsWindowOpen()
    {
      if (IsLyricsWindowActive)
      {
        LyricsWindow.Visibility = Visibility.Visible;
        LyricsWindow.Activate();
      }
      else
      {
        IsLyricsWindowActive = true;
        LyricsWindow = new UserWindow(new LyricsControl(), "UMP - Lyrics") { WindowStartupLocation = WindowStartupLocation.CenterOwner };
        LyricsWindow.Show();
        LyricsWindow.Closed += (_, e) => { IsLyricsWindowActive = false; };
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
