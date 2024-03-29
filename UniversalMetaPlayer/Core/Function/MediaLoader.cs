﻿using System;
using System.Collections.Generic;
using System.Text;
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
      switch (GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine))
      {
        case Enums.MediaLoadEngineType.YoutubeDL:
          this.Loader = new YTDLMediaLoader(mediaLocation);
          break;
        case Enums.MediaLoadEngineType.Native:
        default:
          this.Loader = new NativeMediaLoader(mediaLocation);
          break;
      }
    }

    public MediaLoader(MediaInformation info)
    {
      switch (GlobalProperty.Options.Getter<Enums.MediaLoadEngineType>(Enums.ValueName.MediaLoadEngine))
      {
        case Enums.MediaLoadEngineType.YoutubeDL:
          this.Loader = new YTDLMediaLoader(info);
          break;
        case Enums.MediaLoadEngineType.Native:
        default:
          this.Loader = new NativeMediaLoader(info);
          break;
      }
    }

    public bool Online => Loader.Online;

    public async Task<GenericResult<string>> GetID() => await Loader.GetID();

    public async Task<MediaInformation> GetInformationAsync(bool fullLoad) => await Loader.GetInformationAsync(fullLoad);

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache) => await Loader.GetStreamPathAsync(useCache);
  }
}
