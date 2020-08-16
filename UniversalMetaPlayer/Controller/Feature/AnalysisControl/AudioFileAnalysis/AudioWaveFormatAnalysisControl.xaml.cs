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

using UMP.Core.Player;

namespace UMP.Controller.Feature.AnalysisControl.AudioFileAnalysis
{
  public partial class AudioWaveFormatAnalysisControl : FeatureModelControl
  {
    public AudioWaveFormatAnalysisControl() : base("분석 - 속성")
    {
      InitializeComponent();

      if (MainMediaPlayer.MediaLoadedCheck)
      {
        PropertyPanel.Children.Clear();
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"채널 : [{(MainMediaPlayer.WaveFormat.Channels == 2 ? "스테레오" : MainMediaPlayer.WaveFormat.Channels == 1 ? "모노" : "알 수 없음")}]"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"셈플당 비트 : [{MainMediaPlayer.WaveFormat.BitsPerSample}]",
          ToolTip = "[샘플당 비트 수] 입니다\n(일반적으로 16 or 32 때로는 24 or 8) 일부 코덱의 경우 0 일 수 있습니다"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"평균 byte/s : [{MainMediaPlayer.WaveFormat.AverageBytesPerSecond}]",
          ToolTip = "[초당 사용된 평균 바이트 수] 입니다"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"블록 정렬 : [{MainMediaPlayer.WaveFormat.BlockAlign}]"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"인코딩 : [{MainMediaPlayer.WaveFormat.Encoding}]",
          ToolTip = "[사용된 인코딩 유형] 입니다"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"압축 바이트 수 : [{MainMediaPlayer.WaveFormat.ExtraSize}]",
          ToolTip = "[웨이브에서 사용하는 추가 바이트 수] 입니다\nWAVENATEX 헤더 뒤에 추가 데이터를 저장하는 압축 형식을 제외하고 일반적으로 0 입니다"
        });
        PropertyPanel.Children.Add(new Label()
        {
          Content = $"샘플링 속도 (Sample/Second) : [{MainMediaPlayer.WaveFormat.SampleRate}]"
        });
      }
      else
      {
        PropertyPanel.Children.Clear();
        PropertyPanel.Children.Add(new Label()
        {
          Content = "미디어가 로드되지 않았습니다"
        });
      }

      this.Loaded += (_, e) =>
      {
        MainMediaPlayer.PropertyChangedEvent += (e) =>
          {
            if (e == "MainPlayerInitialized")
            {
              PropertyPanel.Children.Clear();
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"채널 : [{(MainMediaPlayer.WaveFormat.Channels == 2 ? "스테레오" : MainMediaPlayer.WaveFormat.Channels == 1 ? "모노" : "알 수 없음")}]"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"셈플당 비트 : [{MainMediaPlayer.WaveFormat.BitsPerSample}]",
                ToolTip = "[샘플당 비트 수] 입니다\n(일반적으로 16 or 32 때로는 24 or 8) 일부 코덱의 경우 0 일 수 있습니다"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"평균 byte/s : [{MainMediaPlayer.WaveFormat.AverageBytesPerSecond}]",
                ToolTip = "[초당 사용된 평균 바이트 수] 입니다"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"블록 정렬 : [{MainMediaPlayer.WaveFormat.BlockAlign}]"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"인코딩 : [{MainMediaPlayer.WaveFormat.Encoding}]",
                ToolTip = "[사용된 인코딩 유형] 입니다"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"압축 바이트 수 : [{MainMediaPlayer.WaveFormat.ExtraSize}]",
                ToolTip = "[웨이브에서 사용하는 추가 바이트 수] 입니다\nWAVENATEX 헤더 뒤에 추가 데이터를 저장하는 압축 형식을 제외하고 일반적으로 0 입니다"
              });
              PropertyPanel.Children.Add(new Label()
              {
                Content = $"샘플링 속도 (Sample/Second) : [{MainMediaPlayer.WaveFormat.SampleRate}]"
              });
            }
          };
      };
    }
  }
}
