using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CustomMediaPlayer.Option;
using MahApps.Metro.IconPacks;
using static CustomMediaPlayer.MainMediaPlayer;

namespace CustomMediaPlayer
{
    /// <summary>
    /// 메인 윈도우 뷰 모델
    /// </summary>
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void Notify(string propName)
        { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName)); }

        private MainWindow mainWindow => (MainWindow)Application.Current.MainWindow; // 메인윈도우 참조 (코드 가독성을 위함)


        // 전채 재생시간 (단일 파일)
        public bool DurationViewStatus =>  MainWindow.Optioncore.DurationViewStatus;
        private TimeSpan totaltime;
        public TimeSpan totalTime
        {
            get { return totaltime; }
            set { totaltime = value; Notify("TotalTimestring"); }
        }
        public string TotalTimestring
        {
            get
            {
                if (NowPlayStream == null) // 오류 방지용
                    return MainWindow.Utility.TimeSpanStringConverter(TimeSpan.Zero);
                if (DurationViewStatus)
                    return MainWindow.Utility.TimeSpanStringConverter(totalTime); // 전채 시간
                else
                    return "-" + MainWindow.Utility.TimeSpanStringConverter(totalTime - currentpostion); // 남은 시간
            }
        }

        // 현재 재생위치
        private TimeSpan currentpostion;
        public TimeSpan CurrentPostion
        {
            get { return currentpostion; }
            set { currentpostion = value; Notify("CurrentPostiondouble"); Notify("CurrentPostionstring"); }
        }
        public double CurrentPostiondouble
        {
            get
            {
                if (NowPlayStream == null) // 오류 방지용
                    return 0;
                return currentpostion.TotalMilliseconds;
            }
            set
            { NowPlayStream.CurrentTime = TimeSpan.FromMilliseconds(value); }
        }
        public string CurrentPostionstring
        {
            get
            {
                if (NowPlayStream == null) // 오류 방지용
                    return MainWindow.Utility.TimeSpanStringConverter(TimeSpan.Zero);
                return MainWindow.Utility.TimeSpanStringConverter(currentpostion);
            }
        }

        // 반복재생
        private int repeatplayoption;
        public int RepeatPlayOption
        {
            get { return repeatplayoption; }
            set
            {
                repeatplayoption = value;
                if (repeatplayoption < 0 || repeatplayoption > 3)
                    repeatplayoption = 0;
                // 반복 아이콘 설정
                var RepeatIcon = new PackIconControl() { Width = 20, Height = 20 };
                if (repeatplayoption == 0) // 반복 안함
                { RepeatIcon.Kind = PackIconMaterialKind.RepeatOff; }
                else if (repeatplayoption == 1) // 한 곡 반복
                { RepeatIcon.Kind = PackIconMaterialKind.RepeatOnce; }
                else if(repeatplayoption == 2) // 전채 반복
                { RepeatIcon.Kind = PackIconMaterialKind.Repeat; }
                mainWindow.RepeatButton.Content = RepeatIcon;
            }
        }

        // 볼륨
        private int volume;
        public int BeforeVolume { get; private set; } = 1;
        public string volumestring { get; private set; }
        public int Volume
        {
            get
            {  return volume; }
            set
            {
                if (value >= 0 && value <= 100) // 오류 방지용
                {
                    volume = value;
                    volumestring = volume.ToString() + "%";
                    if(volume != 0)
                        BeforeVolume = volume;
                    mediaPlayer.Volume = volume / 100f;
                    VolumeMuteButtonIcon = VolumeMuteButtonIconChanger();
                    Notify("Volume");
                    Notify("volumestring");
                }
            }
        }

        // 볼륨 뮤트 버튼 아이콘 관련
        private PackIconBase volumemutebuttonicon;
        public PackIconBase VolumeMuteButtonIcon
        {
            get { if (volumemutebuttonicon == null) volumemutebuttonicon = VolumeMuteButtonIconChanger(); return volumemutebuttonicon; }
            set { volumemutebuttonicon = value; Notify("VolumeMuteButtonIcon"); }
        }
        public PackIconBase VolumeMuteButtonIconChanger()
        {
            var Icon = new PackIconControl() { Width = 15, Height = 15 };
            if (volume > 75)
                Icon.Kind = PackIconMaterialKind.VolumeHigh;
            else if (volume > 35)
                Icon.Kind = PackIconMaterialKind.VolumeMedium;
            else if (volume > 0)
                Icon.Kind = PackIconMaterialKind.VolumeLow;
            else if (volume == 0)
                Icon.Kind = PackIconMaterialKind.VolumeOff;
            return Icon;
        }

        // 배경색 관련 이벤트와 프로퍼티 정의
        #region 배경색 관련
        public delegate void BackgroundColorChangedHandler(Brush brush);
        public event BackgroundColorChangedHandler BackgroundColorChanged;
        private Brush backgroundbrush;
        public Brush BackgroundBrush
        {
            get { return backgroundbrush; }
            set
            {
                backgroundbrush = value;
                if (BackgroundColorChanged != null)
                    BackgroundColorChanged.Invoke(backgroundbrush);
                Notify("BackgroundBrush");
            }
        }
        #endregion
    }
}
