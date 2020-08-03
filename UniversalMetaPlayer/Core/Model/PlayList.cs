using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using UMP.Utility;

using PlaylistsNET.Content;
using PlaylistsNET.Models;
using UMP.Core.Function;

namespace UMP.Core.Model
{
  public class PlayList : ObservableCollection<MediaInformation>
  {
    private Log Log { get; }
    public event UMP_PropertyChangedEventHandler PropertyChangedEvent;
    private void OnPropertyChanged(string name) => PropertyChangedEvent?.Invoke(name);

    /// <summary>
    /// 플레이 리스트 구별을 위한 고유 값
    /// </summary>
    public string EigenValue { get; private set; }

    /// <summary>
    /// 플레이 리스트 이름
    /// </summary>
    public string PlayListName
    {
      get => _PlayListName;
      set
      {
        _PlayListName = value;
        OnPropertyChanged("PlayListName");
      }
    }
    private string _PlayListName;

    /// <summary>
    /// 플레이 리스트의 총 길이
    /// </summary>
    public TimeSpan TotalDuration
    {
      get => _TotalDuration;
      private set
      {
        _TotalDuration = value;
        OnPropertyChanged("TotalDuration");
      }
    }
    public TimeSpan _TotalDuration = TimeSpan.Zero;

    public PlayList(string name = "Nameless")
    {
      EigenValue = RandomFunc.RandomString();
      PlayListName = name;
      Log = new Log($"{typeof(PlayList)} - ({EigenValue})");
    }

    #region Save & Load
    /// <summary>
    /// 플레이리스트 정보 직렬화
    /// </summary>
    /// <returns>직렬화된 플레이리스트 정보</returns>
    public async Task Save(string path = "")
    {
      M3uPlaylist playlist = new M3uPlaylist { IsExtended = true };

      for (int i = 0; i < base.Count; i++)
      {
        var entry = new M3uPlaylistEntry()
        {
          Album = base[i].Tags[MediaInfoType.AlbumTitle],
          AlbumArtist = base[i].Tags[MediaInfoType.AlbumArtist],
          Duration = base[i].Duration,
          Path = base[i].MediaLocation,
          Title = base[i].Title
        };

        playlist.PlaylistEntries.Add(entry);
      }
      playlist.FileName = PlayListName;

      string m3uData = PlaylistToTextHelper.ToText(playlist);

      var savepath = Path.Combine(!string.IsNullOrWhiteSpace(path) ? path : Path.Combine(GlobalProperty.FileSavePath, "PlayList"));
      Checker.DirectoryCheck(savepath);
      playlist.Path = savepath;

      await File.WriteAllTextAsync(Path.Combine(savepath, $"{PlayListName}.m3u8"), m3uData, Encoding.UTF8);

      Log.Info("플레이 리스트 저장 완료");
    }

    /// <summary>
    /// 플레이리스트 정보 역직렬화 시도
    /// </summary>
    /// <param name="Properties">처리할 플레이리스트 정보</param>
    /// <returns>성공 여부</returns>
    public async Task<bool> Load(string path, bool newPlaylist = true)
    {
      if (File.Exists(path))
      {
        var parser = PlaylistParserFactory.GetPlaylistParser(".m3u8");
        Stream playListStream = null;
        IBasePlaylist playListData = null;
        try
        {
          playListStream = new FileStream(path, FileMode.Open, FileAccess.Read);
          playListData = parser.GetFromStream(playListStream);
          playListData.FileName = Path.GetFileNameWithoutExtension(path);
        }
        catch (Exception e)
        {
          Log.Fatal("플레이 리스트 로드 중 오류 발생. (Parsing Error)", e, $"Path : [{path}]");
          return false;
        }
        finally
        {
          if (playListStream != null)
            playListStream.Close();
        }

        if (playListData == null)
        {
          Log.Fatal("플레이 리스트 로드 중 오류 발생. (Data is Null)", $"Path : [{path}]");
          return false;
        }

        if (newPlaylist)
          PlayListName = playListData.FileName ?? "Nameless";

        List<string> paths = playListData.GetTracksPaths();
        if (paths.Count < 0)
        {
          Log.Fatal("플레이 리스트 로드 중 오류 발생", new Exception("(PlayList Count < 0) is Impossible"), $"PlayList Name : [{playListData.FileName}]\nPath : [{path}]");
          return false;
        }

        for (int i = 0; i < paths.Count; i++)
        {
          var media = new MediaLoader(paths[i]);
          var info = await media.GetInformationAsync(false);
          base.Add(info);
          TotalDuration += info.Duration;
          if (!info.LoadState)
            Log.Warn($"플레이 리스트 로드 경고 IsLoaded : [{info.LoadState}]", $"Title : [{info.Title}]\nPath : [{paths[i]}]");
        }

        if (newPlaylist)
          EigenValue = RandomFunc.RandomString();

        Log.Info($"플레이 리스트 로드 완료 MediaCount : [{paths.Count}]", $"Path : [{path}]");
        return true;
      }
      else
      {
        Log.Fatal("플레이 리스트 로드 중 오류 발생", new FileNotFoundException("File Not Found"), $"Path : [{path}]");
        return false;
      }
    }
    #endregion

    public new async Task Add(MediaInformation Information)
    {
      if (!Information.LoadState)
        Information = await new MediaLoader(Information.MediaLocation).GetInformationAsync(false);
      this.TotalDuration += Information.Duration;
      base.Add(Information);
      Log.Info($"플레이 리스트 항목 추가 완료 IsLoaded : [{Information.LoadState}]", $"Title : [{Information.Title}]\nFileName : {Path.GetFileName(Information.MediaLocation)}");
    }

    /// <summary>
    /// 리스트 추가
    /// </summary>
    /// <param name="mediaPath">추가할 미디어의 위치</param>
    public async Task Add(string mediaPath)
    {
      var loader = new MediaLoader(mediaPath);
      Log.Debug("플레이 리스트 항목 추가 시도", $"Path : [{mediaPath}]");

      var info = await loader.GetInformationAsync(false);
      TotalDuration += info.Duration;
      base.Add(info);
      Log.Info($"플레이 리스트 항목 추가 완료 IsLoaded : [{info.LoadState}]", $"Title : [{info.Title}]\nFileName : {Path.GetFileName(mediaPath)}");
    }

    /// <summary>
    /// 리스트에서 <seealso cref="MediaInformation"/>를 제거합니다.
    /// </summary>
    /// <param name="media">제거할 미디어 정보</param>
    public new void Remove(MediaInformation mediaInfo)
    {
      Log.Debug("플레이 리스트 항목 제거 시도", $"Title : [{mediaInfo.Title}]");
      if (base.Contains(mediaInfo))
      {
        base.Remove(mediaInfo);
        TotalDuration -= mediaInfo.Duration;
        Log.Info("플레이 리스트 항목 제거 완료", $"Title : [{mediaInfo.Title}]");
      }
      else
        Log.Fatal("플레이 리스트 항목 제거 실패", new NullReferenceException("Unlisted Media"), $"Title : [{mediaInfo.Title}]");
    }

    /// <summary>
    /// Index의 미디어 정보를 리스트에서 제거합니다.
    /// </summary>
    /// <param name="index">제거할 미디어 정보 Index</param>
    public new void RemoveAt(int index)
    {
      Log.Debug($"플레이 리스트 Index 항목 제거 시도.\nIndex : [{index}]");
      if (base.Count > index && index >= 0)
      {
        index--;
        base.RemoveAt(index);
        TotalDuration -= base[index].Duration;
        Log.Info($"플레이 리스트 Index 항목 제거 완료.\nIndex : [{index}]");
      }
      else
        Log.Fatal($"플레이 리스트 Index 항목 제거 실패", new IndexOutOfRangeException($"Index Out Of Range.\nBase Count : [{base.Count}]\nIndex : [{index}]"));
    }

    public new void Insert(int index, MediaInformation item)
    {
      if (base.Count > index && index >= 0)
      {
        base.Insert(index, item);
        TotalDuration += item.Duration;
      }
    }

    public async Task ReloadAtAsync(int index)
    {
      if (base.Count > index && index >= 0)
      {
        var item = base[index];
        TotalDuration -= item.Duration;
        item = await new MediaLoader(item.MediaLocation).GetInformationAsync(false);
        TotalDuration += item.Duration;
        base[index] = item;
        Log.Info($"플레이 리스트 리로드 완료 Index : [{index}] IsLoaded : [{item.LoadState}]", $"Title : [{item.Title}]\nLocation : [{item.MediaLocation}]");
      }
    }

    public async Task ReloadAsync(MediaInformation item)
    {
      int index = base.IndexOf(item);
      if (index >= 0)
        await ReloadAtAsync(index);
    }

    public async Task ReloadAllAsync()
    {
      for (int i = 0; i < base.Count; i++)
      {
        var item = base[i];
        TotalDuration -= item.Duration;
        item = await new MediaLoader(item.MediaLocation).GetInformationAsync(false);
        TotalDuration += item.Duration;
        base[i] = item;
        Log.Info("플레이 리스트 전체 리로드 완료");
      }
    }

    public new void Clear()
    {
      this.TotalDuration = TimeSpan.Zero;
      base.Clear();
    }
  }
}
