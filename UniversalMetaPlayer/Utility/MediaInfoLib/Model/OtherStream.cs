/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.
 * Use at your own risk, under the same license as MediaInfo itself.
 * Copyright (C) 2012 Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 ******************************************************************************
 * OtherStream.cs
 * 
 * Presents information and functionality specific to a 'other' stream.
 ******************************************************************************
 */

using System.ComponentModel;
using System.Text;

using UMP.Utility.MediaInfoLib.Library;

namespace UMP.Utility.MediaInfoLib.Model
{
  ///<summary>Represents a single 'other' stream.</summary>
  [TypeConverter(typeof(ExpandableObjectConverter))]
  public sealed class OtherStream : BaseStreamCommons
  {
    ///<summary>OtherStream constructor.</summary>
    ///<param name="mediaInfo">A MediaInfo object.</param>
    ///<param name="id">The MediaInfo ID for this other stream.</param>
    public OtherStream(Lib_MediaInfo mediaInfo, int id)
        : base(mediaInfo, StreamKind.Other, id)
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
  }
}
