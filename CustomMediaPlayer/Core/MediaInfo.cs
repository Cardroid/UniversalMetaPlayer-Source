using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using NAudio.Wave;

namespace CustomMediaPlayer.Core
{
    public class MediaInfo
    {
        private TagLib.File Fileinfo;
        private TagLib.Tag FileTag;

        public MediaInfo(FileInfo File)
        {
            FileName = File.Name;
            LocalLocation = File.DirectoryName;

            string Empty = "정보 없음";

            Fileinfo = TagLib.File.Create(File.FullName);
            FileTag = Fileinfo.Tag;

            // 미디어 정보를 정보 클래스에 저장
            try
            {
                TagLib.IPicture pic = Fileinfo.Tag.Pictures[0];  //pic contains data for image.
                MemoryStream stream = new MemoryStream(pic.Data.Data);  // create an in memory stream
                AlbumImage = BitmapFrame.Create(stream);
            }
            catch { AlbumImage = MainWindow.LogoImage; }
            Title = FileTag.Title ?? FileName;
            AlbumTitle = FileTag.Album ?? Empty;
            ArtistName = FileTag.FirstAlbumArtist ?? Empty;
            Duration = new AudioFileReader(File.FullName).TotalTime;
        }

        #region 프로퍼티 정의
        /// <summary>
        /// 파일의 이름
        /// </summary>
        public string FileName { get; private set; } = null;
        /// <summary>
        /// 파일의 위치
        /// </summary>
        public string LocalLocation { get; private set; } = null;
        /// <summary>
        /// 존재하면 엘범이미지, 존재하지 않으면 로고 이미지가 반환됩니다.
        /// </summary>
        public ImageSource AlbumImage { get; private set; } = MainWindow.LogoImage;
        /// <summary>
        /// 타이틀
        /// </summary>
        public string Title { get; private set; } = null;
        /// <summary>
        /// 엘범 타이틀
        /// </summary>
        public string AlbumTitle { get; private set; } = null;
        /// <summary>
        /// 아티스트 이름
        /// </summary>
        public string ArtistName { get; private set; } = null;
        /// <summary>
        /// 미디어의 총 재생시간
        /// </summary>
        public TimeSpan Duration { get; private set; } = TimeSpan.Zero;
        #endregion
    }
}
