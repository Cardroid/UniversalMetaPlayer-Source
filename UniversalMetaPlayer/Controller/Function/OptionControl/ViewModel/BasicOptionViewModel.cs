using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

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
      GlobalProperty.PropertyChanged += (e) =>
      {
        if (e == "FileSavePath")
          OnPropertyChanged("FileSavePathToolTip");
        if (e == "PrivateLogging")
          OnPropertyChanged("PrivateLoggingIsChecked");
        if (e == "MediaLoadEngine")
          OnPropertyChanged("SelectedItem");
      };

      MediaLoadEngineTypes = new List<GlobalProperty.Options.Enums.MediaLoadEngineType>
      {
        GlobalProperty.Options.Enums.MediaLoadEngineType.Native,
        //GlobalProperty.Options.Enums.MediaLoadEngineType.YoutubeDL
      };
    }

    public string FileSavePathToolTip =>
      $"현재 설정 : \"{Path.GetFullPath(GlobalProperty.Options.FileSavePath)}\"\n\n" +

      $"프로그램에서 파일을 보관하는 폴더입니다. 빈 폴더를 권장합니다.";

    public bool PrivateLoggingIsChecked
    {
      get => GlobalProperty.Options.PrivateLogging;
      set => GlobalProperty.Options.PrivateLogging = value;
    }

    public string PrivateLoggingToolTip =>
      "로그에 더 자세한 사항을 기록합니다.\n" +
      "개인 정보가 포함 될 수 있으나, 모든 정보는 익명으로 저장됩니다.\n" +
      "(로그정보로 사용자를 특정할 수 없음)";

    public GlobalProperty.Options.Enums.MediaLoadEngineType SelectedItem
    {
      get => GlobalProperty.Options.MediaLoadEngine;
      set => GlobalProperty.Options.MediaLoadEngine = value;
    }
    public List<GlobalProperty.Options.Enums.MediaLoadEngineType> MediaLoadEngineTypes { get; }
    public string MediaLoadEngineToolTip =>
      $"기본값 : {GlobalProperty.Options.DefaultValue.DefaultMediaLoadEngineType}\n\n" +

      $"미디어를 불러올 때 사용하는 엔진입니다.";


    #region 가사창
    public bool IsCheckedLyricsWindowActiveToggleButton
    {
      get => _IsCheckedLyricsWindowActiveToggleButton;
      set
      {
        LyricsWindowClose();
        _IsCheckedLyricsWindowActiveToggleButton = value;
        if (_IsCheckedLyricsWindowActiveToggleButton)
          LyricsWindowOpen();
        OnPropertyChanged("IsCheckedLyricsWindowActiveToggleButton");
      }
    }
    private bool _IsCheckedLyricsWindowActiveToggleButton = false;

    private void LyricsWindowOpen()
    {
      LyricsWindow = new UserWindow(new LyricsControl(), "UMP - Lyrics") { WindowStartupLocation = WindowStartupLocation.CenterOwner };
      LyricsWindow.Show();
      LyricsWindow.Closed += (_, e) =>
      {
        LyricsWindowClose();
        _IsCheckedLyricsWindowActiveToggleButton = false;
        OnPropertyChanged("IsCheckedLyricsWindowActiveToggleButton");
      };
    }

    private void LyricsWindowClose()
    {
      if (LyricsWindow != null)
        LyricsWindow.Close();
      LyricsWindow = null;
    }

    private UserWindow LyricsWindow
    {
      get => _LyricsWindow.IsAlive ? (UserWindow)_LyricsWindow.Target : null;
      set => _LyricsWindow = new WeakReference(value);
    }
    private WeakReference _LyricsWindow = new WeakReference(null);

    public string LyricsWindowActiveToolTip =>
      "가사를 볼 수 있는 창을 활성화 합니다.";
    #endregion
  }
}
