using System;
using System.Threading.Tasks;
using UMP.Core.Global;
using UMP.Core.Model;
using UMP.Core.Model.Media;

namespace UMP.Core.Function
{
  public class MediaLoader : IMediaLoader
  {
    private readonly IMediaLoader Loader;

    public MediaLoader(string mediaLocation)
    {
      this.Loader = (GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine)) switch
      {
        Enums.MediaLoadEngineType.YoutubeDL => new YTDLMediaLoader(mediaLocation),
        _ => new NativeMediaLoader(mediaLocation),
      };
    }

    public MediaLoader(MediaInformation info)
    {
      this.Loader = (GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine)) switch
      {
        Enums.MediaLoadEngineType.YoutubeDL => new YTDLMediaLoader(info),
        _ => new NativeMediaLoader(info),
      };
    }

    public event UMP_ProgressChangedEventHandler ProgressChanged
    {
      add => Loader.ProgressChanged += value;
      remove => Loader.ProgressChanged -= value;
    }

    public bool Online => Loader.Online;

    public string CachePath
    {
      get => Loader.CachePath;
      set => Loader.CachePath = value;
    }

    public async Task<GenericResult<string>> GetID() => await Loader.GetID();

    public async Task<GenericResult<MediaInformation>> GetInformationAsync(bool fullLoad) => await Loader.GetInformationAsync(fullLoad);

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache) => await Loader.GetStreamPathAsync(useCache);
  }
}
