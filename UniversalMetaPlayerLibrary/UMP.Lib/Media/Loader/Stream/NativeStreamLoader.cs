using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Serilog;

using UMP.Lib.Media.Model;
using UMP.Lib.Model;
using UMP.Lib.Option;
using UMP.Lib.Utility;

namespace UMP.Lib.Media.Loader.Stream
{
    public class NativeStreamLoader : IStreamLoader
    {
        public NativeStreamLoader(ILogger logger = null)
        {
            if (logger != null)
                this.Log = logger;
            else
                this.Log = LogHelper.Logger;

            this.CachePath = CacheManager.CachePath;
        }

        private ILogger Log { get; }

        private string CachePath { get; }

        public event MessageProgressChangedEventHandler<StreamProgressKind> StreamProgressChanged;

        private IMediaIDParer MediaIDParer;

        private void OnProgressChanged(StreamProgressKind StreamProgressKind, double percentage, string userMessage = "") =>
          StreamProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs<StreamProgressKind>(StreamProgressKind, percentage, userMessage));

        public async Task<GenericResult<string>> GetStreamPathAsync(MediaInformation mediaInfo, bool useCache, IMediaIDParer mediaIDParer)
        {
            this.MediaIDParer = mediaIDParer;

            OnProgressChanged(StreamProgressKind.Stream, 0, "초기화 중...");
            if (mediaInfo.IsOnlineMedia)
            {
                // 미디어의 위치가 Youtube인지 확인
                if (!mediaInfo.Domain.ToLower().Equals("youtu"))
                {
                    mediaInfo.StreamLoadedStatus = MediaStreamLoadedStatus.LoadFail;
                    var e = new NotSupportedException("NativeStreamLoader는 Youtube 미디어만 지원합니다");
                    OnProgressChanged(StreamProgressKind.Stream, -1, $"Fatal : {e}");
                    return new GenericResult<string>(false, e.Message);
                }

                var result = await GetYouTubeMediaAsync(mediaInfo, useCache);

                if (result)
                {
                    mediaInfo.MediaStreamPath = result.Result;
                    mediaInfo.StreamLoadedStatus = MediaStreamLoadedStatus.Loaded;
                    OnProgressChanged(StreamProgressKind.Stream, 100, "완료");
                }
                else
                {
                    mediaInfo.StreamLoadedStatus = MediaStreamLoadedStatus.LoadFail;
                    OnProgressChanged(StreamProgressKind.Stream, -1, $"Fatal : {result.Result}");
                }

                return result;
            }
            else
            {
                if (File.Exists(mediaInfo.MediaStreamPath))
                {
                    mediaInfo.StreamLoadedStatus = MediaStreamLoadedStatus.Loaded;
                    OnProgressChanged(StreamProgressKind.Stream, 100, "완료");
                    return new GenericResult<string>(true, mediaInfo.MediaStreamPath);
                }
                else
                {
                    mediaInfo.StreamLoadedStatus = MediaStreamLoadedStatus.LoadFail;
                    OnProgressChanged(StreamProgressKind.Stream, -1, $"Fatal : 파일이 없습니다");
                    return new GenericResult<string>(false);
                }
            }
        }

        /// <summary>
        ///  YouTube 미디어 스트림 & 정보 다운로드시도
        /// </summary>
        /// <param name="mediaInfo">미디어 정보 클래스</param>
        /// <param name="useCache">캐시된 미디어 사용 여부</param>
        /// <returns>스트림 캐시 저장 경로</returns>
        private async Task<GenericResult<string>> GetYouTubeMediaAsync(MediaInformation mediaInfo, bool useCache)
        {
            OnProgressChanged(StreamProgressKind.StreamLoad, 0, "초기화 중...");
            if (mediaInfo.IsOnlineMedia)
            {
                Log.Error(PredefineMessage.IsNotOnlineMedia);
                OnProgressChanged(StreamProgressKind.StreamLoad, -1, $"Error : {PredefineMessage.IsNotOnlineMedia}");
                return new GenericResult<string>(false);
            }

            // 캐시된 정보를 로드
            if (useCache)
            {
                OnProgressChanged(StreamProgressKind.StreamLoad, 5, "캐시 검색 중...");
                var streamCachePathResult = LocalMediaLoader.TryGetOnlineMediaCacheAsync((MediaInformation.).Result, CachePath);
                if (streamCachePathResult)
                {
                    Log.Debug("미디어 캐시가 확인됨");
                    OnProgressChanged(StreamProgressKind.StreamLoad, 100, "캐시 로드 완료");
                    return new GenericResult<string>(true, streamCachePathResult.Result);
                }
                else
                {
                    Log.Warning("캐시 된 미디어 로드 실패. 온라인에서 다운로드 시도");
                    OnProgressChanged(StreamProgressKind.StreamLoad, 10, "캐시 로드 실패...");
                }
            }

            if (Checker.GetChecker().CheckForInternetConnection())
            {
                // 캐시폴더가 존재하지 않을시 생성
                Checker.GetChecker().DirectoryCheck(CachePath);

                var id = await MediaIDParer.GetID(mediaInfo);
                if (!id)
                    return new GenericResult<string>(false, "MediaID 파싱 실패");
                string mp3FilePath = Path.Combine(CachePath, $"{id.Result}.mp3");
                string streamPath = string.Empty;
                try
                {
                    // 유튜브 스트림 다운로드
                    OnProgressChanged(StreamProgressKind.StreamLoad, 15, "다운로드 중...");
                    var streamPathResult = await TryDownloadYouTubeStreamAsync(CachePath);
                    if (streamPathResult)
                        streamPath = streamPathResult.Result;
                    else
                        throw new WebException("미디어 스트림 다운로드 실패");


                    // Mp3로 변환
                    OnProgressChanged(StreamProgressKind.StreamLoad, 40, "변환 중...");
                    MediaLoaderProgress progress = new MediaLoaderProgress();
                    progress.ProgressChanged += (percentage, msg) => OnProgressChanged(StreamProgressKind.StreamConvert, (int)percentage, msg);
                    if (!await Converter.ConvertToMP3Async(streamPath, mp3FilePath, progress))
                        throw new FileNotFoundException($"File is Null\nSourceFile : [{streamPath}]\nTargetFile : [{mp3FilePath}]");
                    File.Delete(streamPath);
                    if (File.Exists(mp3FilePath))
                        MediaInformation.MediaStreamPath = mp3FilePath;
                    else
                        throw new FileNotFoundException("미디어 스트림 변환 실패 (변환을 완료 했지만, 파일을 찾을 수 없습니다)");
                    Log.Information("미디어 스트림 변환 완료");


                    // 메타 데이터 저장
                    OnProgressChanged(StreamProgressKind.StreamLoad, 75, "메타데이터 저장 중...");
                    if (await TryYouTubeMetaDataSave())
                    {
                        Log.Info("미디어 메타데이터 다운로드 & 병합 완료");
                        OnProgressChanged(StreamProgressKind.StreamLoad, 100, "완료");
                    }
                    else
                        throw new InvalidCastException("미디어 메타데이터 다운로드 & 병합 실패");
                }
                catch (Exception e)
                {
                    Log.Fatal("미디어 스트림 처리 중 오류 발생", e, $"MediaLocation : [{Information.MediaLocation}]\nMp3Path : [{mp3FilePath}]\nStreamPath : [{streamPath}]");
                    OnProgressChanged(StreamProgressKind.StreamLoad, -1, $"Fatal : {e.Message}");
                    return new GenericResult<string>(false, e.Message);
                }
                return new GenericResult<string>(true, mp3FilePath);
            }
            else
            {
                Log.Error(PredefineMessage.UnableNetwork);
                OnProgressChanged(StreamProgressKind.StreamLoad, -1, $"Error : {PredefineMessage.UnableNetwork}");
                return new GenericResult<string>(false);
            }
        }

        /// <summary>
        /// 온라인에서 유튜브 스트림 다운로드를 시도합니다.
        /// </summary>
        /// <param name="path">저장할 폴더 경로</param>
        /// <returns>다운로드 성공시 스트림 저장 폴더 경로</returns>
        private async Task<GenericResult<string>> TryDownloadYouTubeStreamAsync(string path)
        {
            OnProgressChanged(StreamProgressKind.StreamDownload, 0, "초기화 중...");
            Checker.DirectoryCheck(path);

            if (Checker.CheckForInternetConnection())
            {
                // 미디어 스트림 다운로드
                var youtube = new YoutubeClient();
                try
                {
                    OnProgressChanged(StreamProgressKind.StreamDownload, 5, "매니페스트 가져오는 중...");
                    var streamManifest = await youtube.Videos.Streams.GetManifestAsync(Information.MediaLocation);
                    var streamInfo = streamManifest.GetAudioOnly().WithHighestBitrate();

                    if (streamInfo == null)
                        throw new NullReferenceException("스트림 정보가 Null입니다");

                    string savepath = Path.Combine(path, $"{(await GetID()).Result}.{streamInfo.Container}");

                    // Download the stream to file
                    OnProgressChanged(StreamProgressKind.StreamDownload, 10, "스트림 다운로드 시작 중...");
                    MediaLoaderProgress progress = new MediaLoaderProgress();
                    progress.ProgressChanged += (percentage, msg) => OnProgressChanged(StreamProgressKind.StreamDownload, (int)(percentage * 90) + 10, msg);
                    await youtube.Videos.Streams.DownloadAsync(streamInfo, savepath, progress);

                    if (string.IsNullOrWhiteSpace(savepath))
                        throw new FileNotFoundException("스트림 파일을 찾을 수 없습니다");

                    Log.Info("온라인에서 미디어 스트림 로드 완료");
                    OnProgressChanged(StreamProgressKind.StreamDownload, 100, "완료");
                    return new GenericResult<string>(true, savepath);
                }
                catch (Exception e)
                {
                    Log.Fatal("미디어 스트림 다운로드 실패", e);
                    return new GenericResult<string>(false, e.Message);
                }
            }
            else
            {
                Log.Error(PredefineMessage.UnableNetwork);
                OnProgressChanged(StreamProgressKind.StreamDownload, -1, $"Error : {PredefineMessage.UnableNetwork}");
                return new GenericResult<string>(false);
            }
        }
    }
}
