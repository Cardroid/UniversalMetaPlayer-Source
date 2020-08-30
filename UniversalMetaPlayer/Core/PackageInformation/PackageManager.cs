using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.PackageInformation
{
  public class PackageManager
  {
    public static PackageInformationModel GetFullPackageInfo()
    {
      PackageInformationModel packageInformationModel = new PackageInformationModel();
      packageInformationModel.Add("Costura.Fody", "https://github.com/Fody/Costura", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("Fody", "https://github.com/Fody/Fody", "MIT", "https://www.nuget.org/packages/Fody/6.2.4/license");
      packageInformationModel.Add("gong-wpf-dragdrop", "https://github.com/punker76/gong-wpf-dragdrop", "BSD 3-Clause", "https://www.nuget.org/packages/gong-wpf-dragdrop/2.2.0/license");
      packageInformationModel.Add("log4net", "http://logging.apache.org/log4net/", "Apache 2.0", "http://logging.apache.org/log4net/license.html");
      packageInformationModel.Add("MaterialDesignColors", "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("MaterialDesignExtensions", "https://spiegelp.github.io/MaterialDesignExtensions/", "MIT", "https://spiegelp.github.io/MaterialDesignExtensions/#license");
      packageInformationModel.Add("MaterialDesignThemes", "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("MediaInfo(Lib)", "https://mediaarea.net/MediaInfo", "BSD", "https://mediaarea.net/MediaInfo/License");
      packageInformationModel.Add("NAudio", "https://github.com/naudio/NAudio", "Ms-PL", "https://github.com/naudio/NAudio/blob/master/license.txt");
      packageInformationModel.Add("NAudio.Lame", "https://github.com/Corey-M/NAudio.Lame", "MIT", "https://www.nuget.org/packages/NAudio.Lame/1.1.5/license");
      packageInformationModel.Add("NeatInput.Windows", "https://github.com/LegendaryB/NeatInput", "MIT", "https://www.nuget.org/packages/NeatInput.Windows/2.0.1/license");
      packageInformationModel.Add("Newtonsoft.Json", "https://www.newtonsoft.com/json", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("NYoutubeDL", "https://gitlab.com/BrianAllred/NYoutubeDL", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("PlaylistsNET", "https://github.com/tmk907/PlaylistsNET", "MIT", "https://licenses.nuget.org/MIT");
      packageInformationModel.Add("SoundTouch(library)", "http://soundtouch.surina.net/", "LGPL v2.1", "http://soundtouch.surina.net/license.html");
      packageInformationModel.Add("TagLibSharp", "https://github.com/mono/taglib-sharp", "LGPL-2.1-only", "https://licenses.nuget.org/LGPL-2.1-only");
      packageInformationModel.Add("WindowsAPICodePack-Shell", "https://github.com/aybe/Windows-API-Code-Pack-1.1", "Custom License", "https://github.com/aybe/Windows-API-Code-Pack-1.1/blob/master/LICENCE");
      packageInformationModel.Add("XamlFlair.WPF", "https://github.com/XamlFlair/XamlFlair", "MIT", "https://github.com/XamlFlair/XamlFlair/blob/master/LICENSE");
      packageInformationModel.Add("YoutubeExplode", "https://github.com/Tyrrrz/YoutubeExplode", "LGPL-3.0-only", "https://licenses.nuget.org/LGPL-3.0-only");
      return packageInformationModel;
    }
  }
}
