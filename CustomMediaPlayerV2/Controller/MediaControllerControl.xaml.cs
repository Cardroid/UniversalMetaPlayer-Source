using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CMP2.Core;

namespace CMP2.Controller
{
  public partial class MediaControllerControl : UserControl
  {
    private MediaControllerControlViewModel ViewModel { get; set; }

    public MediaControllerControl()
    {
      this.Loaded += (s, e) =>
      {
<<<<<<< HEAD
<<<<<<< HEAD
<<<<<<< HEAD
=======
        InitializeComponent();
        ViewModel = (MediaControllerControlViewModel)DataContext;

>>>>>>> parent of 4baef91... Add MainPlayer Option Changed Event & Fixed App namespace
=======
        InitializeComponent();
        ViewModel = (MediaControllerControlViewModel)this.DataContext;

>>>>>>> parent of 708ea27... Add Uno Project
=======
        InitializeComponent();
        ViewModel = (MediaControllerControlViewModel)this.DataContext;

>>>>>>> parent of 708ea27... Add Uno Project
        this.ProgressSlider.ValueChanged += ProgressSlider_ValueChanged;

        // 마우스 휠을 사용한 볼륨조절
        this.MouseWheel += (s, e) =>
        {
          if (e.Delta > 0)
          {
            if (Math.Abs(e.Delta) > 120)
              ViewModel.Volume += 4;
            ViewModel.Volume += 1;
          }
          else if (e.Delta < 0)
          {
            if (Math.Abs(e.Delta) > 120)
              ViewModel.Volume -= 4;
            ViewModel.Volume -= 1;
          }
        };

        // 볼륨 뮤트 버튼
        VolumeMuteButton.Click += (s, e) =>
        {
          if (ViewModel.Volume > 0)
            ViewModel.Volume = 0;
          else
            ViewModel.Volume = ViewModel.BeforeVolume;
        };
      };
    }

    private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
      if (ProgressSlider.IsMouseOver && Mouse.LeftButton == MouseButtonState.Pressed)
      {
        // 진행 슬라이더를 클릭 또는 드래그 하여 미디어 재생 위치 변경
        MainMediaPlayer.AudioFile.CurrentTime = TimeSpan.FromMilliseconds(e.NewValue);
      }
    }
  }
}
