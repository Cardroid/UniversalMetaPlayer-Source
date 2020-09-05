using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using UMP.Utility.MediaInfoLib;
using UMP.Utility.MediaInfoLib.Library;

namespace UnitTestProject
{
  [TestClass]
  public class InformationLoadTest
  {
    [TestMethod]
    public void MediaInfo_Uselibrary()
    {
      string result = string.Empty;

      Lib_MediaInfo MI = new Lib_MediaInfo();

      result += "Open\n\n";
      try
      {
        MI.Open(@"Test.mp3");
      }
      catch (Exception e)
      {
        MI.Close();
        result += e.ToString();
        Trace.WriteLine(result);
        Assert.IsTrue(false);
        return;
      }

      result += "Inform with Complete=false\n";
      result += $"{MI.Option("Complete")}\n";
      result += $"{MI.Inform()}\n\n";

      result += "Inform with Complete=true\n";
      result += $"{MI.Option("Complete", "1")}\n";
      result += $"{MI.Inform()}\n\n";

      //result += "Custom Inform\n";
      //result += $"{MI.Option("Inform", "General;File size is %FileSize% bytes")}\n";
      //result += $"{MI.Inform()}\n\n";

      //result += "Get with Stream=General and Parameter='FileSize'\n";
      //result += $"{MI.Get(0, 0, "FileSize")}\n\n";

      //result += "Get with Stream=General and Parameter=46\n";
      //result += $"{MI.Get(0, 0, 46)}\n\n";

      //result += "Count_Get with StreamKind=Stream_Audio\n";
      //result += $"{MI.Count_Get(StreamKind.Audio)}\n\n";

      //result += "Get with Stream=General and Parameter='AudioCount'\n";
      //result += $"{MI.Get(StreamKind.General, 0, "AudioCount")}\n\n";

      //result += "Get with Stream=Audio and Parameter='StreamCount'\n";
      //result += $"{MI.Get(StreamKind.Audio, 0, "StreamCount")}\n\n";

      //result += "Get with Stream=General and Parameter=0\n";
      //result += $"{MI.Get(StreamKind.General, 0, 0)}\n\n";

      //for (int i = 0; i < 281; i++)
      //{
      //  result += $"Get with Stream=Audio and Parameter={i}\n";
      //  for (int j = 0; j < 8; j++)
      //  {
      //    result += $"{(InfoKind)j,-14} : ";
      //    result += $"{MI.Get(StreamKind.Audio, 0, i, (InfoKind)j)}\n";
      //  }
      //  result += "\n";
      //}

      result += "Close";
      MI.Close();

      Trace.WriteLine(result);
      Assert.IsTrue(true);
    }

    [TestMethod]
    public void DisplayMediaInfo_Uselibrary()
    {
      string result = string.Empty;
      MediaFileInfo MI;

      result += "Open\n\n";
      try
      {
        MI = new MediaFileInfo(@"Test.mp3");
      }
      catch (Exception e)
      {
        result += e.ToString();
        Trace.WriteLine(result);
        Assert.Fail(result);
        return;
      }

      result += $"\n\n{MI.General.Inform()}\n\n";

      Dictionary<string,string> tmp;

      result += "*** General ***\n\n";
      tmp = GetPropertyString(MI.General);
      foreach (var item in tmp.Keys)
        result += $"{item}   : {tmp[item]}\n";

      result += "\n";

      for (int n = 0; n < MI.General.AudioCount; n++)
      {
        result += $"\n*** Audio #{n} ***\n\n";
        tmp = GetPropertyString(MI.Audio[n]);
        foreach (var item in tmp.Keys)
          result += $"{item}   : {tmp[item]}\n";
      }

      result += "\n";

      for (int n = 0; n < MI.General.TextCount; n++)
      {
        result += $"\n*** Text #{n} ***\n\n";
        tmp = GetPropertyString(MI.Text[n]);
        foreach (var item in tmp.Keys)
          result += $"{item}   : {tmp[item]}\n";
      }

      result += "\n";

      for (int n = 0; n < MI.General.ImageCount; n++)
      {
        result += $"\n*** Image #{n} ***\n\n";
        tmp = GetPropertyString(MI.Image[n]);
        foreach (var item in tmp.Keys)
          result += $"{item}   : {tmp[item]}\n";
      }

      result += "\n";

      for (int n = 0; n < MI.General.OtherCount; n++)
      {
        result += $"\n*** Other #{n} ***\n\n";
        tmp = GetPropertyString(MI.Other[n]);
        foreach (var item in tmp.Keys)
          result += $"{item}   : {tmp[item]}\n";
      }

      result += "\n";

      for (int n = 0; n < MI.General.MenuCount; n++)
      {
        result += $"\n*** Menu #{n} ***\n\n";
        tmp = GetPropertyString(MI.Menu[n]);
        foreach (var item in tmp.Keys)
          result += $"{item}   : {tmp[item]}\n";
      }

      Trace.WriteLine(result);
      Assert.IsNotNull(result);
    }

    private Dictionary<string,string> GetPropertyString(object obj)
    {
      var prop = obj.GetType().GetProperties();
      Dictionary<string, string> result = new Dictionary<string, string>();

      for (int i = 0; i < prop.Length; i++)
      {
        var value = prop[i].GetGetMethod().Invoke(obj, null);
        if (value != null)
        {
          if (prop[i].Name != "EncoderSettings")
            result[prop[i].Name] = value.ToString();
        }
      }
      return result;
    }
  }
}
