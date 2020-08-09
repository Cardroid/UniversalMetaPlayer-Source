using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using UMP.Core;
using UMP.Core.PackageInformation;
using UMP.Utility;

namespace UMP.Controller.Option.OptionControl
{
  /// <summary>
  /// Information.xaml에 대한 상호 작용 논리
  /// </summary>
  public partial class InformationOption : UserControl
  {
    public InformationOption()
    {
      InitializeComponent();

      this.Mainlogo.Width = 120;
      this.Mainlogo.Height = 120;
      this.Mainlogo.Source = GlobalProperty.StaticValues.LogoImage;

      this.CoreVersionTitleLabel.Content = "코어 버전 : ";
      this.FileVersionTitleLabel.Content = "파일 버전 : ";
      this.BitVersionTitleLabel.Content = "Bit : ";
      this.CoreVersionLabel.Content = GlobalProperty.StaticValues.CoreVersion;
      this.FileVersionLabel.Content = GlobalProperty.StaticValues.FileVersion;
      this.BitVersionLabel.Content = GlobalProperty.StaticValues.BitVersion;

      this.ProjectRepositoryUrl.NavigateUri = new Uri("https://github.com/Cardroid/UniversalMetaPlayer");
      this.ProjectRepositoryUrl.ToolTip = "https://github.com/Cardroid/UniversalMetaPlayer 으로 이동합니다.";
      this.ProjectRepositoryUrl.Inlines.Add("프로젝트 리포지토리");
      this.ProjectRepositoryUrl.RequestNavigate += Url_RequestNavigate;

      List<PackageInformation> packages = new List<PackageInformation>();
      var allPackages = PackageManager.GetFullPackageInfo();
      foreach (var item in allPackages.Values)
        packages.Add(item);
      this.PackageList.ItemsSource = packages;
    }

    private void Url_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      ProcessStartInfo ps = new ProcessStartInfo
      {
        FileName = e.Uri.AbsoluteUri,
        UseShellExecute = true
      };
      Process.Start(ps);
    }
  }
}
