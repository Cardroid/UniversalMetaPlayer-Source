using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using YoutubeExplode;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

using UMP.Lib.Media.Model;
using UMP.Lib.Model;
using UMP.Lib.Utility;

namespace UMP.Lib.Media.Loader
{
    public class NativeMediaLoader : IMediaStreamLoader
    {
        Log Log { get; }
        private MediaInformation Information;
        public bool Online { get; private set; }
        public string CachePath { get; set; }

        public event MessageProgressChangedEventHandler ProgressChanged;
        private void OnProgressChanged(ProgressKind progressKind, int percentage, string userMessage = "") => ProgressChanged?.Invoke(this, new MessageProgressChangedEventArgs(progressKind, percentage, userMessage));

        private NativeMediaLoader()
        {
            this.Log = new Log($"{typeof(NativeMediaLoader)}");
            this.CachePath = GlobalProperty.Predefine.OnlineMediaCachePath;
        }

        public NativeMediaLoader(string mediaLocation) : this()
        {
            if (!string.IsNullOrWhiteSpace(mediaLocation))
            {
                this.Information = new MediaInformation(mediaLocation);
                if (Checker.IsLocalPath(mediaLocation) && File.Exists(mediaLocation))
                {
                    this.Online = false;
                    this.Information.MediaStreamPath = Information.MediaLocation;
                }
                else
                    this.Online = true;
            }
            else
                this.Information = MediaInformation.Create();
        }

        public NativeMediaLoader(MediaInformation mediaInfo) : this(mediaInfo.MediaLocation) { }

        public async Task<GenericResult<string>> GetID()
        {
            if (!Online)
            {
                Log.Error(PredefineMessage.IsNotOnlineMedia);
                return new GenericResult<string>(false);
            }

            VideoId? id = null;
            await Task.Run(() => { id = VideoId.TryParse(Information.MediaLocation); });

            if (id.HasValue)
                return new GenericResult<string>(true, id.Value.ToString());
            else
                return new GenericResult<string>(false);
        }

        public async Task<GenericResult<MediaInformation>> GetInformationAsync(bool fullLoad)
        {
            OnProgressChanged(ProgressKind.Info, 0, "초기화 중...");
            OnProgressChanged(ProgressKind.Info, 5, "미디어 타입 확인 중...");
            if (Online)
            {
                OnProgressChanged(ProgressKind.Info, 10, "미디어 파일 로드 중...");
                var result = await GetYouTubeMediaAsync(true); if (result)
                    Information.MediaStreamPath = result.Result;
                else
                {
                    OnProgressChanged(ProgressKind.Info, -1, $"Fatal : {result.Result}");
                    return new GenericResult<MediaInformation>(false, Information);
                }
            }

            OnProgressChanged(ProgressKind.Info, 20, "미디어 정보 로드 중...");
            MediaLoaderProgress progress = new MediaLoaderProgress();
            progress.ProgressChanged += (percentage, msg) => OnProgressChanged(ProgressKind.InfoLoad, (int)percentage, msg);
            var resultInfo = await LocalMediaLoader.TryLoadInfoAsync(Information, fullLoad, Log, progress);
            if (resultInfo)
            {
                OnProgressChanged(ProgressKind.Info, 100, "완료");
                Information = resultInfo.Result;
                return new GenericResult<MediaInformation>(true, Information);
            }
            else
            {
                LoadFailProcess();
                OnProgressChanged(ProgressKind.Info, -1, "미디어 정보 로드 실패");
                return new GenericResult<MediaInformation>(false, Information);
            }
        }
        #region Info
        /// <summary>
        /// 유튜브에서 정보를 다운로드하여 다운로드된 스트림에 저장합니다<br/>
        /// (Mp3로 변환후 저장 권장)
        /// </summary>
        private async Task<bool> TryYouTubeMetaDataSave()
        {
            if (!Online)
            {
                Log.Error(PredefineMessage.IsNotOnlineMedia);
                return false;
            }

            OnProgressChanged(ProgressKind.InfoSave, 0, "초기화 중...");
            OnProgressChanged(ProgressKind.InfoSave, 5, "파일 로드 중...");
            if (string.IsNullOrWhiteSpace(Information.MediaStreamPath) && !File.Exists(Information.MediaStreamPath))
            {
                Log.Fatal("파일이 없습니다", new FileNotFoundException("File Not Found"), $"Path : [{Information.MediaStreamPath}]");
                return false;
            }

            if (Checker.GetChecker().CheckForInternetConnection())
            {
                bool success = false;
                await Task.Run(async () =>
                {
                    YoutubeClient youtube = new YoutubeClient();
                    Video videoinfo = null;

                    OnProgressChanged(ProgressKind.InfoSave, 10, "메타데이터 다운로드 중...");
                    try
                    {
                        OnProgressChanged(ProgressKind.InfoDownload, 0, "초기화 중...");
                        OnProgressChanged(ProgressKind.InfoDownload, 5, "정보 다운로드 중...");
                        videoinfo = await youtube.Videos.GetAsync(Information.MediaLocation);
                        OnProgressChanged(ProgressKind.InfoDownload, 100, "완료");
                    }
                    catch (Exception e)
                    {
                        Log.Fatal("네트워크에서 정보 로드 실패", e);
                        OnProgressChanged(ProgressKind.InfoDownload, -1, $"Fatal : {e.Message}");
                        OnProgressChanged(ProgressKind.InfoSave, -1, "Fatal : 정보 다운로드 실패");
                        success = false;
                        return;
                    }

                    OnProgressChanged(ProgressKind.InfoSave, 20, "파일 정보 생성 중...");
                    TagLib.File Fileinfo = null;
                    try { Fileinfo = TagLib.File.Create(Information.MediaStreamPath); }
                    catch (Exception e)
                    {
                        Log.Fatal("Mp3 파일 열기 또는 메타정보 로드 실패", e);
                        Fileinfo?.Dispose();
                        success = false;
                        OnProgressChanged(ProgressKind.InfoSave, -1, $"Fatal : {e.Message}");
                        return;
                    }

            // 기본정보 처리
            OnProgressChanged(ProgressKind.InfoSave, 35, "기본 정보 처리 중...");
                    Fileinfo.Tag.Title = videoinfo.Title;
                    Fileinfo.Tag.Album = $"\"{videoinfo.Url}\" form Online";
                    Fileinfo.Tag.AlbumArtists = new string[] { videoinfo.Author };
                    Fileinfo.Tag.Description = $"YouTubeID : \"{videoinfo.Id}\"";

            // 가사(자막) 처리
            OnProgressChanged(ProgressKind.InfoSave, 45, "가사(자막) 처리 중...");
                    try
                    {
                        var trackManifest = await youtube.Videos.ClosedCaptions.GetManifestAsync(Information.MediaLocation);

                        var trackInfo = trackManifest.TryGetByLanguage("ko");

                        if (trackInfo != null)
                        {
                            var track = await youtube.Videos.ClosedCaptions.GetAsync(trackInfo);

                            string caption = string.Empty;

                            for (int i = 0; i < track.Captions.Count; i++)
                                caption += $"{track.Captions[i].Text}\n";

                            Fileinfo.Tag.Lyrics = caption[0..^1];
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Warn("자막 로드 및 파싱 실패", e);
                        OnProgressChanged(ProgressKind.InfoSave, 50, "Warn : 가사(자막) 로드 밎 파싱 실패");
                    }

            // 섬네일 처리
            OnProgressChanged(ProgressKind.InfoSave, 55, "섬네일 다운로드 중...");
                    byte[] imagedata = null;
                    using (WebClient webClient = new WebClient())
                    {
                        try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MaxResUrl); }
                        catch
                        {
                            OnProgressChanged(ProgressKind.InfoSave, 56, "MaxRes 화질 다운로드 실패, HighRes 화질 시도");
                            try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.HighResUrl); }
                            catch
                            {
                                OnProgressChanged(ProgressKind.InfoSave, 57, "HighRes 화질 다운로드 실패, StandardRes 화질 시도");
                                try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.StandardResUrl); }
                                catch
                                {
                                    OnProgressChanged(ProgressKind.InfoSave, 58, "StandardRes 화질 다운로드 실패, MediumRes 화질 시도");
                                    try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.MediumResUrl); }
                                    catch
                                    {
                                        OnProgressChanged(ProgressKind.InfoSave, 59, "MediumRes 화질 다운로드 실패, LowRes 화질 시도");
                                        try { imagedata = await webClient.DownloadDataTaskAsync(videoinfo.Thumbnails.LowResUrl); }
                                        catch (Exception e)
                                        {
                                            Log.Warn("Thumbnail 다운로드 중 오류가 발생했습니다", e);
                                            OnProgressChanged(ProgressKind.InfoSave, 60, $"Warn : {e.Message}");
                                            imagedata = null;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    OnProgressChanged(ProgressKind.InfoSave, 70, "섬네일 처리 중...");
                    if (imagedata != null)
                    {
                        try
                        {
                            Fileinfo.Tag.Pictures = new TagLib.IPicture[]
                    {
                new TagLib.Picture(new TagLib.ByteVector(imagedata))
                {
                  Type = TagLib.PictureType.FrontCover,
                  Description = "Cover"
                }
                      };
                        }
                        catch (Exception e)
                        {
                            Log.Warn("메타데이터에 섬네일 정보등록을 실패했습니다", e);
                            OnProgressChanged(ProgressKind.InfoSave, 75, $"Warn : {e.Message}");
                        }
                    }
                    else
                    {
                        Log.Warn("섬네일 추출 실패", new NullReferenceException("Image data is Null"));
                        OnProgressChanged(ProgressKind.InfoSave, 75, "Warn : 섬네일 추출 실패 (Image data is Null)");
                    }

                    OnProgressChanged(ProgressKind.InfoSave, 85, "파일에 정보 쓰는 중...");
                    try
                    {
                        Fileinfo.Save();
                        Log.Info("YouTube에서 Mp3 메타 데이터 저장 완료");
                        success = true;
                        OnProgressChanged(ProgressKind.InfoSave, 100, "완료");
                    }
                    catch (Exception e)
                    {
                        Log.Fatal("메타데이터에 저장에 실패했습니다", e);
                        success = false;
                        OnProgressChanged(ProgressKind.InfoSave, -1, $"Fatal : {e.Message}");
                        return;
                    }
                    finally { Fileinfo.Dispose(); }
                    return;
                });
                return success;
            }
            else
            {
                Log.Error(PredefineMessage.UnableNetwork);
                OnProgressChanged(ProgressKind.InfoSave, -1, $"Error : {PredefineMessage.UnableNetwork}");
                return false;
            }
        }
        #endregion

        #region Other Function
        /// <summary>
        /// 미디어 로드에 실패했을 경우 호출
        /// </summary>
        private void LoadFailProcess()
        {
            if (!Information.Title.ToLower().StartsWith(GlobalProperty.Predefine.MEDIA_INFO_NULL.ToLower()))
            {
                if (string.IsNullOrWhiteSpace(Information.Title))
                    Information.Title = $"{GlobalProperty.Predefine.MEDIA_INFO_NULL} {Path.GetFileNameWithoutExtension(Information.MediaLocation)}";
                else
                    Information.Title = $"{GlobalProperty.Predefine.MEDIA_INFO_NULL} {Information.Title}";
            }
            Information.MediaStreamPath = string.Empty;
        }
        #endregion
    }
}
