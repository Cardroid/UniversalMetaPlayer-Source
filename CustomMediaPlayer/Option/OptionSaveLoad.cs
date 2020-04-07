using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Newtonsoft.Json;
using MahApps.Metro;
using Newtonsoft.Json.Linq;

namespace CustomMediaPlayer.Option
{
    public static class OptionSaveLoad
    {
        public static string SaveFilePath { get { return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.Create) + @"\Finitio\CustomMediaPlayer\"; } }
        public static string SaveFileName { get { return $"UserSet.json"; } }

        public static OptionValue optionValue;

        public static void Reset()
        {
            optionValue = new OptionValue();
        }

        public static void Save()
        {
            if (!Directory.Exists(SaveFilePath))
                Directory.CreateDirectory(SaveFilePath);
            optionValue.Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            try
            {
                JObject SaveJ = new JObject(
                                    new JProperty("KeyHooking", optionValue.KeyHooking),
                                    new JProperty("MediaOpeningPlay", optionValue.MediaOpeningPlay),
                                    new JProperty("RepeatOption", optionValue.RepeatOption),
                                    new JProperty("Volume", optionValue.Volume),
                                    new JProperty("AccentColor", optionValue.AccentColor),
                                    new JProperty("BaseColor", optionValue.BaseColor),
                                    new JProperty("BackgroundColor", optionValue.BackgroundColor),
                                    new JProperty("LastMediaSave", optionValue.LastMediaSave),
                                    new JProperty("LastMediaPath", optionValue.LastMediaPath),
                                    new JProperty("LastMediaPostion", optionValue.LastMediaPostion),
                                    new JProperty("Version", optionValue.Version));

                File.WriteAllText(SaveFilePath + SaveFileName, SaveJ.ToString());
            }
            catch { }
        }

        public static void Load()
        {
            try
            {
                string txt;
                using (StreamReader sr = new StreamReader(SaveFilePath + SaveFileName))
                {
                    txt = sr.ReadToEnd();
                }
                var readJson = JObject.Parse(txt);
                optionValue.KeyHooking = readJson.Value<bool?>("KeyHooking")?? optionValue.KeyHooking;
                optionValue.MediaOpeningPlay = readJson.Value<bool?>("MediaOpeningPlay") ?? optionValue.MediaOpeningPlay;
                optionValue.RepeatOption = readJson.Value<int?>("RepeatOption") ?? optionValue.RepeatOption;
                optionValue.Volume = readJson.Value<int?>("Volume")?? optionValue.Volume;
                optionValue.AccentColor = readJson.Value<string>("AccentColor") ?? optionValue.AccentColor;
                optionValue.BaseColor = readJson.Value<string>("BaseColor")?? optionValue.BaseColor;
                optionValue.BackgroundColor = readJson.Value<string>("BackgroundColor") ?? optionValue.BackgroundColor;
                optionValue.LastMediaSave = readJson.Value<bool?>("LastMediaSave") ?? optionValue.LastMediaSave;
                optionValue.LastMediaPath = readJson.Value<string>("LastMediaPath") ?? optionValue.LastMediaPath;
                optionValue.LastMediaPostion = readJson.Value<double?>("LastMediaPostion") ?? optionValue.LastMediaPostion;
                optionValue.Version = readJson.Value<string>("Version") ?? FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            }
            catch { }
        }

        public class OptionValue
        {
            public bool? KeyHooking = false;
            public bool? MediaOpeningPlay = true;
            public bool? DurationViewStatus = false;
            public int? RepeatOption = 0;
            public int? Volume = 50;
            public string AccentColor = "Green";
            public string BaseColor = "BaseDark";
            public string BackgroundColor = "Black";
            public bool? LastMediaSave = false;
            public string LastMediaPath = null;
            public double? LastMediaPostion = 0;
            public string Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        }
    }

}
