/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * ImageStream.cs
 * 
 * Presents information and functionality specific to an image stream.
 ******************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using UMP.Utility.MediaInfoLib.Library;

namespace UMP.Utility.MediaInfoLib.Model
{
  ///<summary>Represents a single image stream.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class ImageStream : BaseStreamCommons
  {
    ///<summary>ImageStream constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="id">The MediaInfo ID for this image stream.</param>
    public ImageStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.Image, id)
    {
    }
    /// <summary>Overides base method to provide short summary of stream kind.</summary>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0}", this.Format);
      if (!string.IsNullOrEmpty(this.FormatProfile)) sb.AppendFormat(" {0}", this.FormatProfile);
      sb.AppendFormat(", {0}x{1} px", this.Width, this.Height);
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

    #region GeneralVideoAudioTextImageCommon

    ///<summary>Date and time stream encoding completed.</summary>
    [Description("Date and time stream encoding completed."), Category("GeneralVideoAudioTextImageCommon")]
    public new DateTime EncodedDate => base.EncodedDate;

    ///<summary>Software used to encode this stream.</summary>
    [Description("Software used to encode this stream."), Category("GeneralVideoAudioTextImageCommon")]
    public string EncodedLibrary => base.EncoderLibrary;

    ///<summary>Media type of stream, formerly called MIME type.</summary>
    [Description("Media type of stream, formerly called MIME type."), Category("GeneralVideoAudioTextImageCommon")]
    public new string InternetMediaType => base.InternetMediaType;

    ///<summary>Size in bytes.</summary>
    [Description("Size in bytes."), Category("GeneralVideoAudioTextImageCommon")]
    public new long Size => base.Size;

    ///<summary>Encoder settings used for encoding this stream.
    ///String format: name=value / name=value / ...</summary>
    [Description("Encoder settings used for encoding this stream. (Raw String)"), Category("GeneralVideoAudioTextImageCommon")]
    public new string EncoderSettingsRaw => base.EncoderSettingsRaw;

    ///<summary>Encoder settings used for encoding this stream (as dictionary).</summary>
    [Description("Encoder settings used for encoding this stream. (Dictionary)"), Category("GeneralVideoAudioTextImageCommon")]
    public new IDictionary<string, string> EncoderSettings => base.EncoderSettings;

    #endregion

    #region VideoAudioTextImageCommon

    ///<summary>Compression mode (lossy or lossless).</summary>
    [Description("Compression mode (lossy or lossless)."), Category("VideoAudioTextImageCommon")]
    public new string CompressionMode => base.CompressionMode;

    ///<summary>Ratio of current size to uncompressed size.</summary>
    [Description("Ratio of current size to uncompressed size."), Category("VideoAudioTextImageCommon")]
    public new string CompressionRatio => base.CompressionRatio;

    ///<example>Stream bit depth (16, 24, 32...)</example>
    [Description("Stream bit depth (16, 24, 32...)"), Category("VideoAudioTextImageCommon")]
    public new int BitDepth => base.BitDepth;

    #endregion

    #region VideoAudioTextImageMenuCommon

    ///<summary>2-letter (if available) or 3-letter ISO code.</summary>
    [Description("2-letter (if available) or 3-letter ISO code."), Category("VideoAudioTextImageMenuCommon")]
    public new string Language => base.Language;

    #endregion

    #region VideoImageCommon

    ///<summary>Ratio of display width to display height.</summary>
    [Description("Ratio of display width to display height."), Category("VideoImageCommon")]
    public new float DisplayAspectRatio => base.DisplayAspectRatio;

    ///<summary>Ratio of pixel width to pixel height.</summary>
    [Description("Ratio of pixel width to pixel height."), Category("VideoImageCommon")]
    public new float PixelAspectRatio => base.PixelAspectRatio;

    #endregion

    #region VideoTextImageCommon

    ///<summary>Height in pixels.</summary>
    [Description("Height in pixels."), Category("VideoTextImageCommon")]
    public new int Height => base.Height;

    ///<summary>Width in pixels.</summary>
    [Description("Width in pixels."), Category("VideoTextImageCommon")]
    public new int Width => base.Width;

    ///<summary>Colorspace used for pixel encoding.</summary>
    [Description("Colorspace used for pixel encoding."), Category("VideoTextImageCommon")]
    public string ColorSpace => base.Colorspace;

    ///<summary>ChromaSubsampling used for pixel encoding.</summary>
    [Description("ChromaSubsampling used for pixel encoding."), Category("VideoTextImageCommon")]
    public new string ChromaSubsampling => base.ChromaSubsampling;

    #endregion
  }
}
