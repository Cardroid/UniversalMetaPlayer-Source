using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UMP.Utility;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net;

namespace UMP.Core.Model
{
  /// <summary>
  /// 미디어 정보 구조체
  /// </summary>
  public struct MediaInformation
  {
    /// <summary>
    /// 정보 로드 여부
    /// </summary>
    public bool LoadState { get; set; }
    /// <summary>
    /// 파일의 위치 (온라인, 로컬)
    /// </summary>
    public string MediaLocation { get; set; }
    /// <summary>
    /// 실제 스트림 파일의 위치 (무조건 로컬)
    /// </summary>
    public string MediaStreamPath { get; set; }
    /// <summary>
    /// 타이틀
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// 미디어의 총 재생시간
    /// </summary>
    public TimeSpan Duration { get; set; }
    /// <summary>
    /// 존재하면 엘범이미지, 존재하지 않으면 로고 이미지가 반환됩니다.
    /// </summary>
    public ImageSource AlbumImage { get; set; }
    /// <summary>
    /// 기타 테그들
    /// </summary>
    public MediaInfoDictionary Tags { get; set; }
  }
}