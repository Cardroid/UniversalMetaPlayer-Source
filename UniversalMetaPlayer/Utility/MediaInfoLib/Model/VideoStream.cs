/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * VideoStream.cs
 * 
 * Presents information and functionality specific to a video stream.
 ******************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using UMP.Utility.MediaInfoLib.Library;

namespace UMP.Utility.MediaInfoLib.Model
{
  ///<summary>Represents a single video stream.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class VideoStream : BaseStreamCommons
  {
    ///<summary>VideoStream constructor</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="id">The MediaInfo ID for this audio stream.</param>
    public VideoStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.Video, id)
    { }

    /// <summary>
    /// 스트림 타입에 대한 짧은 설명을 가져옵니다
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();

      sb.AppendFormat("{0}", string.IsNullOrEmpty(this.Language) ? "Unknown" : this.Language);

      sb.AppendFormat(", {0}", this.Format);

      if (!string.IsNullOrEmpty(this.FormatProfile))
        sb.AppendFormat(" {0}", this.FormatProfile);

      sb.AppendFormat(", {0}x{1}@{2}fps ", this.Width, this.Height, this.FrameRate);

      if (!string.IsNullOrEmpty(this.Title))
        sb.AppendFormat(", '{0}'", this.Title);

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

    ///<summary>Commercial format name.</summary>
    [Description("Commercial format name for this container or stream."), Category("AllStreamsCommon")]
    public string FormatCommercial => base.Format_Commercial;

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

    #region GeneralVideoAudioTextMenu

    ///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
    [Description("Stream delay (e.g. to sync audio/video) in ms."), Category("GeneralVideoAudioTextMenu")]
    public new int Delay => base.Delay;

    ///<summary>Duration of the stream in milliseconds.</summary>
    [Description("Duration of the stream in milliseconds."), Category("GeneralVideoAudioTextMenu")]
    public new int Duration => base.Duration;

    #endregion

    #region GeneralVideoCommon

    ///<summary>Encoding application of file or stream.</summary>
    [Description("Encoding application of file or stream."), Category("GeneralVideoCommon")]
    public new string EncodedApplication => base.EncodedApplication;

    #endregion

    #region VideoAudioTextCommon

    ///<summary>The bitrate(s) of this stream, in bits per second, separated by /</summary>
    [Description("The bitrate(s) of this stream, in bits per second, separated by /"), Category("VideoAudioTextCommon")]
    public new string BitRate => base.BitRate;

    ///<summary>The maximum bitrate of this stream in BPS.</summary>
    [Description("The maximum bitrate of this stream in BPS."), Category("VideoAudioTextCommon")]
    public new int BitRateMaximum => base.BitRateMaximum;

    ///<summary>The minimum bitrate of this stream in BPS.</summary>
    [Description("The minimum bitrate of this stream in BPS."), Category("VideoAudioTextCommon")]
    public new int BitRateMinimum => base.BitRateMinimum;

    ///<summary>The maximum allowed bitrate, in BPS, with the encoder
    /// settings used. Some encoders report the average BPS.</summary>
    [Description("The maximum allowed bitrate, in BPS, with the encoder settings used. Some encoders report the average BPS."), Category("VideoAudioTextCommon")]
    public new int BitRateNominal => base.BitRateNominal;

    ///<summary>Mode (CBR, VBR) used for bit allocation.</summary>
    [Description("Mode (CBR, VBR) used for bit allocation."), Category("VideoAudioTextCommon")]
    public new string BitRateMode => base.BitRateMode;

    ///<summary>How the stream is muxed into the container.</summary>
    [Description("How the stream is muxed into the container."), Category("VideoAudioTextCommon")]
    public new string MuxingMode => base.MuxingMode;

    ///<summary>The total number of frames (e.g. video frames).</summary>
    [Description("The total number of frames (e.g. video frames)."), Category("VideoAudioTextCommon")]
    public new int FrameCount => base.FrameCount;

    ///<summary>Frame rate of the stream in frames per second.</summary>
    [Description("Frame rate of the stream in frames per second."), Category("VideoAudioTextCommon")]
    public new float FrameRate => base.FrameRate;

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

    #region VideoTextCommon

    ///<summary>Frame rate mode (CFR, VFR) of stream.</summary>
    [Description("Frame rate mode (CFR, VFR) of stream."), Category("VideoTextCommon")]
    public new string FrameRateMode => base.FrameRateMode;

    ///<summary>Scan Type - Interlaced or Progressive</summary>
    [Description("Scan Type - Interlaced or Progressive."), Category("VideoTextCommon")]
    public string ScanType => base.Scantype;

    ///<summary>Scan Order - if Interlaced, the field order</summary>
    [Description("Scan Order - if Interlaced, the field order."), Category("VideoTextCommon")]
    public string ScanOrder => base.Scanorder;

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
