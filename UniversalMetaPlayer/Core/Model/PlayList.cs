using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using UMP.Utils;

using PlaylistsNET.Content;
using PlaylistsNET.Models;
using UMP.Core.MediaLoader;
using UMP.Core.Player;
using UMP.Core.Model.Media;
using UMP.Core.Global;
using System.Diagnostics;

namespace UMP.Core.Model
{
  public class PlayList : ObservableCollection<MediaInformation>
  {
    public PlayList(string name = "")
    {
      PlayListName = name;
      Log = new Log($"{typeof(PlayList)} - ({EigenValue})");
      NeedSave = false;
    }

    private Log Log { get; }
    public event PropertyChangedEventHandler Field_PropertyChanged;
    private void OnPropertyChanged(string propertyName) => Field_PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    /// <summary>
    /// 플레이리스트 구별을 위한 고유 값
    /// </summary>
    public string EigenValue { get; private set; }

    /// <summary>
    /// 저장이 필요한지의 여부
    /// </summary>
    public bool NeedSave { get; private set; }

    /// <summary>
    /// 플레이리스트 이름
    /// </summary>
    public string PlayListName
    {
      get => _PlayListName;
      set
      {
        if (string.IsNullOrWhiteSpace(value))
          _PlayListName = "Nameless";
        else
        {
          _PlayListName = value;

          string invalid = $"{new string(Path.GetInvalidFileNameChars())}\"";
          for (int i = 0; i < invalid.Length; i++)
            _PlayListName = _PlayListName.Replace(invalid[i].ToString(), "");

          if (_PlayListName != value)
            GlobalMessageEvent.Invoke("파일 이름에 사용할 수 없는 문자를 자동으로 제거했습니다", true);
        }

        EigenValue = Converter.SHA256Hash(_PlayListName);
        NeedSave = true;
        OnPropertyChanged("PlayListName");
      }
    }
    private string _PlayListName;

    /// <summary>
    /// 플레이리스트의 총 길이
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
    private TimeSpan _TotalDuration = TimeSpan.Zero;

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

      var savepath = Path.Combine(!string.IsNullOrWhiteSpace(path)
        ? path
        : Path.Combine(GlobalProperty.Options.Getter<string>(Enums.ValueName.FileSavePath), "PlayList"));
      Checker.DirectoryCheck(savepath);
      playlist.Path = savepath;

      await File.WriteAllTextAsync(Path.Combine(savepath, $"{PlayListName}.m3u8"), m3uData, Encoding.UTF8);

      if (Directory.GetFiles(savepath, $"{PlayListName}.m3u8").Length > 0)
      {
        Log.Info("플레이리스트 저장 완료");
        GlobalMessageEvent.Invoke("플레이리스트 저장 완료", true);
        NeedSave = false;
      }
      else
      {
        Log.Info("플레이리스트 저장 실패");
        GlobalMessageEvent.Invoke("[Save Error] 플레이리스트 저장 실패", false);
        NeedSave = true;
      }
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
          Log.Fatal("플레이리스트 로드 중 오류 발생. (Parsing Error)", e, $"Path : [{path}]");
          GlobalMessageEvent.Invoke("플레이리스트 로드 실패! [로그를 확인해주세요]");
          return false;
        }
        finally
        {
          if (playListStream != null)
            playListStream.Close();
        }

        if (playListData == null)
        {
          Log.Fatal("플레이리스트 로드 중 오류 발생 (Data is Null)", $"Path : [{path}]");
          GlobalMessageEvent.Invoke("플레이리스트 로드 실패 [로그를 확인해주세요]");
          return false;
        }

        if (newPlaylist)
          PlayListName = playListData.FileName ?? "Nameless";

        List<string> paths = playListData.GetTracksPaths();
        if (paths.Count < 0)
        {
          Log.Fatal("플레이리스트 로드 중 오류 발생", new Exception("(PlayList Count < 0) is Impossible"), $"PlayList Name : [{playListData.FileName}]\nPath : [{path}]");
          GlobalMessageEvent.Invoke("플레이리스트 로드 실패! [로그를 확인해주세요]");
          return false;
        }

        bool loadErrorItemExists = false;
        for (int i = 0; i < paths.Count; i++)
        {
          var media = new MediaLoader.MediaLoader(paths[i]);
          var infoResult = await media.GetInformationAsync(false);
          var info = infoResult.Result;
          base.Add(info);
          TotalDuration += info.Duration;
          if (!info.LoadState)
          {
            Log.Warn($"플레이리스트 로드 경고 IsLoaded : [{info.LoadState}]", $"Title : [{info.Title}]\nPath : [{paths[i]}]");
            loadErrorItemExists = true;
          }
        }

        if (newPlaylist)
          EigenValue = Converter.SHA256Hash(playListData.FileName);

        Log.Info($"플레이리스트 로드 완료 MediaCount : [{paths.Count}] Loaded Warning [{loadErrorItemExists}]", $"Path : [{path}]");
        if (loadErrorItemExists)
          GlobalMessageEvent.Invoke("플레이리스트 로드 완료 [오류 항목이 있습니다]", false);
        else
          GlobalMessageEvent.Invoke("플레이리스트 로드 완료", true);
        NeedSave = false;
        return true;
      }
      else
      {
        Log.Fatal("플레이리스트 로드 중 오류 발생", new FileNotFoundException("File Not Found"), $"Path : [{path}]");
        GlobalMessageEvent.Invoke("플레이리스트 로드 실패! [로그를 확인해주세요]");
        return false;
      }
    }
    #endregion

    public new async Task Add(MediaInformation information)
    {
      if (!information.LoadState)
      {
        var loadResult = await new MediaLoader.MediaLoader(information.MediaLocation).GetInformationAsync(false);
        information = loadResult.Result;

        if (!loadResult)
        {
          Log.Error($"플레이리스트 항목 추가 실패 IsLoaded : [{information.LoadState}]", $"Title : [{information.Title}]\nFileName : {Path.GetFileName(information.MediaLocation)}");
          GlobalMessageEvent.Invoke($"플레이리스트 항목 추가 실패 (로그확인 권장)\nTitle : [{information.Title}]");
          return;
        }
      }
      this.TotalDuration += information.Duration;
      base.Add(information);
      Log.Info($"플레이리스트 항목 추가 완료 IsLoaded : [{information.LoadState}]", $"Title : [{information.Title}]\nFileName : {Path.GetFileName(information.MediaLocation)}");
    }

    /// <summary>
    /// 리스트 추가
    /// </summary>
    /// <param name="mediaLocation">추가할 미디어의 위치</param>
    public async Task Add(string mediaLocation)
    {
      var loader = new MediaLoader.MediaLoader(mediaLocation);
#if DEBUG
      Log.Debug("플레이리스트 항목 추가 시도", $"Path : [{mediaLocation}]");
      Log.Debug($"\n\n{mediaLocation}");
      loader.ProgressChanged += (_, e) => Log.Debug($"{e.ProgressKind}".PadRight(15) + $"{e.Percentage}%".PadLeft(4) + $"   {e.UserMessage}");
#endif

      var loadResult = await loader.GetInformationAsync(false);
      var info = loadResult.Result;

      if (!loadResult)
      {
        Log.Error($"플레이리스트 항목 추가 실패 IsLoaded : [{info.LoadState}]", $"Title : [{info.Title}]\nFileName : {Path.GetFileName(mediaLocation)}");
        GlobalMessageEvent.Invoke($"플레이리스트 항목 추가 실패 (로그확인 권장)\nTitle : [{info.Title}]");
        return;
      }

      TotalDuration += info.Duration;
      base.Add(info);
      Log.Info($"플레이리스트 항목 추가 완료 IsLoaded : [{info.LoadState}]", $"Title : [{info.Title}]\nFileName : {Path.GetFileName(mediaLocation)}");
      NeedSave = true;
    }

    /// <summary>
    /// 리스트에서 <seealso cref="MediaInformation"/>를 제거합니다.
    /// </summary>
    /// <param name="media">제거할 미디어 정보</param>
    public new void Remove(MediaInformation mediaInfo)
    {
      Log.Debug("플레이리스트 항목 제거 시도", $"Title : [{mediaInfo.Title}]");
      if (base.Contains(mediaInfo))
      {
        base.Remove(mediaInfo);
        TotalDuration -= mediaInfo.Duration;
        Log.Info("플레이리스트 항목 제거 완료", $"Title : [{mediaInfo.Title}]");
      }
      else
        Log.Fatal("플레이리스트 항목 제거 실패", new NullReferenceException("Unlisted Media"), $"Title : [{mediaInfo.Title}]");
      NeedSave = true;
    }

    /// <summary>
    /// Index의 미디어 정보를 리스트에서 제거합니다.
    /// </summary>
    /// <param name="index">제거할 미디어 정보 Index</param>
    public new void RemoveAt(int index)
    {
      Log.Debug($"플레이리스트 Index 항목 제거 시도.\nIndex : [{index}]");
      if (base.Count > index && index >= 0)
      {
        index--;
        base.RemoveAt(index);
        TotalDuration -= base[index].Duration;
        Log.Info($"플레이리스트 Index 항목 제거 완료.\nIndex : [{index}]");
      }
      else
        Log.Fatal($"플레이리스트 Index 항목 제거 실패", new IndexOutOfRangeException($"Index Out Of Range.\nBase Count : [{base.Count}]\nIndex : [{index}]"));
      NeedSave = true;
    }

    public new void Insert(int index, MediaInformation item)
    {
      if (base.Count > index && index >= 0)
      {
        base.Insert(index, item);
        TotalDuration += item.Duration;
        NeedSave = true;
      }
    }

    public async Task ReloadAtAsync(int index)
    {
      if (base.Count > index && index >= 0)
      {
        var item = base[index];
        if (MainMediaPlayer.MediaLoadedCheck && this.IndexOf(MainMediaPlayer.NotChangedMediaInformation) == index)
        {
          Log.Error($"플레이리스트 항목 리로드 실패 \n재생 중인 항목 리로드 시도\nIndex : [{index}]\nIsLoaded : [{item.LoadState}]", $"Title : [{item.Title}]\nLocation : [{item.MediaLocation}]");
          GlobalMessageEvent.Invoke($"재생 중인 항목은 리로드 할 수 없습니다.\nTitle : [{item.Title}]");
          return;
        }
        TotalDuration -= item.Duration;
        var loader = new MediaLoader.MediaLoader(item.MediaLocation);
#if DEBUG
        Log.Debug($"\n\n{item.Title}   {item.MediaLocation}");
        loader.ProgressChanged += (_, e) => Log.Debug($"{e.ProgressKind}".PadRight(15) + $"{e.Percentage}%".PadLeft(4) + $"   {e.UserMessage}");
#endif
        await loader.GetStreamPathAsync(false);
        var loadResult = await loader.GetInformationAsync(false);
        item = loadResult.Result;

        if (!loadResult)
        {
          Log.Error($"플레이리스트 항목 리로드 실패 Index : [{index}] IsLoaded : [{item.LoadState}]", $"Title : [{item.Title}]\nLocation : [{item.MediaLocation}]");
          GlobalMessageEvent.Invoke($"플레이리스트 항목 리로드 실패 (로그확인 권장)\nTitle : [{item.Title}]");
          return;
        }

        TotalDuration += item.Duration;
        base[index] = item;
        Log.Info($"플레이리스트 항목 리로드 완료 Index : [{index}] IsLoaded : [{item.LoadState}]", $"Title : [{item.Title}]\nLocation : [{item.MediaLocation}]");
        GlobalMessageEvent.Invoke($"플레이리스트 항목 리로드 완료\nTitle : [{item.Title}]", true);
        NeedSave = true;
      }
    }

    public async Task ReloadAsync(MediaInformation item) => await ReloadAtAsync(base.IndexOf(item));

    public async Task ReloadAllAsync()
    {
      for (int i = 0; i < base.Count; i++)
        await ReloadAtAsync(i);
    }

    public new void Clear()
    {
      this.TotalDuration = TimeSpan.Zero;
      base.Clear();
      NeedSave = true;
    }
  }
}
