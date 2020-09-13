/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * TextStream.cs
 * 
 * Presents information and functionality specific to a text (subtitle) stream.
 ******************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using UMP.Lib.Utility.MediaInfoLib.Reference;

namespace UMP.Lib.Utility.MediaInfoLib.Model
{
  ///<summary>Represents a single text stream.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class TextStream : BaseStreamCommons
  {
    ///<summary>TextStream constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="id">The MediaInfo ID for this text stream.</param>
    internal TextStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.Text, id)
    {
    }
    /// <summary>Overides base method to provide short summary of stream kind.</summary>
    public override string ToString()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0}", string.IsNullOrEmpty(this.Language) ? "ud" : this.Language);
      sb.AppendFormat(", {0}", this.Format);
      if (!string.IsNullOrEmpty(this.FormatProfile)) sb.AppendFormat(" {0}", this.FormatProfile);
      if (this.Default) sb.Append(", Default");
      if (this.Forced) sb.Append(", Forced");
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

    #region GeneralVideoAudioTextMenu

    ///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
    [Description("Stream delay (e.g. to sync audio/video) in ms."), Category("GeneralVideoAudioTextMenu")]
    public new int Delay => base.Delay;

    ///<summary>Duration of the stream in milliseconds.</summary>
    [Description("Duration of the stream in milliseconds."), Category("GeneralVideoAudioTextMenu")]
    public new int Duration => base.Duration;

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

    #region VideoTextCommon

    ///<summary>Frame rate mode (CFR, VFR) of stream.</summary>
    [Description("Frame rate mode (CFR, VFR) of stream."), Category("VideoTextCommon")]
    public new string FrameRateMode => base.FrameRateMode;

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

    #region AudioTextCommon

    ///<summary>The Default flag for this stream.</summary>
    [Description("The Default flag for this stream."), Category("AudioTextCommon")]
    public bool Default => base.Default_Track;

    ///<summary>The Forced-Display flag for this stream.</summary>
    [Description("The Forced-Display flag for this stream."), Category("AudioTextCommon")]
    public bool Forced => base.Forced_Track;

    #endregion
  }
}
