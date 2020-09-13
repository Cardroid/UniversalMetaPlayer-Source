using System;
using System.IO;

using UMP.Lib.Utility;

namespace UMP.Lib.Media.Model
{
  public interface IMediaInformation
  {
    public void InformationLoad(IMediaInfoLoader loader);

    public void StreamLoad(IMediaStreamLoader loader);

    /// <summary>
    /// 정보 로드 상태
    /// </summary>
    public MediaInfoLoadedStatus LoadedStatus { get;  }

    /// <summary>
    /// 온라인 미디어 여부
    /// </summary>
    public bool IsOnlineMedia { get;  }

    /// <summary>
    /// 파일의 위치 (온라인, 로컬)
    /// </summary>
    public string MediaLocation { get;  }

    /// <summary>
    /// 실제 스트림 파일의 위치 (무조건 로컬)
    /// </summary>
    public string MediaStreamPath { get;  }

    /// <summary>
    /// 타이틀
    /// </summary>
    public string Title { get;  }

    /// <summary>
    /// 미디어의 총 재생시간
    /// </summary>
    public TimeSpan Duration { get;  }

    /// <summary>
    /// 존재하면 엘범이미지, 존재하지 않으면 로고 이미지가 반환됩니다.
    /// </summary>
    public byte[][] AlbumImage { get;  }

    /// <summary>
    /// 기타 테그들
    /// </summary>
    public MediaInfoTags Tags { get;  }
  }

  public enum MediaInfoLoadedStatus
  {
    /// <summary>
    /// 정보가 로드 되지 않음
    /// </summary>
    Not, 
    /// <summary>
    /// 일부 정보가 로드됨<br/>
    /// (Full 보다 메모리 절약)
    /// </summary>
    Semi,
    /// <summary>
    /// 모든 정보가 로드됨
    /// </summary>
    Full
  }
}