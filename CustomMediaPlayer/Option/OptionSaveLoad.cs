using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using MahApps.Metro;
using Newtonsoft.Json.Linq;
using CustomMediaPlayer.Option.OptionPage;
using System.Windows.Media;
using CustomMediaPlayer.Controllers.PlayList;

namespace CustomMediaPlayer.Option
{
  public class OptionSaveLoad
  {
    public static string SaveFilePath { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + @"\Finitio\CustomMediaPlayer\"; } }
    public static string SaveFileName { get { return $"UserSet.json"; } }
    private MainWindow mainWindow => (MainWindow)System.Windows.Application.Current.MainWindow;

    public bool Save()
    {
      if (!Directory.Exists(SaveFilePath))
        Directory.CreateDirectory(SaveFilePath);
      try
      {
        OptionValue optionValue = new OptionValue
        {
          KeyHooking = MainWindow.Optioncore.KeyHookOption,
          MediaOpeningPlay = MainWindow.Optioncore.MediaOpeningPlayOption,
          DurationViewStatus = MainWindow.Optioncore.DurationViewStatus,
          RepeatOption = mainWindow.ViewModel.RepeatPlayOption,
          Volume = mainWindow.ViewModel.Volume,
          AccentColor = ThemeManager.DetectAppStyle().Item2.Name,
          BaseColor = ThemeManager.DetectAppStyle().Item1.Name,
          BackgroundColor = mainWindow.ViewModel.BackgroundBrush.ToString(),
          LastMediaSave = MainWindow.Optioncore.LastSongSaveOption
        };
        if (MainWindow.Optioncore.LastSongSaveOption && MainMediaPlayer.NowPlayAudioStream != null)
        {
          if (MainMediaPlayer.NowPlayMedia != null)
          {
            optionValue.LastMediaID = MainMediaPlayer.NowPlayMedia.ID.ToString();
            if (int.TryParse(optionValue.LastMediaID, out _) )
              optionValue.LastMediaPostion = MainMediaPlayer.NowPlayAudioStream.CurrentTime.TotalMilliseconds;
            else
              optionValue.LastMediaPostion = 0;
          }
        }

        var JsonObj = JsonConvert.SerializeObject(optionValue, Formatting.Indented);

        File.WriteAllText(SaveFilePath + SaveFileName, JsonObj);
      }
      catch (Exception e)
      {
        mainWindow.MainPopup.Child = new Popup.MainWindowPopupPage(Popup.PopupContents.LoadError, $"Playlist Load Error : {e.Message}");
        return false;
      }
      return true;
    }

    public void Load()
    {
      OptionValue DefaultValue = new OptionValue();
      OptionValue optionValue;
      if (File.Exists(SaveFilePath + SaveFileName))
      {
        try
        {
          string Jsonstring = File.ReadAllText(SaveFilePath + SaveFileName);
          optionValue = JsonConvert.DeserializeObject<OptionValue>(Jsonstring);
        }
        catch { optionValue = DefaultValue; }
      }
      else { optionValue = DefaultValue; }

      // 로드 오류시를 대비
      if (optionValue == null)
        optionValue = DefaultValue;

      // 옵션 로드
      MainWindow.Optioncore.KeyHookOption = optionValue.KeyHooking;
      MainWindow.Optioncore.MediaOpeningPlayOption = optionValue.MediaOpeningPlay;
      mainWindow.ViewModel.RepeatPlayOption = optionValue.RepeatOption;
      mainWindow.ViewModel.Volume = optionValue.Volume;
      MainWindow.Optioncore.LastSongSaveOption = optionValue.LastMediaSave;

      // 테마 로드
      try
      {
        ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent(optionValue.AccentColor), ThemeManager.GetAppTheme(optionValue.BaseColor));
        mainWindow.ViewModel.BackgroundBrush = new BrushConverter().ConvertFromString(optionValue.BackgroundColor) as Brush;
      }
      catch
      {
        ThemeManager.ChangeAppStyle(System.Windows.Application.Current, ThemeManager.GetAccent(DefaultValue.AccentColor), ThemeManager.GetAppTheme(DefaultValue.BaseColor));
        mainWindow.ViewModel.BackgroundBrush = new BrushConverter().ConvertFromString(DefaultValue.BackgroundColor) as Brush;
      }

      // 플레이리스트 로드
      try
      {
        MainWindow.PlayList.Playlist = PlayListLoad.Load("PlayList.json");
      }
      catch (Exception e)
      {
        mainWindow.MainPopup.Child = new Popup.MainWindowPopupPage(Popup.PopupContents.LoadError, $"Playlist Load Error : {e.Message}");
      }

      // 저장된 미디어 로드
      if (optionValue.LastMediaSave && (Environment.GetCommandLineArgs().Length < 2))
      {
        try
        {
          if (int.TryParse(optionValue.LastMediaID, out int index) && MainWindow.PlayList.Playlist.Count > 0)
          {
            MainMediaPlayer.Init(MainWindow.PlayList.Playlist[index - 1]);
            MainMediaPlayer.NowPlayAudioStream.CurrentTime = TimeSpan.FromMilliseconds(optionValue.LastMediaPostion);
          }
          else
          {
            mainWindow.MainPopup.Child = new Popup.MainWindowPopupPage(Popup.PopupContents.LoadError, $"LastMedia Load Error : 저장된 미디어 정보 오류");
          }
        }
        catch
        {
          // 저장된 미디어 정보 오류
          mainWindow.MainPopup.Child = new Popup.MainWindowPopupPage(Popup.PopupContents.LoadError, $"LastMedia Load Error : 저장된 미디어 정보 오류");
        }
      }
    }
  }

  public class OptionValue
  {
    public bool KeyHooking = false;
    public bool MediaOpeningPlay = true;
    public bool DurationViewStatus = false;
    public int RepeatOption = 0;
    public int Volume = 50;
    public string AccentColor = "Green";
    public string BaseColor = "BaseDark";
    public string BackgroundColor = @"#FF000000";
    public bool LastMediaSave = false;
    public string LastMediaID = "";
    public double LastMediaPostion = 0;
    public string CoreVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
    public string FileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
  }
}
