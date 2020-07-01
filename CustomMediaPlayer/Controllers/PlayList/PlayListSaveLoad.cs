using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using CustomMediaPlayer.Core;

namespace CustomMediaPlayer.Controllers.PlayList
{
  public class PlayListSave
  {
    public static void Save(PlayListInfo playList)
    {
      JArray MediaPropertieArray = new JArray();
      for (int i = 0; i < playList.Count; i++)
        MediaPropertieArray.Add(MediaInfo.Serialize(playList[i]));
      JObject Jobj = new JObject
            {
                new JProperty("PlayListProperties", JToken.FromObject(playList.Serialize())),
                new JProperty("MediaProperties", MediaPropertieArray)
            };
      File.WriteAllText("PlayList.json", Jobj.ToString());
    }
  }

  public class PlayListLoad
  {
    public static PlayListInfo Load(string filepath)
    {
      if (!File.Exists(filepath))
        throw new FileNotFoundException("파일을 찾을 수 없습니다 없습니다 (File Not Found)");
      string Jsonstring = File.ReadAllText(filepath);
      if (string.IsNullOrWhiteSpace(Jsonstring))
        throw new NullReferenceException("저장된 정보가 없습니다 (File is Null)");
      JObject jObject = JObject.Parse(Jsonstring);
      PlayListInfo TargetPlayList = new PlayListInfo();

      string[] PlayListProperties = null;
      string[] MediaPropertieArray = null;
      try
      {
        PlayListProperties = jObject.Value<JArray>("PlayListProperties").ToObject<string[]>();
        MediaPropertieArray = jObject.Value<JArray>("MediaProperties").ToObject<string[]>();
        if (PlayListProperties == null || MediaPropertieArray == null)
          throw new NullReferenceException("저장된 정보가 없습니다 (File is Null)");
      }
      catch
      {
#if DEBUG
        Debug.WriteLine("재생목록 불러오기 오류 : 형변환 오류");
#endif
        throw new TypeLoadException("플레이리스트 정보 변환 실패 (TypeLoad Error)");
      }

      if (MediaPropertieArray.Length % 2 != 0)
      {
#if DEBUG
        Debug.WriteLine("재생목록 불러오기 오류 : 배열 갯수 오류");
#endif
        throw new IndexOutOfRangeException("플레이리스트 정보 변환 실패 (Indexing Error)");
      }

      //string[] playListProperties = new string[2];
      //for(int i = 0; i< playListProperties.Length; i++)
      //{
      //    playListProperties[i] = PlayListProperties[i].Value<string>();
      //}

      if (!TargetPlayList.Deserialize(PlayListProperties))
      {
#if DEBUG
        Debug.WriteLine("재생목록 불러오기 오류 : 플레이리스트 정보 변환 실패");
#endif
        throw new TypeLoadException("플레이리스트 정보 변환 실패 (Deserialize Error)");
      }
      int Count = MediaPropertieArray.Length / 2;
      string[] MediaProperties;
      for (int i = 0; i < Count; i++)
      {
        MediaProperties = new string[2];
        for (int j = 0; j < 2; j++)
        {
#if DEBUG
          Debug.WriteLine(MediaPropertieArray[i * 2 + j]);
#endif
          MediaProperties[j] = MediaPropertieArray[i * 2 + j];
        }
        if (MediaInfo.Deserialize(MediaProperties, out MediaInfo Media))
          TargetPlayList.Load(Media);
        else
        {
#if DEBUG
          Debug.WriteLine("재생목록 불러오기 오류 : 미디어 초기화 실패");
#endif
          throw new Exception("플레이리스트 정보 변환 실패 (Media Load Error)");
        }
      }
      TargetPlayList.IDRefresh();
      return TargetPlayList;
    }
  }
}
