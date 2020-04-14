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
        PlayListInfo TargetPlayList;

        public PlayListSave(PlayListInfo playList)
        {
            TargetPlayList = playList;
        }

        public void Save()
        {
            JArray MediaPropertieArray = new JArray();
            for (int i = 0; i < TargetPlayList.Count; i++)
            {
                MediaPropertieArray.Add(TargetPlayList[i].Serialize());
            }
            JObject Jobj = new JObject
            {
                new JProperty("PlayListProperties", JToken.FromObject(TargetPlayList.Serialize())),
                new JProperty("MediaProperties", MediaPropertieArray)
            };
            File.WriteAllText("PlayList.json", Jobj.ToString());
        }
    } 

    public class PlayListLoad
    {
        string Jsonstring;

        public PlayListLoad(string filepath)
        {
            Jsonstring = File.ReadAllText(filepath);
        }

        public void Load()
        {
            JObject jObject = JObject.Parse(Jsonstring);
            PlayListInfo TargetPlayList = new PlayListInfo();

            string[] PlayListProperties = null;
            string[] MediaPropertieArray = null;
            try
            {
                PlayListProperties = jObject.Value<JArray>("PlayListProperties").ToObject<string[]>();
                MediaPropertieArray = jObject.Value<JArray>("MediaProperties").ToObject<string[]>();
            }
            catch
            {
#if DEBUG
                Debug.WriteLine("재생목록 불러오기 오류 : 형변환 오류");
#endif
                return;
            }

            if (MediaPropertieArray.Length % 3 != 0)
            {
#if DEBUG
                Debug.WriteLine("재생목록 불러오기 오류 : 배열 갯수 오류");
#endif
                return;
            }

            //string[] playListProperties = new string[2];
            //for(int i = 0; i< playListProperties.Length; i++)
            //{
            //    playListProperties[i] = PlayListProperties[i].Value<string>();
            //}

            if(!TargetPlayList.Deserialize(PlayListProperties))
            {
#if DEBUG
                Debug.WriteLine("재생목록 불러오기 오류 : 플레이리스트 정보 로드 실패");
#endif
                return;
            }
            int Count = MediaPropertieArray.Length / 3;
            string[] MediaProperties;
            for (int i = 0; i < Count; i++)
            {
                MediaProperties = new string[3];
                for (int j = 0; j < 3; j++)
                {
#if DEBUG
                    Debug.WriteLine(MediaPropertieArray[i*3 + j]);
#endif
                    MediaProperties[j] = MediaPropertieArray[i*3 + j];
                }
                MediaInfo Media = new MediaInfo(null);
                if (Media.Deserialize(MediaProperties))
                    TargetPlayList.Load(Media);
                else
#if DEBUG
                    Debug.WriteLine("재생목록 불러오기 오류 : 미디어 초기화 실패");
#endif
            }
            TargetPlayList.IDRefresh();
            MainWindow.PlayList.Playlist = TargetPlayList;
        }
    }
}
