using System;
using System.Collections.Generic;
using System.Diagnostics;
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
      this.Mainlogo.Source = GlobalProperty.LogoImage;

      this.CoreVersionTitleLabel.Content = "코어 버전 : ";
      this.FileVersionTitleLabel.Content = "파일 버전 : ";
      this.CoreVersionLabel.Content = GlobalProperty.CoreVersion;
      this.FileVersionLabel.Content = GlobalProperty.FileVersion;

      this.ProjectRepositoryUrlLabel.Content = "프로젝트 리포지토리";
      this.ProjectRepositoryUrl.NavigateUri = new Uri("https://github.com/Cardroid/UniversalMetaPlayer");
      this.ProjectRepositoryUrl.ToolTip = "https://github.com/Cardroid/UniversalMetaPlayer 으로 이동합니다.";
      this.ProjectRepositoryUrl.Inlines.Add("Github");
      this.ProjectRepositoryUrl.RequestNavigate += Url_RequestNavigate;

      List<PackageInformation> packages = new List<PackageInformation>();
      var allPackages = PackageManager.GetFullPackageInfo();
      foreach (var item in allPackages.Values)
        packages.Add(item);
      this.PackageList.ItemsSource = packages;

      #region UpdateLog
      this.UpdateLogLabel.Text =
        $"v2.5.2.56" +
        $"최초 릴리스";
      #endregion
    }

    private void Url_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      if (Checker.CheckForInternetConnection())
      {
        Process.Start(e.Uri.AbsoluteUri);
      }
    }
  }
}
