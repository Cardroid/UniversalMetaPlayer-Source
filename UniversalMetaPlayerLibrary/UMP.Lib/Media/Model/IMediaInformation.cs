using System;

namespace UMP.Lib.Media.Model
{
    public interface IMediaInformation
    {
        /// <summary>
        /// 정보 로드 상태
        /// </summary>
        public MediaInfoLoadedStatus InfoLoadedStatus { get; }

        /// <summary>
        /// 스트림 로드 상태
        /// </summary>
        public MediaStreamLoadedStatus StreamLoadedStatus { get; }

        /// <summary>
        /// 온라인 미디어 여부
        /// </summary>
        public bool IsOnlineMedia { get; }

        /// <summary>
        /// 파일의 위치 (Url, 로컬)
        /// </summary>
        public string MediaLocation { get; }

        /// <summary>
        /// 실제 스트림 파일의 위치 (무조건 로컬)
        /// </summary>
        public string MediaStreamPath { get; }

        /// <summary>
        /// 도메인 (온라인일 경우)
        /// </summary>
        public string Domain { get; }

        /// <summary>
        /// 타이틀
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// 미디어의 길이
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// 엘범이미지<br/>
        /// [1] = 이미지 Index, [2] = 이미지 Data
        /// </summary>
        public byte[][] AlbumImage { get; }

        /// <summary>
        /// 기타 테그들
        /// </summary>
        public MediaInfoTags Tags { get; }
    }

    public enum MediaInfoLoadedStatus
    {
        /// <summary>
        /// 정보가 로드 되지 않음
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// 일부 정보가 로드됨<br/>
        /// (Full 보다 메모리 절약)
        /// </summary>
        SemiLoaded = 1,
        /// <summary>
        /// 모든 정보가 로드됨
        /// </summary>
        FullLoaded = 2,
        /// <summary>
        /// 로드에 실패한 경우
        /// </summary>
        LoadFail = -1,
    }

    public enum MediaStreamLoadedStatus
    {
        /// <summary>
        /// 스트림이 로드 되지 않았거나, 신뢰할 수 없는 경우
        /// </summary>
        NotLoaded = 0,
        /// <summary>
        /// 스트림이 로드됨<br/>
        /// (스트림 위치가 확정됨)
        /// </summary>
        Loaded = 1,
        /// <summary>
        /// 로드에 실패한 경우
        /// </summary>
        LoadFail = -1,
    }
}