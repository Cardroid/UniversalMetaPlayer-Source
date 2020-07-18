using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CMP2.Core.Model
{
  public class PlayList : ObservableCollection<MediaInfo>
  {
    public event CMP_PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    public string PlayListName { get; set; } = "Nameless";
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
    private Log Log { get; } = new Log(typeof(PlayList));

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
      if ((int)media.LoadedCheck < 2)
      {
        if (media.MediaType == MediaType.Local)
          media.TryLocalInfomationLoad();
        else if (media.MediaType == MediaType.Youtube)
          await media.TryYouTubeInfomationLoadAsync();
      }

      base.Add(media);
      TotalDuration += media.Duration;
    }
    public new void Remove(MediaInfo media)
    {
      if (base.Contains(media))
      {
        base.Remove(media);
        TotalDuration -= media.Duration;
      }
    }
    public new void RemoveAt(int index)
    {
      if (base.Count >= index && index >= 0)
      {
        index--;
        TotalDuration -= base[index].Duration;
        base.RemoveAt(index);
      }
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
