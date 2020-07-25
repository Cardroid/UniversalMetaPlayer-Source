using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UMP.Core;
using UMP.Core.Model;

namespace UMP.Controller.ViewModel
{
  public class MediaInfoControlViewModel : ViewModelBase
  {
    public MediaInfoControlViewModel()
    {
      MainMediaPlayer.PropertyChangedEvent += MainMediaPlayer_PropertyChangedEvent;
    }

    private void MainMediaPlayer_PropertyChangedEvent(string propertyname)
    {
      if (propertyname == "MediaInfomation")
      {
        OnPropertyChanged("MediaTitle");
        OnPropertyChanged("AlbumTitle");
        OnPropertyChanged("AlbumArtist");
        OnPropertyChanged("AlbumImage");
      }
    }

    public const string INFO_NULL = "정보 없음";

  #region 미디어 제목
    public string MediaTitle =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfomation.Title)
        ? MainMediaPlayer.MediaInfomation.Title : INFO_NULL)
        : "재생 중인 미디어가 없습니다";
  #endregion

  #region 앨범 제목
    public string AlbumTitle =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfomation.AlbumTitle)
        ? MainMediaPlayer.MediaInfomation.AlbumTitle : INFO_NULL)
        : "Error";
  #endregion

  #region 아티스트 이름
    public string AlbumArtist =>
      MainMediaPlayer.MediaLoadedCheck
        ? (!string.IsNullOrWhiteSpace(MainMediaPlayer.MediaInfomation.AlbumArtist)
        ? MainMediaPlayer.MediaInfomation.AlbumArtist : INFO_NULL)
        : "Error";
  #endregion

  #region 앨범 이미지
    public ImageSource AlbumImage 
    {
      get
      {
        if (MainMediaPlayer.MediaLoadedCheck && MainMediaPlayer.MediaInfomation.AlbumImage != null)
        {
          ImageMargin = new Thickness(0);
          return MainMediaPlayer.MediaInfomation.AlbumImage;
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
