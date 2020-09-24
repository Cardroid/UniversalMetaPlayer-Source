using System;
using System.IO;

using UMP.Lib.Utility;

namespace UMP.Lib.Media.Model
{
    public class MediaInformation : IMediaInformation
    {
        private MediaInformation()
        {
        }

        internal static MediaInformation Create(string mediaLocation)
        {
            if (string.IsNullOrWhiteSpace(mediaLocation))
                throw new NullReferenceException("MediaLocation is Null");

            bool isOnline = false;
            string mediaStreamPath = string.Empty;

            var instence = new MediaInformation
            {
                InfoLoadedStatus = MediaInfoLoadedStatus.NotLoaded,
                StreamLoadedStatus = MediaStreamLoadedStatus.NotLoaded,
                IsOnlineMedia = isOnline,
                MediaLocation = mediaLocation,
                MediaStreamPath = mediaStreamPath,
                Domain = string.Empty,
                Title = string.Empty,
                Duration = TimeSpan.Zero,
                AlbumImage = null,
                Tags = new MediaInfoTags()
            };

            return instence;
        }

        public MediaInfoLoadedStatus InfoLoadedStatus { get; set; }

        public MediaStreamLoadedStatus StreamLoadedStatus { get; set; }

        public bool IsOnlineMedia { get; set; }

        public string MediaLocation { get; set; }

        public string MediaStreamPath { get; set; }

        public string Domain { get; set; }

        public string Title { get; set; }

        public TimeSpan Duration { get; set; }

        public byte[][] AlbumImage { get; set; }

        public MediaInfoTags Tags { get; set; }
    }
}