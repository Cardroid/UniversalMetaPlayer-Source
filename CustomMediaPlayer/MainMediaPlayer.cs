using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CustomMediaPlayer.Core;
using CustomMediaPlayer.Option;
using NAudio.Wave;

namespace CustomMediaPlayer
{
    public static class MainMediaPlayer
    {
        public static IWavePlayer mediaPlayer = new WaveOut(); //메인 플래이어

        #region 현재 열린 파일 관련
        public delegate void AudioFileEventHandler(FileInfo audioFile);
        public static event AudioFileEventHandler AudioFileOpen;
        public static AudioFileReader NowPlayStream { private set; get; }
        public static MediaInfo NowPlayMediaInfo;
        private static FileInfo nowplayfile;
        public static FileInfo NowPlayFile
        {
            get
            { return nowplayfile; }
            set
            {
                mediaPlayer.Stop();
                NowPlayStream = null;
                nowplayfile = value;
                NowPlayMediaInfo = new MediaInfo(nowplayfile);
                NowPlayStream = new AudioFileReader(nowplayfile.FullName);
                // 미디어 길이 표시 라벨 너비 초기화
                ((MainWindow)Application.Current.MainWindow).TotalTimeLabel.Width = (MainWindow.Utility.MeasureString(MainWindow.Utility.TimeSpanStringConverter(NowPlayMediaInfo.Duration))).Width;
                mediaPlayer.Init(NowPlayStream);
                ((MainWindow)Application.Current.MainWindow).MediaPlayer_PlayStateChange();
                AudioFileOpen.Invoke(nowplayfile);
            }
        }
        #endregion

    }
}
