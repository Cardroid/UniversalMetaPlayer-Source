using System;
using System.Collections.Generic;
using System.Text;

using UMP.Core;
using UMP.Core.Model.ViewModel;
using UMP.Core.Player;
using UMP.Utils;
using UMP.Utils.MediaInfoLib;

namespace UMP.Controller.Function.AnalysisControl.MediaFileAnalysis
{
  public class MediaAnalysisControlViewModel : ViewModelBase
  {
    public MediaAnalysisControlViewModel()
    {
      if (IsUseMediaInfoLibrary)
        DisplayMediaInfo_Uselibrary();
      else
        DisplayMediaInfo();
      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "MainPlayerInitialized")
        {
          if (IsUseMediaInfoLibrary)
            DisplayMediaInfo_Uselibrary();
          else
            DisplayMediaInfo();
        }
      };
    }

    public bool IsUseMediaInfoLibrary
    {
      get => TempProperty.IsUseMediaInfoLibrary;
      set
      {
        TempProperty.IsUseMediaInfoLibrary = value;
        OnPropertyChanged("IsUseMediaInfoLibrary");

        if (TempProperty.IsUseMediaInfoLibrary)
          DisplayMediaInfo_Uselibrary();
        else
          DisplayMediaInfo();
      }
    }

    public string ResultText
    {
      get => _ResultText;
      private set
      {
        _ResultText = value;
        OnPropertyChanged("ResultText");
      }
    }
    private string _ResultText;

    private void DisplayMediaInfo()
    {
      string result = string.Empty;
      if (MainMediaPlayer.MediaLoadedCheck)
      {
        result += "Start\n\n";

        result += "Native Media Info Loader\n\n";

        if (MainMediaPlayer.WaveFormat.Channels == 1)
          result += $"채널 : [1] (모노)\n\n";
        else if (MainMediaPlayer.WaveFormat.Channels == 2)
          result += $"채널 : [2] (스테레오)\n\n";
        else
          result += $"채널 : [{MainMediaPlayer.WaveFormat.Channels}]\n\n";

        result += $"샘플링 비트 : [{MainMediaPlayer.WaveFormat.BitsPerSample}]\n";
        result += "(일반적으로 16 or 32 때로는 24 or 8) 일부 코덱의 경우 0 일 수 있습니다\n\n";

        result += $"평균 Bps(byte/s) : [{Parser.ToSI(MainMediaPlayer.WaveFormat.AverageBytesPerSecond)}Bps]\n";
        result += $"((1 채널 [{Parser.ToSI(MainMediaPlayer.WaveFormat.AverageBytesPerSecond / MainMediaPlayer.WaveFormat.Channels)}byte/s]) * (채널 개수 [{MainMediaPlayer.WaveFormat.Channels}]))\n\n";

        result += $"블록 정렬 : [{MainMediaPlayer.WaveFormat.BlockAlign}]\n\n";

        result += $"인코딩 타입 : [{MainMediaPlayer.WaveFormat.Encoding}]\n\n";

        result += $"압축 바이트 수 : [{MainMediaPlayer.WaveFormat.ExtraSize}]\n";
        result += "[웨이브에서 사용하는 추가 바이트 수] 입니다\nWAVENATEX 헤더 뒤에 추가 데이터를 저장하는 압축 형식을 제외하고 일반적으로 0 입니다\n\n";

        result += $"샘플링 레이트 : [{Parser.ToSI(MainMediaPlayer.WaveFormat.SampleRate)}Hz]\n\n";

        result += "End";
      }
      else
        result = "미디어가 로드되지 않았습니다";
      ResultText = result;
    }

    private void DisplayMediaInfo_Uselibrary()
    {
      string result = string.Empty;

      if (MainMediaPlayer.MediaLoadedCheck)
      {
        MediaFileInfo MI;
        var mediaInfo = MainMediaPlayer.MediaInformation;

        result += "Open\n\n";
        try
        {
          MI = new MediaFileInfo(mediaInfo.MediaStreamPath);
        }
        catch (Exception e)
        {
          new Log($"{typeof(MediaAnalysisControl)}.DisplayMediaInfo_Uselibrary").Error("", e, $"LoadState : [{mediaInfo.LoadState}]\nMediaLocation : [{mediaInfo.MediaLocation}]");
          result += e.ToString();
          ResultText = result;
          return;
        }

        result += $"\n\n{MI.General.Inform()}\n\n";

        //Dictionary<string, string> tmp;

        //result += "*** General ***\n\n";
        //tmp = GetPropertyString(MI.General);
        //foreach (var item in tmp.Keys)
        //  result += $"{item}   : {tmp[item]}\n";

        //result += "\n";

        //for (int n = 0; n < MI.General.AudioCount; n++)
        //{
        //  result += $"\n*** Audio #{n} ***\n\n";
        //  tmp = GetPropertyString(MI.Audio[n]);
        //  foreach (var item in tmp.Keys)
        //    result += $"{item}   : {tmp[item]}\n";
        //}

        //result += "\n";

        //for (int n = 0; n < MI.General.TextCount; n++)
        //{
        //  result += $"\n*** Text #{n} ***\n\n";
        //  tmp = GetPropertyString(MI.Text[n]);
        //  foreach (var item in tmp.Keys)
        //    result += $"{item}   : {tmp[item]}\n";
        //}

        //result += "\n";

        //for (int n = 0; n < MI.General.ImageCount; n++)
        //{
        //  result += $"\n*** Image #{n} ***\n\n";
        //  tmp = GetPropertyString(MI.Image[n]);
        //  foreach (var item in tmp.Keys)
        //    result += $"{item}   : {tmp[item]}\n";
        //}

        //result += "\n";

        //for (int n = 0; n < MI.General.OtherCount; n++)
        //{
        //  result += $"\n*** Other #{n} ***\n\n";
        //  tmp = GetPropertyString(MI.Other[n]);
        //  foreach (var item in tmp.Keys)
        //    result += $"{item}   : {tmp[item]}\n";
        //}

        //result += "\n";

        //for (int n = 0; n < MI.General.MenuCount; n++)
        //{
        //  result += $"\n*** Menu #{n} ***\n\n";
        //  tmp = GetPropertyString(MI.Menu[n]);
        //  foreach (var item in tmp.Keys)
        //    result += $"{item}   : {tmp[item]}\n";
        //}
      }
      else
        result = "미디어가 로드되지 않았습니다";
      ResultText = result;
    }

    //private Dictionary<string, string> GetPropertyString(object obj)
    //{
    //  var prop = obj.GetType().GetProperties();
    //  Dictionary<string, string> result = new Dictionary<string, string>();

    //  for (int i = 0; i < prop.Length; i++)
    //  {
    //    var value = prop[i].GetGetMethod().Invoke(obj, null);
    //    if (value != null)
    //    {
    //      if (prop[i].Name != "EncoderSettings")
    //        result[prop[i].Name] = value.ToString();
    //    }
    //  }
    //  return result;
    //}
  }
}
