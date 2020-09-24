using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using UMP.Lib.Model;

namespace UMP.Lib.Media.Model
{
    public abstract class MediaLoader
    {
        public MediaLoader(IInfoLoader infoLoader, IStreamLoader streamLoader, IMediaIDParer mediaIDParer)
        {
            this.InfoLoader = infoLoader;
            this.StreamLoader = streamLoader;
            this.MediaIDParer = mediaIDParer;

            this.InfoLoader.InfoProgressChanged += (_, e) =>
              ProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs<MediaProgressKind>((MediaProgressKind)e.ProgressKind, e.Percentage, e.UserMessage));
            this.StreamLoader.StreamProgressChanged += (_, e) =>
              ProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs<MediaProgressKind>((MediaProgressKind)e.ProgressKind, e.Percentage, e.UserMessage));
        }

        private IInfoLoader InfoLoader { get; }
        private IStreamLoader StreamLoader { get; }
        private IMediaIDParer MediaIDParer { get; }

        public event MessageProgressChangedEventHandler<MediaProgressKind> ProgressChanged;

        public Task<GenericResult<string>> GetID(MediaInformation mediaInfo) => MediaIDParer.GetID(mediaInfo);

        public Task<GenericResult<MediaInformation>> GetInformationAsync(MediaInformation mediaInfo, bool fullLoad) => InfoLoader.GetInformationAsync(mediaInfo, fullLoad, MediaIDParer);

        public Task<GenericResult<string>> GetStreamPathAsync(MediaInformation mediaInfo, bool useCache) => StreamLoader.GetStreamPathAsync(mediaInfo, useCache, MediaIDParer);
    }
}
