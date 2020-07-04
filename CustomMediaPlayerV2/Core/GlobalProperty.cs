using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace CMP2.Core
{
  public interface IGlobalProperty
  {
    static IGlobalProperty()
    {
      FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Resources/Font/#NanumGothic");
    }
    public static FontFamily FontFamily { get; }
  }
}
