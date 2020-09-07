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

    public void Add(string packageName, string projectUrl, string license, string licenseUrl) => LicenseDictionary.Add(packageName, new PackageInformation(packageName, projectUrl, license, licenseUrl));

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
    public PackageInformation(string name, string projectUrl, string license, string licenseUrl)
    {
      this.Name = name;
      this.ProjectUrl = projectUrl;
      this.License = license;
      this.LicenseUrl = licenseUrl;
    }
    /// <summary>
    /// 이름
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// 프로젝트 주소
    /// </summary>
    public string ProjectUrl { get; }
    /// <summary>
    /// 라이센스 명
    /// </summary>
    public string License { get; }
    /// <summary>
    /// 라이센스 주소
    /// </summary>
    public string LicenseUrl { get; }
  }
}
