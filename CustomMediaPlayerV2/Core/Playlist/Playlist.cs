using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace CMP2.Core.Playlist
{
  public class PlayList : ObservableCollection<IMediaInfo>
  {
    public string PlayListName { get; set; } = "Nameless";
    public TimeSpan TotalDuration { get; set; } = TimeSpan.Zero;

    public Task<string[,]> Serialize()
    {
      string[,] Properties = new string[Count + 1, 2];
      Properties[0,0] = PlayListName;
      Properties[0,1] = TotalDuration.TotalMilliseconds.ToString();
      for (int i = 0; i < Count; i++)
      {
        Properties[i + 1, 0] = base[i].Title;
        Properties[i + 1, 1] = base[i].FileFullName;
      }
      return Task.FromResult(Properties);
    }

    private const string MEDIA_INFO_NULL = "(Null)";

    public Task<bool> Deserialize(string[,] Properties)
    {
      if (Properties.GetLength(0) > 0)
      {
        try
        {
          PlayListName = Properties[0, 0];
          for (int i = 1; i < Properties.Length; i += 2)
          {
            var media = new MediaInfo(Properties[i, 1]);
            if (media.LoadedCheck == LoadState.NotTryed)
              media.InfomationLoader();
            if (media.LoadedCheck != LoadState.Complete)
              if (!media.Title.StartsWith(MEDIA_INFO_NULL))
                media.Title = $"{MEDIA_INFO_NULL} {media.Title}";
            Add(media);
            IDRefresh();
          }
          return Task.FromResult(true);
        }
        catch { return Task.FromResult(false); }
      }
      return Task.FromResult(false);
    }

    public void IDRefresh()
    {
      if (base.Count > 0)
        for (int i = 0; i < base.Count; i++)
        {
          base[i].ID = i + 1;
        }
    }
    public new void Add(IMediaInfo media)
    {
      media.ID = base.Count + 1;
      TotalDuration += media.Duration;
      base.Add(media);
    }
    public new void Remove(IMediaInfo media)
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
        for (int i = index; i < base.Count; i++)
        {
          base[i].ID = i;
        }
      }
    }
    public new void Insert(int index, IMediaInfo item)
    {
      if (base.Count >= index && index >= 0)
      {
        base.Insert(index, item);
        TotalDuration += item.Duration;
        for (int i = index; i < base.Count; i++)
        {
          base[i].ID = i;
        }
      }
    }
    public new void Move(int oldIndex, int newIndex)
    {
      base.Move(oldIndex, newIndex);

      if (oldIndex < newIndex)
      {
        for (int i = oldIndex; i < newIndex; i++)
        {
          base[i].ID = i;
        }
      }
      else
      {
        for (int i = newIndex; i < oldIndex; i++)
        {
          base[i].ID = i;
        }
      }
    }
  }
}
