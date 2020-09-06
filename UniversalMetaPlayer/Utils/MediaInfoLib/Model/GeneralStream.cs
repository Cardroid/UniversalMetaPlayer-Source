/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * GeneralStream.cs
 * 
 * Presents information and functionality at the file-level.
 ******************************************************************************
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;

using UMP.Utils.MediaInfoLib.Library;

namespace UMP.Utils.MediaInfoLib.Model
{
  ///<summary>For inheritance by classes representing media files.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class GeneralStream : BaseStreamCommons
  {
    ///<summary>GeneralStream constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="id">The MediaInfo ID for this audio stream.</param>
    public GeneralStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.General, id)
    {
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

    #region GeneralVideoCommon

    ///<summary>Encoding application of file or stream.</summary>
    [Description("Encoding application of file or stream."), Category("GeneralVideoCommon")]
    public new string EncodedApplication => base.EncodedApplication;

    #endregion

    #region General

    private string _EncodedBy;
    ///<summary>Name of the person/group who encoded this file.</summary>
    [Description("Name of the person/group who encoded this file."), Category("General")]
    public string EncodedBy
    {
      get
      {
        if (_EncodedBy == null)
          _EncodedBy = GetString("EncodedBy");
        return _EncodedBy;
      }
    }

    private string _Album;
    ///<summary>Album name, if the file represents an album.</summary>
    [Description("Album name, if the file represents an album."), Category("General")]
    public string Album
    {
      get
      {
        if (_Album == null)
          _Album = GetString("Album");
        return _Album;
      }
    }

    private string _Grouping = null;
    ///<summary>The grouping used by iTunes.</summary>
    [Description("The grouping used by iTunes."), Category("General")]
    public string Grouping
    {
      get
      {
        if (_Grouping == null)
          _Grouping = GetString("Grouping");
        return _Grouping;
      }
    }

    private string _Compilation = null;
    ///<summary>The compilation used by iTunes.</summary>
    [Description("The compilation used by iTunes."), Category("General")]
    public string Compilation
    {
      get
      {
        if (_Compilation == null)
          _Compilation = GetString("Compilation");
        return _Compilation;
      }
    }

    private int _bitRate = int.MinValue;
    ///<summary>Overall bitrate of all streams.</summary>
    [Description("Overall bitrate of all streams."), Category("General")]
    public new int BitRate
    {
      get
      {
        if (_bitRate == int.MinValue)
          _bitRate = GetInt("OverallBitRate");
        return _bitRate;
      }
    }

    private int _bitRateMaximum = int.MinValue;
    ///<summary>Maximum overall bitrate of all streams.</summary>
    [Description("Maximum overall bitrate of all streams."), Category("General")]
    public new int BitRateMaximum
    {
      get
      {
        if (_bitRateMaximum == int.MinValue)
          _bitRateMaximum = GetInt("OverallBitRate_Maximum");
        return _bitRateMaximum;
      }
    }

    private int _bitRateMinimum = int.MinValue;
    ///<summary>Minimum overall bitrate of all streams.</summary>
    [Description("Minimum overall bitrate of all streams."), Category("General")]
    public new int BitRateMinimum
    {
      get
      {
        if (_bitRateMinimum == int.MinValue)
          _bitRateMinimum = GetInt("OverallBitRate_Minimum");
        return _bitRateMinimum;
      }
    }

    private int _bitRateNominal = int.MinValue;
    ///<summary>Maximum allowed overall bitrate of all streams.</summary>
    [Description("Maximum allowed overall bitrate of all streams."), Category("General")]
    public new int BitRateNominal
    {
      get
      {
        if (_bitRateNominal == int.MinValue)
          _bitRateNominal = GetInt("OverallBitRate_Nominal");
        return _bitRateNominal;
      }
    }

    #endregion

    #region General/Counts

    private int _videoCount = int.MinValue;
    ///<summary>Number of video streams in this file.</summary>
    [Description("Number of video streams."), Category("General/Counts")]
    public int VideoCount
    {
      get
      {
        if (_videoCount == int.MinValue)
          _videoCount = GetInt("VideoCount");
        return _videoCount;
      }
    }

    private int _audioCount = int.MinValue;
    ///<summary>Number of audio streams in this file.</summary>
    [Description("Number of audio streams."), Category("General/Counts")]
    public int AudioCount
    {
      get
      {
        if (_audioCount == int.MinValue)
          _audioCount = GetInt("AudioCount");
        return _audioCount;
      }
    }

    private int _textCount = int.MinValue;
    ///<summary>Number of subtitles or other texts in this file.</summary>
    [Description("Number of text streams."), Category("General/Counts")]
    public int TextCount
    {
      get
      {
        if (_textCount == int.MinValue)
          _textCount = GetInt("TextCount");
        return _textCount;
      }
    }

    private int _imageCount = int.MinValue;
    ///<summary>Number of images in this file.</summary>
    [Description("Number of image streams."), Category("General/Counts")]
    public int ImageCount
    {
      get
      {
        if (_imageCount == int.MinValue)
          _imageCount = GetInt("ImageCount");
        return _imageCount;
      }
    }

    private int _otherCount = int.MinValue;
    ///<summary>Number of others in this file.</summary>
    [Description("Number of other streams."), Category("General/Counts")]
    public int OtherCount
    {
      get
      {
        if (_otherCount == int.MinValue)
          _otherCount = GetInt("OtherCount");
        return _otherCount;
      }
    }

    private int _menuCount = int.MinValue;
    ///<summary>Number of menu streams in this file.</summary>
    [Description("Number of menu streams."), Category("General/Counts")]
    public int MenuCount
    {
      get
      {
        if (_menuCount == int.MinValue)
          _menuCount = GetInt("MenuCount");
        return _menuCount;
      }
    }

    #endregion
  }
}
