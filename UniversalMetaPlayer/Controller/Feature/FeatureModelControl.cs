using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace UMP.Controller.Feature
{
  public class FeatureModelControl : UserControl
  {
    public FeatureModelControl(string featureName)
    {
      this.FeatureName = featureName;
    }
    public string FeatureName { get; }
  }
}
