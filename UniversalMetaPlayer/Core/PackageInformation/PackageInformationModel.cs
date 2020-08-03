using System;
using System.Collections.Generic;
using System.Text;

namespace UMP.Core.PackageInformation
{
  public class PackageInformationModel
  {
    private Dictionary<string,PackageInformation> LicenseDictionary { get; }

    public PackageInformationModel()
    {
      LicenseDictionary = new Dictionary<string, PackageInformation>();
    }

    public void Add(string packageName, string projectUrl, string license, string licenseUrl) => LicenseDictionary.Add(packageName, new PackageInformation() { Name = packageName, ProjectUrl = projectUrl, License = license, LicenseUrl = licenseUrl });

    public void Remove(string key) => LicenseDictionary.Remove(key);

    public void Clear() => LicenseDictionary.Clear();

    public int Count => LicenseDictionary.Count;
    public Dictionary<string, PackageInformation>.ValueCollection Values => LicenseDictionary.Values;
    public Dictionary<string, PackageInformation>.KeyCollection Keys => LicenseDictionary.Keys;

    public PackageInformation this[string packageName]
    {
      get => LicenseDictionary.TryGetValue(packageName, out PackageInformation value) ? value : new PackageInformation();
      set => LicenseDictionary[packageName] = value;
    }
  }

  public struct PackageInformation
  {
    /// <summary>
    /// 이름
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 프로젝트 주소
    /// </summary>
    public string ProjectUrl { get; set; }
    /// <summary>
    /// 라이센스 명
    /// </summary>
    public string License { get; set; }
    /// <summary>
    /// 라이센스 주소
    /// </summary>
    public string LicenseUrl { get; set; }
  }
}
