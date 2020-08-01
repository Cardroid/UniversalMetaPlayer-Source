﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Core.Model;

namespace UMP.Core.Function
{
  public class MediaLoader : IMediaLoader
  {
    private readonly IMediaLoader Loader;

    public MediaLoader(string mediaLocation)
    {
      switch (GlobalProperty.MediaLoadEngine)
      {
        case GlobalProperty.MediaLoadEngineType.YoutubeDL:
          this.Loader = new YTDLMediaLoader(mediaLocation);
          break;
        case GlobalProperty.MediaLoadEngineType.Native:
        default:
          this.Loader = new NativeMediaLoader(mediaLocation);
          break;
      }
    }
    public MediaLoader(MediaInfomation info)
    {
      switch (GlobalProperty.MediaLoadEngine)
      {
        case GlobalProperty.MediaLoadEngineType.YoutubeDL:
          this.Loader = new YTDLMediaLoader(info);
          break;
        case GlobalProperty.MediaLoadEngineType.Native:
        default:
          this.Loader = new NativeMediaLoader(info);
          break;
      }
    }

    public bool Online => Loader.Online;

    public async Task<GenericResult<string>> GetID() => await Loader.GetID();

    public async Task<MediaInfomation> GetInfomationAsync(bool fullLoad) => await Loader.GetInfomationAsync(fullLoad);

    public async Task<GenericResult<string>> GetStreamPathAsync(bool useCache) => await Loader.GetStreamPathAsync(useCache);
  }
}