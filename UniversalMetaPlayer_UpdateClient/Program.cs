using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Octokit;

namespace UMP.UpdateClient
{
  class Program
  {
    static void Main(string[] args)
    {
      GetVersion();
      Console.ReadLine();
    }

    public static async void GetVersion()
    {
      var client = new GitHubClient(new ProductHeaderValue("UniversalMetaPlayer"));

      string ver;
      Dictionary<string, string> assets = new Dictionary<string, string>();
      string latestver = string.Empty;

      Exception apierror = null;
      try
      {
        var releases = await client.Repository.Release.GetLatest("Cardroid", "UniversalMetaPlayer");

        if (releases == null)
          throw new ArgumentNullException();

        latestver = releases.TagName;

        for (int i = 0; i < releases.Assets.Count; i++)
          assets.Add(releases.Assets[i].Name, releases.Assets[i].BrowserDownloadUrl);
      }
      catch (Exception e)
      {
        apierror = e;
      }

      if (apierror != null)
      {
        Console.WriteLine("Error");
        Console.WriteLine(apierror);
        return;
      }

      Console.WriteLine(latestver);
      foreach (var item in assets.Keys)
      {
        Console.WriteLine(item);
        Console.WriteLine(assets.TryGetValue(item, out string value) ? value : "Null");
      }
    }
  }
}
