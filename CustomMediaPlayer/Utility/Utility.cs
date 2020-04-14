﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using Microsoft.Win32;

namespace CustomMediaPlayer.Utility
{
    public class Utility
    {
        public static ImageSource LogoImage = new BitmapImage(new Uri(@"Resources\IconCustomMusicPlayer.png", UriKind.Relative));
        public static ImageSource LogoNoteImage = new BitmapImage(new Uri(@"Resources\IconnoteCustomMusicPlayer.png", UriKind.Relative));

        // / 관리자 권한 여부 확인
        public static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            if (null != identity)
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            return false;
        }

        /// <summary>
        /// 시간을 지동으로 문자열로 변환합니다,
        /// </summary>
        /// <param name="value">시간으로 바꿀 TimeSpan</param>
        /// <returns>문자열</returns>
        public static string TimeSpanStringConverter(TimeSpan value)
        {
            if (value == TimeSpan.Zero)
                return "00:00";
            if (value > TimeSpan.FromDays(1))
                return value.ToString(@"d\:hh\:mm\:ss");
            else if (value > TimeSpan.FromHours(1))
                return value.ToString(@"h\:mm\:ss");
            else if (value > TimeSpan.FromSeconds(1))
                return value.ToString(@"mm\:ss");
            return "00:00";
        }
        
        /// <summary>
        /// 매개변수 문자열의 그래픽 사이즈를 반환합니다
        /// </summary>
        /// <param name="text">문자열</param>
        /// <returns>Size</returns>
        public static Size MeasureString(string text)
        {
            var formattedText = new FormattedText(text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(((MainWindow)System.Windows.Application.Current.MainWindow).FontFamily, ((MainWindow)System.Windows.Application.Current.MainWindow).FontStyle, ((MainWindow)System.Windows.Application.Current.MainWindow).FontWeight, ((MainWindow)System.Windows.Application.Current.MainWindow).FontStretch),
                ((MainWindow)System.Windows.Application.Current.MainWindow).FontSize,
                Brushes.Black,
                new NumberSubstitution(),
                1);

            return new Size(formattedText.Width + 14, formattedText.Height + 14);
        }

        public string VolumeConverter(int value)
        {
            return value.ToString() +"%";
        }

        /// <summary>
        /// 파일 다이얼로그 열기
        /// </summary>
        /// <returns>선택된 파일의 Fullname, 없다면 null</returns>
        public string[] OpenDialog()
        {
            List<string> filelist = new List<string>();
            // 파일 열기, OpenFileDialog
            OpenFileDialog ofdlg = new OpenFileDialog();
            {
                // 기본 폴더 지정
                ofdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

                // 바로가기 위치 생성
                //string taskbarpath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Microsoft\Internet Explorer\Quick Launch\User Pinned\TaskBar";
                //DirectoryInfo di = new DirectoryInfo(taskbarpath);
                //if (di.Exists == true)
                //{
                //    //ofdlg.InitialDirectory = pathLab;   // 기본 폴더 지정
                //    FileDialogCustomPlace customPlaceLab = new FileDialogCustomPlace(taskbarpath);
                //    ofdlg.CustomPlaces.Add(customPlaceLab);
                //}

                #region 다이얼로그의 좌측 바로가기
                ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Contacts);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Cookies);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Desktop);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Documents);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Favorites);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.LocalApplicationData);
                ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Music);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Pictures);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFiles);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFilesCommon);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Programs);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.RoamingApplicationData);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.SendTo);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.StartMenu);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Startup);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.System);
                //ofdlg.CustomPlaces.Add(FileDialogCustomPlaces.Templates);
                #endregion

                ofdlg.Title = "미디어 파일 선택";
                ofdlg.Multiselect = true; // 1개 이상 선택 가능 설정
                ofdlg.CheckFileExists = true;   // 파일 존재 여부 확인
                ofdlg.CheckPathExists = true;   // 폴더 존재 여부 확인
                ofdlg.Filter = // 필터설정
                    "음악 파일 (*.mp3, *.flac, *.wav)|*.mp3;*.flac;*.wav" + "|" +
                    "모든 파일 (*.*)|*.*";

                // 파일 선택 완료 시 이벤트
                ofdlg.FileOk += (s, a) =>
                {
                    foreach (var file in ofdlg.FileNames)
                    {
                        filelist.Add(file);
                    }
                };
            }
            ofdlg.ShowDialog();

            if (filelist.Count == 0)
                return null;
            string[] filestring = new string[filelist.Count];
            for (int i = 0; i < filelist.Count; i++)
            {
                filestring[i] = filelist[i];
            }
            return filestring;
        }

        /// <summary>
        /// 파일이름으로 지원하는 미디어 파일인지 확인합니다.
        /// </summary>
        /// <param name="filename">파일이름</param>
        /// <returns>bool 타입</returns>
        public static bool IsMedia(string filename)
        {   // 옵션추가가능 
            // 강력한 파일 검사, 일반 파일검사
            string[] MediaExtensions = { ".mp3", ".wav", ".flac" }; // 추가가능
            return MediaExtensions.Contains(Path.GetExtension(filename), StringComparer.OrdinalIgnoreCase);
        }
    }
}
