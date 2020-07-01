using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CustomMediaPlayer.Core;

using Newtonsoft.Json;

namespace CustomMediaPlayer.Controllers.PlayList
{
  public class PlayListCore
  {
    private PlayListInfo playlist = new PlayListInfo();
    public PlayListInfo Playlist
    {
      get { return playlist; }
      set { playlist = value; }
    }
  }

  public class PlayListInfo : ObservableCollection<MediaInfo>, ICloneable
  {
    public string PlayListName { get; set; } = "No name";
    public TimeSpan TotalDuration { get; set; } = TimeSpan.Zero;
    public bool NowWorking { get; set; }
    public string[] Serialize()
    {
      string[] Properties = { PlayListName, TotalDuration.TotalMilliseconds.ToString() };
      return Properties;
    }
    public bool Deserialize(string[] Properties)
    {
      if (Properties.Length == 2)
      {
        try
        {
          PlayListName = Properties[0];
          TotalDuration = TimeSpan.FromMilliseconds(int.Parse(Properties[1]));
          return true;
        }
        catch { return false; }
      }
      return false;
    }

    public void Load(MediaInfo media)
    {
      base.Add(media);
    }
    public void Add(ref MediaInfo media)
    {
      media.ID = base.Count + 1;
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
        for (int i = index; i < base.Count; i++)
        {
          base[i].ID = i;
        }
      }
    }
    public new void Insert(int index, MediaInfo item)
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
    public void IDRefresh()
    {
      for (int i = 0; i < base.Count; i++)
      {
        base[i].ID = i + 1;
      }
    }
    public new void Move(int oldIndex, int newIndex)
    {
      base.Move(oldIndex, newIndex);
      int Index1;
      int Index2;

      if (oldIndex < newIndex)
      {
        Index1 = oldIndex;
        Index2 = newIndex;
      }
      else
      {
        Index1 = newIndex;
        Index2 = oldIndex;
      }
      for (int i = Index1; i < Index2; i++)
      {
        base[i].ID = i;
      }
    }
    public object Clone()
    {
      PlayListInfo cloneplaylist = new PlayListInfo();
      cloneplaylist.PlayListName = PlayListName;
      cloneplaylist.TotalDuration = TotalDuration;
      cloneplaylist.NowWorking = NowWorking;
      for (int i = 0; i < base.Count; i++)
      { cloneplaylist.Add(base[i]); }
      return cloneplaylist;
    }
  }
}
