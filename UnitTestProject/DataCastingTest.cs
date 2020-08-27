using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject
{
  [TestClass]
  public class DataCastingTest
  {
    [TestMethod]
    public void PercentageCast()
    {
      const int max = 100;

      for (int i = 0; i < max; i++)
      {
        int j = (int)((float)i / max * 60f);
        Trace.WriteLine($"100 : {i}%");
        Trace.WriteLine($" 60 : {j}%");
      }
      Assert.IsTrue(true);
    }
  }
}
