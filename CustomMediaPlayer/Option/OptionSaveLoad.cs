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
        private object optionPage => mainWindow.Optionwindow.HamburgerMenuControl.Content;

        public void Save()
        {
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);
            var version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            try
            {
                OptionValue optionValue = new OptionValue();

                optionValue.KeyHooking = ((KeyOptionPage)optionPage).ViewModel.KeyHookOption;
                optionValue.MediaOpeningPlay = ((BasicPage)optionPage).ViewModel.MediaOpeningPlayOption;
                optionValue.RepeatOption = mainWindow.viewModel.RepeatPlayOption;
                optionValue.Volume = mainWindow.viewModel.Volume;
                optionValue.AccentColor = ThemeManager.Accents.ToString();
                optionValue.BaseColor = ThemeManager.AppThemes.ToString();
                optionValue.BackgroundColor = mainWindow.viewModel.BackgroundBrush.ToString();
                optionValue.LastMediaSave = ((BasicPage)optionPage).ViewModel.LastSongSaveOption;
                optionValue.LastMediaPath = MainMediaPlayer.NowPlayFile.FullName ?? null;
                optionValue.LastMediaPostion = MainMediaPlayer.NowPlayStream.CurrentTime.TotalMilliseconds;
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
            ((KeyOptionPage)optionPage).ViewModel.KeyHookOption = optionValue.KeyHooking;
            ((BasicPage)optionPage).ViewModel.MediaOpeningPlayOption = optionValue.MediaOpeningPlay;
            mainWindow.viewModel.RepeatPlayOption = optionValue.RepeatOption;
            mainWindow.viewModel.Volume = optionValue.Volume;
            ((BasicPage)optionPage).ViewModel.LastSongSaveOption = optionValue.LastMediaSave;
            //}
            //catch { }

            // 테마 로드
            //try 
            //{ 
            ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(optionValue.AccentColor), ThemeManager.GetAppTheme(optionValue.BaseColor));
            mainWindow.viewModel.BackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(optionValue.BackgroundColor));
            //}
            //catch 
            //{
            //    ThemeManager.ChangeAppStyle(Application.Current, ThemeManager.GetAccent(DefaultValue.AccentColor), ThemeManager.GetAppTheme(DefaultValue.BaseColor));
            //    mainWindow.viewModel.BackgroundBrush = (Brush)(new BrushConverter().ConvertFromString(DefaultValue.BackgroundColor));
            //}

            // 마지막 미디어 로드

            //try
            //{
            if (optionValue.LastMediaSave && !(Environment.GetCommandLineArgs().Length > 1))
            {
                MainMediaPlayer.NowPlayFile = new FileInfo(optionValue.LastMediaPath);
                MainMediaPlayer.NowPlayStream.CurrentTime = TimeSpan.FromMilliseconds(optionValue.LastMediaPostion);
            }
            //}
            //catch { mainWindow.MainPopup.Child = new Popup.SaveMediaFileErrorPopupPage(); mainWindow.MainPopup.IsOpen = true; }
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
