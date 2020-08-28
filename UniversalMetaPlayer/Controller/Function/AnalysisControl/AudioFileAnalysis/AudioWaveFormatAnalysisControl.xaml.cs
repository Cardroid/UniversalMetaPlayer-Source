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

using UMP.Core;
using UMP.Core.Model.Func;
using UMP.Core.Player;
using UMP.Utility.MediaInfoLib;

namespace UMP.Controller.Function.AnalysisControl.AudioFileAnalysis
{
  public partial class AudioWaveFormatAnalysisControl : FunctionControlForm
  {
    public AudioWaveFormatAnalysisControl() : base("분석 - 속성")
    {
      InitializeComponent();

      DisplayMediaInfo_Uselibrary();

      this.Loaded += (_, e) =>
      {
        MainMediaPlayer.PropertyChanged += (_, e) =>
          {
            if (e.PropertyName == "MainPlayerInitialized")
              DisplayMediaInfo_Uselibrary();
          };
      };
    }

    private void DisplayMediaInfo_Uselibrary()
    {
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        MediaInfo MI = new MediaInfo();
        var mediaInfo = MainMediaPlayer.MediaInformation;

        PropertyPanel.Children.Clear();
        var textBlock = new TextBlock();
        var scrollHelper = new ScrollViewer();
        scrollHelper.Content = textBlock;
        PropertyPanel.Children.Add(scrollHelper);

        string toDisplay = string.Empty;

        toDisplay += "\r\n\r\nOpen\r\n";
        try
        {
          MI.Open(mediaInfo.MediaStreamPath);
        }
        catch (Exception e)
        {
          new Log($"{typeof(AudioWaveFormatAnalysisControl)}.DisplayMediaInfo_Uselibrary").Error("", e, $"LoadState : [{mediaInfo.LoadState}]\nMediaLocation : [{mediaInfo.MediaLocation}]");
          toDisplay += e.ToString();
          MI.Close();
          return;
        }
        finally
        {
          textBlock.Text = toDisplay;
        }

        toDisplay += "\r\n\r\nInform with Complete=false\r\n";
        MI.Option("Complete");
        toDisplay += MI.Inform();

        toDisplay += "\r\n\r\nInform with Complete=true\r\n";
        MI.Option("Complete", "1");
        toDisplay += MI.Inform();

        toDisplay += "\r\n\r\nCustom Inform\r\n";
        MI.Option("Inform", "General;File size is %FileSize% bytes");
        toDisplay += MI.Inform();

        toDisplay += "\r\n\r\nGet with Stream=General and Parameter='FileSize'\r\n";
        toDisplay += MI.Get(0, 0, "FileSize");

        toDisplay += "\r\n\r\nGet with Stream=General and Parameter=46\r\n";
        toDisplay += MI.Get(0, 0, 46);

        toDisplay += "\r\n\r\nCount_Get with StreamKind=Stream_Audio\r\n";
        toDisplay += MI.Count_Get(StreamKind.Audio);

        toDisplay += "\r\n\r\nGet with Stream=General and Parameter='AudioCount'\r\n";
        toDisplay += MI.Get(StreamKind.General, 0, "AudioCount");

        toDisplay += "\r\n\r\nGet with Stream=Audio and Parameter='StreamCount'\r\n";
        toDisplay += MI.Get(StreamKind.Audio, 0, "StreamCount");

        toDisplay += "\r\n\r\nClose\r\n";
        textBlock.Text = toDisplay;
        MI.Close();
      }
      else
      {
        PropertyPanel.Children.Clear();
        PropertyPanel.Children.Add(new Label()
        {
          Content = "미디어가 로드되지 않았습니다"
        });
      }
    }

    private void DisplayMediaInfo()
    {
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
          Content = $"샘플링 레이트 : [{MainMediaPlayer.WaveFormat.SampleRate}]",
          ToolTip = "샘플링 속도 (Sample/Second)"
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
    }
  }
}
