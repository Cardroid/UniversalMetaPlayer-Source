using System;
using System.IO;
using System.Threading.Tasks;

using Serilog;

using UMP.Lib.Media.Model;
using UMP.Lib.Model;
using UMP.Lib.Option;
using UMP.Lib.Utility;

namespace UMP.Lib.Media.Loader
{
    public class LocalMediaLoader
    {
        /// <summary>
        /// 로컬 미디어 로드 시도
        /// </summary>
        /// <param name="info">Location이 포함된 <see cref="MediaInformation"/></param>
        /// <param name="fullLoad">모든 정보 로드 여부 (메모리 소모량 많아 짐)</param>
        /// <param name="log">로거</param>
        /// <param name="progress">진행율</param>
        /// <returns>성공시 true</returns>
        public static async Task<GenericResult<MediaInformation>> LoadInfoAsync(MediaInformation info, bool fullLoad, ILogger logger = null, IMessageProgress<double> progress = null)
        {
            if (logger == null)
                logger = LogHelper.Logger;

            if (progress != null)
                progress.Report(0, "초기화 중...");
            var path = info.MediaStreamPath;

            if (progress != null)
                progress.Report(5, "미디어 타입 파싱 중...");
            bool online = Parser.GetParser().GetUrlInfo(info.MediaLocation).Success;

            if (progress != null)
                progress.Report(10, "파일 정보 로드 중...");
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                await Task.Run(() =>
                {
                    using var Fileinfo = TagLib.File.Create(path);

            // 미디어 정보를 정보 클래스에 저장
            if (progress != null)
                        progress.Report(15, "타이틀 로드 중...");
                    info.Title = string.IsNullOrWhiteSpace(Fileinfo.Tag.Title)
            ? online
            ? $"{info.MediaLocation} form Online"
            : Path.GetFileNameWithoutExtension(info.MediaLocation)
            : Fileinfo.Tag.Title;

                    if (progress != null)
                        progress.Report(20, "재생길이 로드 중...");
                    info.Duration = Fileinfo.Properties.Duration;

            // 모든 정보 로드
            if (fullLoad)
                    {
                        if (progress != null)
                            progress.Report(25, "엘범 이미지 로드 중...");
                        int i = 0;
                        try
                        {
                            for (i = 0; i < Fileinfo.Tag.Pictures.Length; i++)
                                info.AlbumImage[i] = Fileinfo.Tag.Pictures[i].Data.Data;
                        }
                        catch (Exception e)
                        {
                            if (info.AlbumImage != null)
                                info.AlbumImage[i] = null;
                            logger.Debug(e, "앨범 이미지 로드 오류 처리");
                        }

                        if (progress != null)
                            progress.Report(40, "테그 정보 로드 중...");
                        info.Tags[MediaInfoType.AlbumTitle] = string.IsNullOrWhiteSpace(Fileinfo.Tag.Album)
                ? online
                ? $"{info.MediaLocation} form Online"
                : Path.GetFileNameWithoutExtension(info.MediaLocation)
                : Fileinfo.Tag.Album;
                        info.Tags[MediaInfoType.AlbumArtist] = Fileinfo.Tag.FirstAlbumArtist;
                        info.Tags[MediaInfoType.Lyrics] = Fileinfo.Tag.Lyrics;
                    }
                    progress.Report(90, "상태 업데이트 중...");
                    info.LoadedStatus = fullLoad ? MediaInfoLoadedStatus.FullLoaded : MediaInfoLoadedStatus.SemiLoaded;
                    info.Tags.IsLock = true;
                });
                progress.Report(100, "완료");
                return new GenericResult<MediaInformation>(true, info);
            }
            else
            {
                if (progress != null)
                    progress.Report(-1, "미디어 파일이 없습니다");
                logger.Error("미디어 파일이 없습니다");
                return new GenericResult<MediaInformation>(false);
            }
        }

        /// <summary>
        /// 캐시에서 미디어 스트림 파일 찾기 시도
        /// </summary>
        /// <param name="id">비디오 ID</param>
        /// <returns>성공시 true, 미디어 스트림 파일 경로</returns>
        public static GenericResult<string> TryGetOnlineMediaCacheAsync(string id, string cachePath)
        {
            if (!Directory.Exists(cachePath) || string.IsNullOrWhiteSpace(id))
                return new GenericResult<string>(false);

            string[] searchFiles = Directory.GetFiles(cachePath, $"{id}.mp3", SearchOption.AllDirectories);
            if (searchFiles.Length > 0)
                return new GenericResult<string>(true, searchFiles[0]);
            else
                return new GenericResult<string>(false);
        }
    }
}
