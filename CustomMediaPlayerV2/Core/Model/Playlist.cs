using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

using CMP2.Utility;

namespace CMP2.Core.Model
{
  public class PlayList : ObservableCollection<MediaInfo>
  {
    private Log Log { get; }
    public event CMP_PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public string PlayListName { get; set; }
    public string EigenValue { get; }
    public TimeSpan _TotalDuration = TimeSpan.Zero;
    public TimeSpan TotalDuration
    {
      get => _TotalDuration;
      private set
      {
        _TotalDuration = value;
        OnPropertyChanged("TotalDuration");
      }
    }

    public PlayList(string name = "Nameless")
    {
      EigenValue = RandomFunc.RandomString();
      PlayListName = name;
      Log = new Log($"{typeof(PlayList)} - ({EigenValue})[{PlayListName}]");
    }

    /// <summary>
    /// 플레이리스트 정보 직렬화
    /// </summary>
    /// <returns>직렬화된 플레이리스트 정보</returns>
    public Task<string[,]> Serialize()
    {
      string[,] Properties = new string[Count + 1, 3];
      Properties[0, 0] = PlayListName;
      Properties[0, 1] = TotalDuration.TotalMilliseconds.ToString();
      for (int i = 0; i < Count; i++)
      {
        Properties[i + 1, 0] = base[i].Title;
        Properties[i + 1, 1] = base[i].MediaType.ToString();
        Properties[i + 1, 2] = base[i].MediaLocation;
      }
      Log.Debug("직렬화 성공");
      return Task.FromResult(Properties);
    }

    /// <summary>
    /// 플레이리스트 정보 역직렬화 시도
    /// </summary>
    /// <param name="Properties">처리할 플레이리스트 정보</param>
    /// <returns>성공 여부</returns>
    public async Task<bool> Deserialize(string[,] Properties)
    {
      if (Properties.GetLength(0) > 0)
      {
        try
        {
          PlayListName = Properties[0, 0];
          for (int i = 1; i < Properties.Length; i += 2)
          {
            if (Enum.TryParse(Properties[i, 1], out MediaType mediaType))
            {
              var media = new MediaInfo(mediaType, Properties[i, 1]);
              await Add(media);
            }
          }
          Log.Debug("역직렬화 성공");
          return true;
        }
        catch (Exception e)
        {
          Log.Error("역직렬화 실패", e);
          return false;
        }
      }
      else
      {
        Log.Error("역직렬화 실패 (Properties.Length > 0)");
        return false;
      }
    }

    public new async Task Add(MediaInfo media)
    {
      Log.Debug($"[{media.Title}](을)를 미디어 리스트에 등록 시도.");
      if ((int)media.LoadedCheck < 2)
      {
        if (media.MediaType == MediaType.Local)
          media.TryLocalInfomationLoad();
        else if (media.MediaType == MediaType.Youtube)
          await media.TryYouTubeInfomationLoadAsync();
      }

      base.Add(media);
      TotalDuration += media.Duration;
      Log.Info($"[{media.Title}](을)를 미디어 리스트에 등록 성공.");
    }

    public new void Remove(MediaInfo media)
    {
      Log.Debug($"[{media.Title}](을)를 미디어 리스트에서 제거 시도.");
      if (base.Contains(media))
      {
        base.Remove(media);
        TotalDuration -= media.Duration;
        Log.Info($"[{media.Title}](을)를 미디어 리스트에서 제거 성공.");
      }
      else
        Log.Error($"[{media.Title}](을)를 미디어 리스트에서 제거 실패.", new NullReferenceException($"Unlisted Media.\nTitle : [{media.Title}]"));
    }

    public new void RemoveAt(int index)
    {
      Log.Debug($"[{index}]번째 미디어를 미디어 리스트에서 제거 시도.");
      if (base.Count >= index && index >= 0)
      {
        index--;
        TotalDuration -= base[index].Duration;
        base.RemoveAt(index);
        Log.Info($"[{index}]번째 미디어를 미디어 리스트에서 제거 성공.");
      }
      else
        Log.Error($"[{index}]번째 미디어를 미디어 리스트에서 제거 실패.", new IndexOutOfRangeException($"Index Out Of Range.\nBase Count : [{base.Count}]\nIndex : [{index}]"));
    }

    public new void Insert(int index, MediaInfo item)
    {
      if (base.Count >= index && index >= 0)
      {
        base.Insert(index, item);
        TotalDuration += item.Duration;
      }
    }
  }
}
