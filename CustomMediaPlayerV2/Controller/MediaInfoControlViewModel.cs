using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CMP2.Core;

using ControlzEx.Standard;

namespace CMP2.Controller
{
  public class MediaInfoControlViewModel : ViewModel
  {
    public MediaInfoControlViewModel()
    {
      MainMediaPlayer.AudioFileOpenEvent += MediaPlayerProperty_AudioFileOpenEvent;
    }

    private void MediaPlayerProperty_AudioFileOpenEvent(IMediaInfo mediaInfo)
    {
      MediaTitle = mediaInfo.Title;
      AlbumTitle = mediaInfo.AlbumTitle;
      ArtistName = mediaInfo.ArtistName;
      AlbumImage = mediaInfo.AlbumImage;
    }

    public const string INFO_NULL = "정보 없음";

    #region 미디어 제목
    private string _MediaTitle = "재생 중인 미디어가 없습니다";
    public string MediaTitle
    {
      get => MainMediaPlayer.AudioFile != null ? (!string.IsNullOrWhiteSpace(_MediaTitle) ? _MediaTitle : INFO_NULL) : "재생 중인 미디어가 없습니다";
      set
      {
        _MediaTitle = value;
        OnPropertyChanged("MediaTitle");
      }
    }
    #endregion

    #region 앨범 제목
    private string _AlbumTitle = string.Empty;
    public string AlbumTitle
    {
      get => !string.IsNullOrWhiteSpace(_AlbumTitle) ? _AlbumTitle : INFO_NULL;
      set
      {
        _AlbumTitle = value;
        OnPropertyChanged("AlbumTitle");
      }
    }
    #endregion

    #region 아티스트 이름
    private string _ArtistName = string.Empty;
    public string ArtistName
    {
      get => !string.IsNullOrWhiteSpace(_ArtistName) ? _ArtistName : INFO_NULL;
      set
      {
        _ArtistName = value;
        OnPropertyChanged("ArtistName");
      }
    }
    #endregion

    #region 앨범 이미지
    private ImageSource _AlbumImage = null;
    public ImageSource AlbumImage
    {
      get => _AlbumImage ?? IGlobalProperty.LogoNoteImage;
      set
      {
        _AlbumImage = value;
        OnPropertyChanged("AlbumImage");
      }
    }
    #endregion
  }
}
