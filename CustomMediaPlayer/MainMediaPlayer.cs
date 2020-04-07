using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CustomMediaPlayer.Option;
using CustomMediaPlayer.Utility;
using NAudio.Wave;

namespace CustomMediaPlayer
{
    public class MainMediaPlayer
    {
        public static IWavePlayer mediaPlayer = new WaveOut(); //메인 플래이어

        #region 현재 열린 파일 관련
        public delegate void AudioFileEventHandler(FileInfo audioFile);
        public static event AudioFileEventHandler AudioFileOpen;
        public static AudioFileReader audioFileStream { private set; get; }
        public static NowPlayInfo nowPlayInfo;
        private static FileInfo _NowAudioFile;
        public static FileInfo NowAudioFile
        {
            get
            {
                return _NowAudioFile;
            }
            set
            {
                mediaPlayer.Stop();
                audioFileStream = null;
                _NowAudioFile = value;
                nowPlayInfo = new NowPlayInfo(_NowAudioFile);
                audioFileStream = new AudioFileReader(_NowAudioFile.FullName);
                // 미디어 길이 표시 라벨 너비 초기화
                ((MainWindow)Application.Current.MainWindow).TotalTimeLabel.Width = (MainWindow.Utility.MeasureString(MainWindow.Utility.TimeSpanStringConverter(nowPlayInfo.Duration))).Width;
                mediaPlayer.Init(audioFileStream);
                if (OptionSaveLoad.optionValue.MediaOpeningPlay.Value)
                    mediaPlayer.Play();
                ((MainWindow)Application.Current.MainWindow).MediaPlayer_PlayStateChange();
                AudioFileOpen.Invoke(_NowAudioFile);
            }
        }
        #endregion

    }
}
