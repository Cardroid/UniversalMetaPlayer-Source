/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * MultiStreamCommon.cs
 *
 * Provides methods common to more then one stream type
 ******************************************************************************
 */

using System;
using System.Collections.Generic;

using UMP.Utility.MediaInfoLib.Library;

namespace UMP.Utility.MediaInfoLib.Model
{
  /// <summary>Functionality common to more than one stream type.</summary>
  public class BaseStreamCommons : Media
  {
    ///<summary>MultiStreamCommon constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="kind">A MediaInfo StreamKind.</param>
    ///<param name="id">The MediaInfo ID for this audio stream.</param>
    public BaseStreamCommons(Lib_MediaInfo mediaInfo, StreamKind kind, int id)
        : base(mediaInfo, kind, id)
    { }

    #region AllStreamsCommon

    private int _StreamID = int.MinValue;
    ///<summary>스트림 번호</summary>
    public int StreamID
    {
      get
      {
        if (_StreamID == int.MinValue)
          _StreamID = GetInt("ID");
        return _StreamID;
      }
    }

    private string _Format;
    ///<summary>컨테이너 또는 스트림의 포멧</summary>
    public string Format
    {
      get
      {
        if (_Format == null)
          _Format = GetString("Format");
        return _Format;
      }
    }

    private string _Format_Info;
    ///<summary>Additional format information for this stream.</summary>
    public string Format_Info
    {
      get
      {
        if (_Format_Info == null)
          _Format_Info = GetString("Format/Info");
        return _Format_Info;
      }
    }

    private string _Format_Profile;
    ///<summary>Additional format information for this stream.</summary>
    public string Format_Profile
    {
      get
      {
        if (_Format_Profile == null)
          _Format_Profile = GetString("Format_Profile");
        return _Format_Profile;
      }
    }

    private string _Format_Version;
    ///<summary>Additional format information for this stream.</summary>
    public string Format_Version
    {
      get
      {
        if (_Format_Version == null)
          _Format_Version = GetString("Format_Version");
        return _Format_Version;
      }
    }

    private string _Format_Commercial;
    ///<summary>Commercial format name.</summary>
    public string Format_Commercial
    {
      get
      {
        if (_Format_Commercial == null)
          _Format_Commercial = GetString("Format_Commercial");
        return _Format_Commercial;
      }
    }

    private string _Title;
    ///<summary>컨테이너 또는 스트림의 제목</summary>
    public string Title
    {
      get
      {
        if (_Title == null)
          _Title = GetString("Title"); //FIXME Why must this be uppercase? Bug?
        if (string.IsNullOrEmpty(_Title))
        {
          _Title = GetString("TITLE"); //FIXME Why must this be uppercase? Bug?
        }
        return _Title;
      }
    }

    private string _UniqueID;
    ///<summary>This stream's globally unique ID (GUID).</summary>
    public string UniqueID
    {
      get
      {
        if (_UniqueID == null)
          _UniqueID = GetString("UniqueID");
        return _UniqueID;
      }
    }

    #endregion

    #region GeneralVideoCommon

    private string _EncodedApplication;
    /// <summary>Encoding application for this file or stream.</summary>
    public string EncodedApplication
    {
      get
      {
        if (string.IsNullOrEmpty(_EncodedApplication))
          _EncodedApplication = GetString("Encoded_Application");
        return _EncodedApplication;
      }
    }

    #endregion

    #region GeneralVideoAudioTextImageCommon

    private DateTime _EncodedDate = DateTime.MinValue;
    ///<summary>Date and time stream encoding completed.</summary>
    public DateTime EncodedDate
    {
      get
      {
        if (_EncodedDate == DateTime.MinValue)
          _EncodedDate = GetDateTime("Encoded_Date");
        return _EncodedDate;
      }
    }

    private string _EncoderLibrary = null;
    ///<summary>Software used to encode this stream.</summary>
    public string EncoderLibrary
    {
      get
      {
        if (_EncoderLibrary == null)
          _EncoderLibrary = GetString("Encoded_Library");
        return _EncoderLibrary;
      }
    }

    private string _InternetMediaType = null;
    ///<summary>Media type of stream, formerly called MIME type.</summary>
    public string InternetMediaType
    {
      get
      {
        if (_InternetMediaType == null)
          _InternetMediaType = GetString("InternetMediaType");
        return _InternetMediaType;
      }
    }

    private long _Size = long.MinValue;
    ///<summary>Size in bytes.</summary>
    public long Size
    {
      get
      {
        if (_Size == long.MinValue)
        {
          if (Kind == StreamKind.General)
            _Size = GetLong("FileSize");
          else
            _Size = GetLong("StreamSize");
        }
        return _Size;
      }
    }

    private string _EncoderSettingsRaw = null;
    ///<summary>Encoder settings used for encoding this stream.
    ///String format: name=value / name=value / ...</summary>
    public string EncoderSettingsRaw
    {
      get
      {
        if (_EncoderSettingsRaw == null)
          _EncoderSettingsRaw
              = GetString("Encoded_Library_Settings");
        return _EncoderSettingsRaw;
      }
    }

    private IDictionary<string, string> _EncoderSettings = null;
    ///<summary>Encoder settings used for encoding this stream.</summary>
    public IDictionary<string, string> EncoderSettings
    {
      get
      {
        if (_EncoderSettings == null)
        {
          _EncoderSettings = new Dictionary<string, string>();
          string settings = EncoderSettingsRaw;
          foreach (var setting in settings.Split('/'))
          {
            var keyValue = setting.Trim().Split('=');
            if (keyValue.Length == 2)
            {
              if (!_EncoderSettings.ContainsKey(keyValue[0]))
                _EncoderSettings.Add(keyValue[0], keyValue[1]);
            }
          }
        }
        return _EncoderSettings;
      }
    }

    #endregion

    #region GeneralVideoAudioTextImageMenuCommon

    private string _CodecId = null;
    ///<summary>Codec ID available from some codecs.</summary>
    ///<example>AAC audio:A_AAC, h.264 video:V_MPEG4/ISO/AVC</example>
    public string CodecId
    {
      get
      {
        if (_CodecId == null)
          _CodecId = GetString("CodecID");
        return _CodecId;
      }
    }

    private string _CodecCommonName = null;
    ///<summary>Common name of the codec.</summary>
    public string CodecCommonName
    {
      get
      {
        if (_CodecCommonName == null)
          _CodecCommonName = GetString("CodecID/Hint");
        return _CodecCommonName;
      }
    }

    #endregion

    #region GeneralVideoAudioTextMenu

    private int _Delay = int.MinValue;
    ///<summary>Stream delay (e.g. to sync audio/video) in ms.</summary>
    public int Delay
    {
      get
      {
        if (_Delay == int.MinValue)
          _Delay = GetInt("Delay");
        return _Delay;
      }
    }

    private int _Duration = int.MinValue;
    ///<summary>Duration of the stream in milliseconds.</summary>
    public int Duration
    {
      get
      {
        if (_Duration == int.MinValue)
          _Duration = GetInt("Duration");
        return _Duration;
      }
    }

    #endregion

    #region VideoAudioTextCommon

    private string _BitRate = null;
    ///<summary>The bit rate of this stream, in bits per second</summary>
    public string BitRate
    {
      get
      {
        if (string.IsNullOrEmpty(_BitRate))
          _BitRate = GetString("BitRate");
        return _BitRate;
      }
    }

    private int _BitRateMaximum = int.MinValue;
    ///<summary>The maximum bitrate of this stream in BPS.</summary>
    public int BitRateMaximum
    {
      get
      {
        if (_BitRateMaximum == int.MinValue)
          _BitRateMaximum = GetInt("BitRate_Maximum");
        return _BitRateMaximum;
      }
    }

    private int _BitRateMinimum = int.MinValue;
    ///<summary>The minimum bitrate of this stream in BPS.</summary>
    public int BitRateMinimum
    {
      get
      {
        if (_BitRateMinimum == int.MinValue)
          _BitRateMinimum = GetInt("BitRate_Minimum");
        return _BitRateMinimum;
      }
    }

    private int _BitRateNominal = int.MinValue;
    ///<summary>The maximum allowed bitrate, in BPS, with the encoder
    /// settings used. Some encoders report the average BPS.</summary>
    public int BitRateNominal
    {
      get
      {
        if (_BitRateNominal == int.MinValue)
          _BitRateNominal = GetInt("BitRate_Nominal");
        return _BitRateNominal;
      }
    }

    private string _BitRateMode = null;
    ///<summary>Mode (CBR, VBR) used for bit allocation.</summary>
    public string BitRateMode
    {
      get
      {
        if (_BitRateMode == null)
          _BitRateMode = GetString("BitRate_Mode");
        return _BitRateMode;
      }
    }

    private string _MuxingMode = null;
    ///<summary>How the stream is muxed into the container.</summary>
    public string MuxingMode
    {
      get
      {
        if (_MuxingMode == null)
          _MuxingMode = GetString("MuxingMode");
        return _MuxingMode;
      }
    }

    private int _FrameCount = int.MinValue;
    ///<summary>The total number of frames (e.g. video frames).</summary>
    public int FrameCount
    {
      get
      {
        if (_FrameCount == int.MinValue)
          _FrameCount = GetInt("FrameCount");
        return _FrameCount;
      }
    }

    private float _FrameRate = float.MinValue;
    ///<summary>Frame rate of the stream in frames per second.</summary>
    public float FrameRate
    {
      get
      {
        if (_FrameRate == float.MinValue)
          _FrameRate = GetFloat("FrameRate");
        return _FrameRate;
      }
    }

    #endregion

    #region VideoAudioTextImageCommon

    private string _CompressionMode = null;
    ///<summary>Compression mode (lossy or lossless).</summary>
    public string CompressionMode
    {
      get
      {
        if (_CompressionMode == null)
          _CompressionMode = GetString("Compression_Mode");
        return _CompressionMode;
      }
    }

    private string _CompressionRatio = null;
    ///<summary>Ratio of current size to uncompressed size.</summary>
    public string CompressionRatio
    {
      get
      {
        if (_CompressionRatio == null)
          _CompressionRatio = GetString("Compression_Ratio");
        //FIXME Never seen in tests. Derive from Streamsize_* parameters?
        return _CompressionRatio;
      }
    }

    private int _BitDepth = int.MinValue;
    ///<example>Stream bit depth (16, 24, 32...)</example>
    public int BitDepth
    {
      get
      {
        if (_BitDepth == int.MinValue)
          _BitDepth = GetInt("BitDepth");
        return _BitDepth;
      }
    }

    #endregion

    #region VideoAudioTextImageMenuCommon

    private string _Language = null;
    ///<summary>2-letter (if available) or 3-letter ISO code.</summary>
    public string Language
    {
      get
      {
        if (_Language == null)
          _Language = GetString("Language");
        return _Language;
      }
    }

    #endregion

    #region VideoImageCommon

    private float _DisplayAspectRatio = float.MinValue;
    ///<summary>Ratio of pixel width to pixel height.</summary>
    public float DisplayAspectRatio
    {
      get
      {
        if (_DisplayAspectRatio == float.MinValue)
          _DisplayAspectRatio = GetFloat("DisplayAspectRatio");
        return _DisplayAspectRatio;
      }
    }

    private float _PixelAspectRatio = float.MinValue;
    ///<summary>Ratio of pixel width to pixel height.</summary>
    public float PixelAspectRatio
    {
      get
      {
        if (_PixelAspectRatio == float.MinValue)
          _PixelAspectRatio = GetFloat("PixelAspectRatio");
        return _PixelAspectRatio;
      }
    }

    #endregion

    #region VideoTextCommon

    private string _FrameRateMode = null;
    ///<summary>Frame rate mode (CFR, VFR) of stream.</summary>
    public string FrameRateMode
    {
      get
      {
        if (_FrameRateMode == null)
          _FrameRateMode = GetString("FrameRate_Mode");
        return _FrameRateMode;
      }
    }

    #endregion

    #region VideoTextImageCommon

    private int _Height = int.MinValue;
    ///<summary>Height in pixels.</summary>
    public int Height
    {
      get
      {
        if (_Height == int.MinValue)
          _Height = GetInt("Height");
        return _Height;
      }
    }

    private int _Width = int.MinValue;
    ///<summary>Width in pixels.</summary>
    public int Width
    {
      get
      {
        if (_Width == int.MinValue)
          _Width = GetInt("Width");
        return _Width;
      }
    }

    private string _ColorSpace;
    ///<summary>Colorspace used for encoding.</summary>
    public string Colorspace
    {
      get
      {
        if (string.IsNullOrEmpty(_ColorSpace))
          _ColorSpace = GetString("ColorSpace");
        return _ColorSpace;
      }
    }

    private string _ChromaSubsampling;
    ///<summary>Colorspace used for encoding.</summary>
    public string ChromaSubsampling
    {
      get
      {
        if (string.IsNullOrEmpty(_ChromaSubsampling))
          _ChromaSubsampling = GetString("ChromaSubsampling");
        return _ChromaSubsampling;
      }
    }

    private string _Scantype;
    ///<summary>Scan Type - Interlaced or Progressive</summary>
    public string Scantype
    {
      get
      {
        if (string.IsNullOrEmpty(_Scantype))
          _Scantype = GetString("ScanType");
        return _Scantype;
      }
    }

    private string _Scanorder;
    ///<summary>Scan Order - if Interlaced, the field order</summary>
    public string Scanorder
    {
      get
      {
        if (string.IsNullOrEmpty(_Scanorder))
          _Scanorder = GetString("ScanOrder");
        return _Scanorder;
      }
    }
    #endregion

    #region AudioTextCommon

    private string _Default;
    ///<summary>Colorspace used for encoding.</summary>
    public bool Default_Track
    {
      get
      {
        if (string.IsNullOrEmpty(_Default))
          _Default = GetString("Default");
        return string.Compare(_Default, "Yes", true) == 0;
      }
    }

    private string _Forced;
    ///<summary>Colorspace used for encoding.</summary>
    public bool Forced_Track
    {
      get
      {
        if (string.IsNullOrEmpty(_Forced))
          _Forced = GetString("Forced");
        return string.Compare(_Forced, "Yes", true) == 0;
      }
    }

    #endregion

  }
}
