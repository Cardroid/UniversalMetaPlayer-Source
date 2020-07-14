using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using CMP2.Core;
using CMP2.Core.Model;

namespace CMP2.Controller.ViewModel
{
  public class MediaInfoControlViewModel : ViewModelBase
  {
    public MediaInfoControlViewModel()
    {
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
    }

    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      if (propertyname == "MediaInfo")
      {
        OnPropertyChanged("MediaTitle");
        OnPropertyChanged("AlbumTitle");
        OnPropertyChanged("ArtistName");
        OnPropertyChanged("AlbumImage");
      }
    }

    public const string INFO_NULL = "정보 없음";

  #region 미디어 제목
    public string MediaTitle =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfo.Title)
        ? MainMediaPlayer.MediaInfo.Title : INFO_NULL)
        : "재생 중인 미디어가 없습니다";
  #endregion

  #region 앨범 제목
    public string AlbumTitle =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfo.AlbumTitle)
        ? MainMediaPlayer.MediaInfo.AlbumTitle : INFO_NULL)
        : "Error";
  #endregion

  #region 아티스트 이름
    public string ArtistName =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfo.ArtistName)
        ? MainMediaPlayer.MediaInfo.ArtistName : INFO_NULL)
        : "Error";
  #endregion

  #region 앨범 이미지
    public ImageSource AlbumImage 
    {
      get
      {
        if (MainMediaPlayer.MediaLoadedCheck && MainMediaPlayer.MediaInfo.AlbumImage != null)
        {
          ImageMargin = new Thickness(0);
          return MainMediaPlayer.MediaInfo.AlbumImage;
        }
        else
        {
          ImageMargin = new Thickness(70);
          return GlobalProperty.LogoNoteImage;
        }
      }
    }

    private Thickness _ImageMargin = new Thickness(70);
    public Thickness ImageMargin 
    {
      get => _ImageMargin;
      private set
      {
        _ImageMargin = value;
        OnPropertyChanged("ImageMargin");
      }
    }
  }
  #endregion
}
