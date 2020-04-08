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

namespace CustomMediaPlayer.Option
{
    public class OptionSaveLoad
    {
        public static string SaveFilePath { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + @"\Finitio\CustomMediaPlayer\"; } }
        public static string SaveFileName { get { return $"UserSet.json"; } }
        private MainWindow mainWindow => (MainWindow)Application.Current.MainWindow;

        public void Save()
        {
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            try
            {
                OptionValue optionValue = new OptionValue();

                optionValue.KeyHooking = MainWindow.Optioncore.KeyHookOption;
                optionValue.MediaOpeningPlay = MainWindow.Optioncore.MediaOpeningPlayOption;
                optionValue.DurationViewStatus = MainWindow.Optioncore.DurationViewStatus;
                optionValue.RepeatOption = mainWindow.ViewModel.RepeatPlayOption;
                optionValue.Volume = mainWindow.ViewModel.Volume;
                optionValue.AccentColor = ThemeManager.DetectAppStyle().Item2.Name;
                optionValue.BaseColor = ThemeManager.DetectAppStyle().Item1.Name;
                optionValue.BackgroundColor = mainWindow.ViewModel.BackgroundBrush.ToString();
                optionValue.LastMediaSave = MainWindow.Optioncore.LastSongSaveOption;
                if (MainMediaPlayer.NowPlayStream != null && MainWindow.Optioncore.LastSongSaveOption)
                {
                    optionValue.LastMediaPath = MainMediaPlayer.NowPlayFile.FullName ?? null;
                    optionValue.LastMediaPostion = MainMediaPlayer.NowPlayStream.CurrentTime.TotalMilliseconds;
                }
                optionValue.Version = version;

                var JsonObj = JsonConvert.SerializeObject(optionValue, Formatting.Indented);

                File.WriteAllText(SaveFilePath + SaveFileName, JsonObj);
            }
            catch { }
        }
        public void Load()
        {
            OptionValue optionValue;
            OptionValue DefaultValue = new OptionValue();
            try
            {
                string Jsonstring = File.ReadAllText(SaveFilePath + SaveFileName);

                optionValue = JsonConvert.DeserializeObject<OptionValue>(Jsonstring);
            }
            catch { optionValue = DefaultValue; }

            // 옵션 로드
            //try
            //{
            MainWindow.Optioncore.KeyHookOption = optionValue.KeyHooking;
            MainWindow.Optioncore.MediaOpeningPlayOption = optionValue.MediaOpeningPlay;
            mainWindow.ViewModel.RepeatPlayOption = optionValue.RepeatOption;
            mainWindow.ViewModel.Volume = optionValue.Volume;
            MainWindow.Optioncore.LastSongSaveOption = optionValue.LastMediaSave;
            //}
            //catch { }

            // 테마 로드
            try
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(optionValue.AccentColor), ThemeManager.GetAppTheme(optionValue.BaseColor));
                mainWindow.ViewModel.BackgroundBrush = new BrushConverter().ConvertFromString(optionValue.BackgroundColor) as Brush;
            }
            catch
            {
                ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(DefaultValue.AccentColor), ThemeManager.GetAppTheme(DefaultValue.BaseColor));
                mainWindow.ViewModel.BackgroundBrush = new BrushConverter().ConvertFromString(DefaultValue.BackgroundColor) as Brush;
            }

            // 마지막 미디어 로드

            try
            {
                if (optionValue.LastMediaSave && !(Environment.GetCommandLineArgs().Length > 1))
                {
                    MainMediaPlayer.NowPlayFile = new FileInfo(optionValue.LastMediaPath);
                    MainMediaPlayer.NowPlayStream.CurrentTime = TimeSpan.FromMilliseconds(optionValue.LastMediaPostion);
                }
            }
            catch { mainWindow.MainPopup.Child = new Popup.SaveMediaFileErrorPopupPage(); mainWindow.MainPopup.IsOpen = true; }
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
        public string LastMediaPath = null;
        public double LastMediaPostion = 0;
        public string Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
    }
}
