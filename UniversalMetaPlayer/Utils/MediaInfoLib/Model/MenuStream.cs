/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * MenuStream.cs
 * 
 * Presents information and functionality specific to a menu stream.
 ******************************************************************************
 */

using System.ComponentModel;
using System.Text;

using UMP.Utils.MediaInfoLib.Library;

namespace UMP.Utils.MediaInfoLib.Model
{
  ///<summary>Represents a single menu stream.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class MenuStream : BaseStreamCommons
  {
    ///<summary>MenuStream constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object which has already opened a media file.</param>
    ///<param name="id">The MediaInfo ID for this menu stream.</param>
    public MenuStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.Menu, id)
    {
    }
    /// <summary>Overides base method to provide short summary of stream kind.</summary>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat(", {0}", this.Format);
      if (!string.IsNullOrEmpty(this.FormatProfile)) sb.AppendFormat(" {0}", this.FormatProfile);
      if (!string.IsNullOrEmpty(this.Title)) sb.AppendFormat(", '{0}'", this.Title);

      return sb.ToString();
    }

    #region AllStreamsCommon

    ///<summary>스트림 번호</summary>
    [Description("스트림 번호"), Category("AllStreamsCommon")]
    public int ID => base.StreamID;

    ///<summary>컨테이너 또는 스트림의 포멧</summary>
    [Description("컨테이너 또는 스트림의 포멧"), Category("AllStreamsCommon")]
    public new string Format => base.Format;

    ///<summary>컨테이너 또는 스트림의 포멧 정보</summary>
    [Description("컨테이너 또는 스트림의 포멧 정보"), Category("AllStreamsCommon")]
    public string FormatInfo => base.Format_Info;

    ///<summary>컨테이너 또는 스트림의 포멧 프로필</summary>
    [Description("컨테이너 또는 스트림의 포멧 프로필"), Category("AllStreamsCommon")]
    public string FormatProfile => base.Format_Profile;

    ///<summary>컨테이너 또는 스트림의 포멧 버전</summary>
    [Description("컨테이너 또는 스트림의 포멧 버전"), Category("AllStreamsCommon")]
    public string FormatVersion => base.Format_Version;

    ///<summary>컨테이너 또는 스트림의 제목</summary>
    [Description("컨테이너 또는 스트림의 제목"), Category("AllStreamsCommon")]
    public new string Title => base.Title;

    ///<summary>This stream's globally unique ID (GUID).</summary>
    [Description("This stream's or container's globally unique ID (GUID)."), Category("AllStreamsCommon")]
    public new string UniqueID => base.UniqueID;

    #endregion

    #region GeneralVideoAudioTextImageMenuCommon

    ///<summary>Codec ID available from some codecs.</summary>
    ///<example>AAC audio:A_AAC, h.264 video:V_MPEG4/ISO/AVC</example>
    [Description("Codec ID available from some codecs."), Category("GeneralVideoAudioTextImageMenuCommon")]
    public new string CodecId => base.CodecId;

    ///<summary>Common name of the codec.</summary>
    [Description("Common name of the codec."), Category("GeneralVideoAudioTextImageMenuCommon")]
    public new string CodecCommonName => base.CodecCommonName;

    #endregion

    #region GeneralVideoAudioTextMenu

    ///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
    [Description("Stream delay (e.g. to sync audio/video) in ms."), Category("GeneralVideoAudioTextMenu")]
    public new int Delay => base.Delay;

    ///<summary>Duration of the stream in milliseconds.</summary>
    [Description("Duration of the stream in milliseconds."), Category("GeneralVideoAudioTextMenu")]
    public new int Duration => base.Duration;

    #endregion

    #region VideoAudioTextImageMenuCommon

    ///<summary>2-letter (if available) or 3-letter ISO code.</summary>
    [Description("2-letter (if available) or 3-letter ISO code."), Category("VideoAudioTextImageMenuCommon")]
    public new string Language => base.Language;

    #endregion
  }
}
