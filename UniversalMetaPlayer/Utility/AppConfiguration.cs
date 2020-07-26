using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace UMP.Utility
{
  public static class AppConfiguration
  {
    public static string Get(string key) => ConfigurationManager.AppSettings[key];

    public static void Set(string key, string value)
    {
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      KeyValueConfigurationCollection cfgCollection = config.AppSettings.Settings;

      try
      {
        cfgCollection.Remove(key);
      }
      catch { }
      cfgCollection.Add(key, value);

      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
    }

    public static void Remove(string key)
    {
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      KeyValueConfigurationCollection cfgCollection = config.AppSettings.Settings;

      try
      {
        cfgCollection.Remove(key);

        config.Save(ConfigurationSaveMode.Modified);
        ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
      }
      catch { }
    }

    public static void Clear()
    {
      Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
      KeyValueConfigurationCollection cfgCollection = config.AppSettings.Settings;

      cfgCollection.Clear();

      config.Save(ConfigurationSaveMode.Modified);
      ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
    }

  }
}
