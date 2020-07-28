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

namespace UMP.Core.Model
{
  public class PlayList : ObservableCollection<MediaInfomation>
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
      Log = new Log($"{typeof(PlayList)} - ({EigenValue})[{PlayListName}]");
    }
    
    #region Save & Load
    /// <summary>
    /// 플레이리스트 정보 직렬화
    /// </summary>
    /// <returns>직렬화된 플레이리스트 정보</returns>
    public async Task Save(string path = "")
    {
      M3uPlaylist playlist = new M3uPlaylist();
      playlist.IsExtended = true;

      for (int i = 0; i < base.Count; i++)
      {
        var entry = new M3uPlaylistEntry()
        {
          Album = base[i].AlbumTitle,
          AlbumArtist = base[i].AlbumArtist,
          Duration = base[i].Duration,
          Path = base[i].MediaLocation,
          Title = base[i].Title
        };

        entry.CustomProperties.Add("MediaType", base[i].MediaType.ToString());

        playlist.PlaylistEntries.Add(entry);
      }
      playlist.FileName = PlayListName;

      string m3uData = PlaylistToTextHelper.ToText(playlist);

      var savepath = Path.Combine(!string.IsNullOrWhiteSpace(path) ? path : Path.Combine(GlobalProperty.FileSavePath, "PlayList"));
      if (!Directory.Exists(savepath))
        Directory.CreateDirectory(savepath);
      playlist.Path = savepath;

      await File.WriteAllTextAsync(Path.Combine(savepath, $"{PlayListName}.m3u8"), m3uData, Encoding.UTF8);

      Log.Info("플레이 리스트 저장 성공");
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
          Log.Error("플레이 리스트 로드 중 오류 발생. (Parsing Error)", e, $"Path : [{path}]");
          return false;
        }
        finally
        {
          if (playListStream != null)
            playListStream.Close();
        }

        if (playListData == null)
        {
          Log.Error("플레이 리스트 로드 중 오류 발생. (Data is Null)", $"Path : [{path}]");
          return false;
        }

        if (newPlaylist)
          PlayListName = playListData.FileName ?? "Nameless";

        List<string> paths = playListData.GetTracksPaths();
        if (paths.Count < 0)
        {
          Log.Error("플레이 리스트 로드 중 오류 발생.", new NullReferenceException("(PlayList Count < 0) is Impossible."), $"PlayList Name : [{playListData}]\nPath : [{path}]");
          return false;
        }

        for (int i = 0; i < paths.Count; i++)
        {
          var mediatype = Checker.MediaTypeChecker(paths[i]);
          if (mediatype != MediaType.NotSupport)
            await Add(new Media(mediatype, paths[i]));
          else
            Log.Warn("지원하지 않는 타입의 미디어를 건너뛰었습니다.",$"Path : [{paths[i]}]");
        }

        if (newPlaylist)
          EigenValue = RandomFunc.RandomString();

        Log.Info("플레이 리스트 로드 성공.\nSuccessful loading PlayList from path", $"Path : [{path}]");
        return true;
      }
      else
      {
        Log.Error("플레이 리스트 로드 중 오류 발생.", new FileNotFoundException("File Not Found"), $"Path : [{path}]");
        return false;
      }
    }
    #endregion

    public new void Add(MediaInfomation infomation)
    {
      this.TotalDuration += infomation.Duration;
      base.Add(infomation);
      Log.Info($"[{infomation.Title}](을)를 플레이 리스트에 등록 성공.");
    }

    /// <summary>
    /// 리스트 추가
    /// </summary>
    /// <param name="media">추가할 미디어</param>
    public async Task Add(Media media)
    {
      Log.Debug("플레이 리스트 항목 추가 시도.", $"Title : [{media.GetInfomation().Title}]");
      if ((int)media.LoadedCheck < 2)
        await media.TryInfoPartialLoadAsync();

      var info = media.GetInfomation();
      base.Add(info);
      TotalDuration += info.Duration;
      if (media.LoadedCheck == LoadState.Fail)
        Log.Warn("플레이 리스트 항목 추가 오류 발생.", $"Title : [{info.Title}]");
      else
        Log.Info("플레이 리스트 항목 추가 성공.", $"Title : [{info.Title}]");
    }

    /// <summary>
    /// 리스트에서 <seealso cref="MediaInfomation"/>를 제거합니다.
    /// </summary>
    /// <param name="media">제거할 미디어 정보</param>
    public new void Remove(MediaInfomation mediaInfo)
    {
      Log.Debug("플레이 리스트 항목 제거 시도.", $"Title : [{mediaInfo.Title}]");
      if (base.Contains(mediaInfo))
      {
        base.Remove(mediaInfo);
        TotalDuration -= mediaInfo.Duration;
        Log.Info("플레이 리스트 항목 제거 성공.", $"Title : [{mediaInfo.Title}]");
      }
      else
        Log.Error("플레이 리스트 항목 제거 실패.", new NullReferenceException("Unlisted Media."), $"Title : [{mediaInfo.Title}]");
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
        Log.Info($"플레이 리스트 Index 항목 제거 성공.\nIndex : [{index}]");
      }
      else
        Log.Error($"플레이 리스트 Index 항목 제거 실패.", new IndexOutOfRangeException($"Index Out Of Range.\nBase Count : [{base.Count}]\nIndex : [{index}]"));
    }

    public new void Insert(int index, MediaInfomation item)
    {
      if (base.Count > index && index >= 0)
      {
        base.Insert(index, item);
        TotalDuration += item.Duration;
      }
    }

    public async Task ReloadAtAsync(int index)
    {
      if(base.Count > index && index >= 0)
      {
        var item = base[index];
        TotalDuration -= item.Duration;
        var media = new Media(item.MediaType, item.MediaLocation);
        await media.TryInfoPartialLoadAsync(false);
        item = media.GetInfomation();
        TotalDuration += item.Duration;
        base[index] = item;
        Log.Info("플레이 리스트 리로드 성공.",$"Title : [{item.Title}]\nLocation : [{item.MediaLocation}]");
      }
    }

    public async Task ReloadAsync(MediaInfomation item)
    {
      int index = base.IndexOf(item);
      if(index >= 0)
        await ReloadAtAsync(index);
    }

    public async Task ReloadAllAsync()
    {
      for(int i = 0; i < base.Count; i++)
      {
        var item = base[i];
        TotalDuration -= item.Duration;
        var media = new Media(item.MediaType, item.MediaLocation);
        await media.TryInfoPartialLoadAsync(false);
        item = media.GetInfomation();
        TotalDuration += item.Duration;
        base[i] = item;
        Log.Info($"플레이 리스트 전체 리로드 성공.");
      }
    }

    public new void Clear()
    {
      this.TotalDuration = TimeSpan.Zero;
      base.Clear();
    }
  }
}
