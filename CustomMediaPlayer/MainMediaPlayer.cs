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
        public static IWavePlayer mediaPlayer = new WaveOut(); //메인 플레이어

        #region 현재 열린 파일 관련
        public delegate void AudioFileEventHandler(MediaFullInfo audioFile);
        public static event AudioFileEventHandler AudioFileOpen;
        public delegate void NowPlayMediaCursorChangeHandler(int id);
        public static event NowPlayMediaCursorChangeHandler NowPlayMediaCursorChange;
        
        public static AudioFileReader NowPlayAudioStream { get; private set; }
        private static MediaFullInfo nowplaymedia;
        public static MediaFullInfo NowPlayMedia
        {
            get
            { return nowplaymedia; }
            set
            {
                mediaPlayer.Stop();
                NowPlayAudioStream = null;
                nowplaymedia = value;
                NowPlayAudioStream = new AudioFileReader(nowplaymedia.FileFullName);
                // 미디어 길이 표시 라벨 너비 초기화
                ((MainWindow)System.Windows.Application.Current.MainWindow).TotalTimeLabel.Width = (Utility.Utility.MeasureString(Utility.Utility.TimeSpanStringConverter(NowPlayMedia.Duration))).Width;
                mediaPlayer.Init(NowPlayAudioStream);
                if (MainWindow.Optioncore.MediaOpeningPlayOption)
                    mediaPlayer.Play();
                ((MainWindow)System.Windows.Application.Current.MainWindow).MediaPlayer_PlayStateChange();
                AudioFileOpen?.Invoke(nowplaymedia);
                NowPlayMediaCursorChange?.Invoke(nowplaymedia.ID);
            }
        }
        #endregion
    }
}
