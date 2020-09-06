/******************************************************************************
 * MediaInfo.NET - A fast, easy-to-use .NET wrapper for MediaInfo.dll
 * 
 * New versions available from http://code.google.com/p/mediainfo-dot-net/
 * or from http://git.vahanus.net/?p=csc/MediaInfoDotNet.git
 * 
 * Use at your own risk, under the same license as MediaInfo itself.
 * 
 * If you want to make a donation to this project, instead make it to the 
 * MediaInfo project at: http://mediainfo.sourceforge.net
 * 
 * Copyright (C) Charles N. Burns
 * Copyright (C) 2013 Carsten Schlote
 * 
 ******************************************************************************
 * 
 * MediaInfo.cs
 * 
 * Library entrypoint.
 * 
 * To make this work in your .NET project:
 * 1) Add this DLL, MediaInfoDotNet.dll, to your project's "references"
 * 2) Copy MediaInfo.DLL into each subfolder of your project's "bin\" folder
 *    You can download it from http://mediainfo.sourceforge.net
 *    Do not try to add MediaInfo.dll to your "references". Wrong type of DLL.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;

using UMP.Utils.MediaInfoLib.Library;
using UMP.Utils.MediaInfoLib.Model;

namespace UMP.Utils.MediaInfoLib
{
  public sealed class MediaFileInfo
  {
    /// <summary>
    /// 새 미디어 파일정보 객체를 생성합니다
    /// </summary>
    /// <param name="filePath">파일의 경로</param>
    public MediaFileInfo(string filePath)
    {
      if (string.IsNullOrWhiteSpace(filePath))
        throw new ArgumentNullException("파일 이름이 Null입니다");

      string pathResult;
      try { pathResult = Path.GetFullPath(filePath); }
      catch { throw; }

      this.MediaInfo = new Lib_MediaInfo();
      MediaInfo.Open(pathResult);
      this.FilePath = pathResult;
    }

    /// <summary>
    /// 파일의 경로
    /// </summary>
    [Description("파일의 경로"), Category("MediaFile")]
    public string FilePath { get; private set; }

    /// <summary>
    /// 이 객체의 모든 주석
    /// </summary>
    [Description("이 객체의 모든 주석"), Category("MediaFile")]
    public string Inform
    {
      get
      {
        General.Option("Complete", _InformComplete ? "1" : string.Empty);
        return General.Inform();
      }
    }

    private bool _InformComplete;
    /// <summary>
    /// 이 객체의 모든 주석의 사용가능 여부
    /// </summary>
    [Description("이 객체의 모든 주석의 사용가능 여부"), Category("MediaFile")]
    public bool InformComplete
    {
      get
      {
        //return String.IsNullOrEmpty(General.miOption("Complete")) ? true : false;
        return _InformComplete;
      }
      set => _InformComplete = value;
    }

    /// <summary>
    /// MediaInfo.dll의 Parameter값 주석
    /// </summary>
    [Description("MediaInfo.dll의 Parameter값 주석"), Category("MediaFile")]
    public string InfoParameters => General.Option("Info_Parameters");

    /// <summary>
    /// MediaInfo.dll의 알려진 코덱 주석
    /// </summary>
    [Description("MediaInfo.dll의 알려진 코덱 주석"), Category("MediaFile")]
    public string InfoCodecs => General.Option("Info_Codecs");

    /// <summary>
    /// MediaInfo.dll의 버전
    /// </summary>
    [Description("MediaInfo.dll의 버전"), Category("MediaFile")]
    public string InfoVersion => General.Option("Info_Version");

    /// <summary>
    /// MediaInfo.dll의 프로젝트 Url
    /// </summary>
    [Description("MediaInfo.dll의 프로젝트 Url"), Category("MediaFile")]
    public string InfoUrl => General.Option("Info_Url");

    private readonly Lib_MediaInfo MediaInfo;
    private GeneralStream _General;
    /// <summary>
    /// 기본 정보 스트림
    /// </summary>
    [Description("기본 정보 스트림"), Category("Streams")]
    public GeneralStream General
    {
      get
      {
        if (_General == null)
          _General = new GeneralStream(MediaInfo, 0);
        return _General;
      }
    }

    /// <summary>
    /// 파일에 하나 이상의 스트림이 존재하면 true
    /// </summary>
    [Description("파일에 하나 이상의 스트림이 존재하면 true"), Category("Streams")]
    public bool HasStreams => General.VideoCount > 0 || General.AudioCount > 0 || General.TextCount > 0 || General.OtherCount > 0 || General.ImageCount > 0 || General.MenuCount > 0;

    private List<VideoStream> _VideoStreams;
    /// <summary>
    /// 비디오 스트림
    /// </summary>
    [Description("비디오 스트림"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<VideoStream>))]
    public List<VideoStream> Video
    {
      get
      {
        if (_VideoStreams == null)
        {
          _VideoStreams = new List<VideoStream>(General.VideoCount);
          for (int id = 0; id < General.VideoCount; ++id)
            _VideoStreams.Add(new VideoStream(MediaInfo, id));
        }
        return _VideoStreams;
      }
    }

    private List<AudioStream> _AudioStreams;
    /// <summary>
    /// 오디오 스트림
    /// </summary>
    [Description("오디오 스트림"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<AudioStream>))]
    public List<AudioStream> Audio
    {
      get
      {
        if (_AudioStreams == null)
        {
          _AudioStreams = new List<AudioStream>(General.AudioCount);
          for (int id = 0; id < General.AudioCount; ++id)
            _AudioStreams.Add(new AudioStream(MediaInfo, id));
        }
        return _AudioStreams;
      }
    }

    private List<TextStream> _TextStreams;
    /// <summary>
    /// 텍스트 스트림
    /// </summary>
    [Description("텍스트 스트림"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<TextStream>))]
    public List<TextStream> Text
    {
      get
      {
        if (_TextStreams == null)
        {
          _TextStreams = new List<TextStream>(General.TextCount);
          for (int id = 0; id < General.TextCount; ++id)
            _TextStreams.Add(new TextStream(MediaInfo, id));
        }
        return _TextStreams;
      }
    }

    private List<ImageStream> _ImageStreams;
    /// <summary>
    /// 이미지 스트림
    /// </summary>
    [Description("이미지 스트림"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<ImageStream>))]
    public List<ImageStream> Image
    {
      get
      {
        if (_ImageStreams == null)
        {
          _ImageStreams = new List<ImageStream>(General.ImageCount);
          for (int id = 0; id < General.ImageCount; ++id)
            _ImageStreams.Add(new ImageStream(MediaInfo, id));
        }
        return _ImageStreams;
      }
    }

    private List<OtherStream> _OtherStreams;
    /// <summary>
    /// 기타 스트림
    /// </summary>
    [Description("기타 스트림 (e.g. chapters)"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<OtherStream>))]
    public List<OtherStream> Other
    {
      get
      {
        if (_OtherStreams == null)
        {
          _OtherStreams = new List<OtherStream>(General.OtherCount);
          for (int id = 0; id < General.OtherCount; ++id)
            _OtherStreams.Add(new OtherStream(MediaInfo, id));
        }
        return _OtherStreams;
      }
    }

    private List<MenuStream> _MenuStreams;
    /// <summary>
    /// 메뉴 스트림
    /// </summary>
    [Description("메뉴 스트림"), Category("Streams")]
    [TypeConverter(typeof(StreamListConverter<MenuStream>))]
    public List<MenuStream> Menu
    {
      get
      {
        if (_MenuStreams == null)
        {
          _MenuStreams = new List<MenuStream>(General.MenuCount);
          for (int id = 0; id < General.MenuCount; ++id)
            _MenuStreams.Add(new MenuStream(MediaInfo, id));
        }
        return _MenuStreams;
      }
    }
  }

  /// <summary>
  /// TypeConverter for the generic lists above. We don't need an editor or setting of a value.<br/>
  /// The list item are expanded into virtial Poperties for the PropertyGrid.
  /// </summary>
  /// <typeparam name="ItemType"></typeparam>
  public class StreamListConverter<ItemType> : ExpandableObjectConverter
  {
    /// <summary>
    /// Return true, if we can handle this detinationType, otherwise call base class.
    /// <see cref="TypeConverter.CanConvertTo(ITypeDescriptorContext, Type)"/>
    /// </summary>
    public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
    {
      if (destinationType == typeof(List<ItemType>))
        return true;
      return base.CanConvertTo(context, destinationType);
    }
    /// <summary>
    /// Convert the list object into some humanreadble string...
    /// <see cref="TypeConverter.ConvertTo(object, Type)"/>
    /// </summary>
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
      if (destinationType == typeof(string))
      {
        List<ItemType> list = (List<ItemType>)value;
        return "There are " + list.Count + " streams of type " + typeof(ItemType).Name;
      }
      return base.ConvertTo(context, culture, value, destinationType);
    }
    /// <summary>
    /// Return true, if we can handle sourceType. 
    /// We can'T, because item are static and can't be changed.
    /// <see cref="TypeConverter.CanConvertFrom(ITypeDescriptorContext, Type)"/>
    /// </summary>
    public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
    {
      //if (sourceType == typeof(string))
      //	return true;
      return base.CanConvertFrom(context, sourceType);
    }
    /// <summary>
    /// Return true, if we can handle sourceType. 
    /// We can'T, because item are static and can't be changed.
    /// <see cref="TypeConverter.ConvertFrom(ITypeDescriptorContext, CultureInfo, object)"/>
    /// </summary>
    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
      //if (value is string) {      // Nothing useful to do here.
      //}  
      return base.ConvertFrom(context, culture, value);
    }

    /// <summary>
    /// Custom GetProperties method. Splits the List items into separate virtual Properties.
    /// <see cref="TypeConverter.GetProperties(ITypeDescriptorContext, object, Attribute[])"/>
    /// </summary>
    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attrs)
    {
      // our list of props.
      ArrayList propList = new ArrayList();

      // add a property descriptor for each stream
      for (int i = 0; i < ((List<ItemType>)component).Count; i++)
        propList.Add(new StreamListPropertyDescriptor<ItemType>(((List<ItemType>)component), i));

      // return the collection of PropertyDescriptors.
      PropertyDescriptor[] props = (PropertyDescriptor[])propList.ToArray(typeof(PropertyDescriptor));
      return new PropertyDescriptorCollection(props);
    }

    /// <summary>
    /// This PropertyDescriptor is used to expand the internal lists of stream into separate virtual
    /// properties for the stream. So the stream can be expanded in the property grid.
    /// </summary>
    private class StreamListPropertyDescriptor<StreamItemType> : PropertyDescriptor
    {
      private readonly List<StreamItemType> owner;
      private readonly int index;
      //System.Drawing.Design.UITypeEditor editor;

      /// <summary>
      /// Initialize our state.
      /// </summary>
      /// <param name="owner">The PropertyTab that created this Property</param>
      /// <param name="index">The vertex this PropertyDescriptor operates on.</param>
      public StreamListPropertyDescriptor(List<StreamItemType> owner, int index) :
          base("Stream " + index, new Attribute[] { CategoryAttribute.Data })
      {
        this.owner = owner;
        this.index = index;
      }

      /// <summary>
      /// The type of component the framework expects for this property.  Notice
      /// This returns List'StreamItemType.  That is because the object that is being browsed
      /// when this property is shown is a List'StreamItemType.  So we are faking the PropertyGrid
      /// into thinking this is a property on that type, even though it isn't.
      /// </summary>	
      public override Type ComponentType => typeof(List<StreamItemType>);

      /// <summary>
      /// Must override abstract properties.
      /// </summary>
      public override bool IsReadOnly => true;

      /// <summary>
      /// This property is a StreamItemType type property.
      /// </summary>
      public override Type PropertyType => typeof(StreamItemType);

      /// <summary>
      /// This allows us to specify the editor that will be used for this
      /// property.
      /// </summary>
      /// <param name="editorBaseType"></param>
      /// <returns></returns>
      public override object GetEditor(Type editorBaseType)
      {
        // make sure we're looking for a UITypeEditor.
        //
        //if (editorBaseType == typeof(System.Drawing.Design.UITypeEditor)) {
        //	// create and return one of our editors.
        //	//
        //	if (editor == null) {
        //		editor = new PointUIEditor(owner.target);
        //	}
        //	return editor;

        //}
        return base.GetEditor(editorBaseType);
      }


      /// <summary>
      /// Gets the value for the StreamList
      /// </summary>
      public override object GetValue(object o)
      {
        // Just as the the underlaying List<> for the item
        return ((List<StreamItemType>)o)[index];
      }

      /// <summary>
      /// Thsi routine should normale set a value from some object.
      /// Obviously we can't change the the data. So just do nothing.
      /// </summary>
      public override void SetValue(object o, object value) { }

      /// <summary>
      /// Abstract base members
      /// </summary>			
      public override void ResetValue(object o) { }

      public override bool CanResetValue(object o) => false;

      public override bool ShouldSerializeValue(object o) => false;
    }
  }
}
