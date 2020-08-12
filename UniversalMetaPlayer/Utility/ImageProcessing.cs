using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using UMP.Core;

namespace UMP.Utility
{
  public static class ImageProcessing
  {
    public static Color GetAverageColor(BitmapSource image, int offset = 1)
    {
      bool error = false;

      //Used for tally
      int r = 0;
      int g = 0;
      int b = 0;

      int total = 0;

      for (int x = 0; x < image.Width; x += offset)
      {
        for (int y = 0; y < image.Height; y += offset)
        {
          Color c;
          try
          {
            c = GetPixelColor(image, x, y);
          }
          catch (Exception e)
          {
            if (!error)
            {
              error = true;
              new Log(typeof(ImageProcessing)).Error("이미지 픽셀 추출 실패", e);
              GlobalEvent.GlobalMessageEventInvoke("이미지 픽셀 추출에 오류가 발생했습니다.\n이미지에 이상이 없는지 확인해주세요", true);
            }
            continue;
          }

          r += c.R;
          g += c.G;
          b += c.B;

          total++;
        }
      }

      //Calculate average
      r /= total;
      g /= total;
      b /= total;

      return Color.FromRgb((byte)r, (byte)g, (byte)b);
    }

    public static Color GetPixelColor(BitmapSource bitmap, int x, int y)
    {
      Color color;
      var bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
      var bytes = new byte[bytesPerPixel];
      var rect = new System.Windows.Int32Rect(x, y, 1, 1);

      bitmap.CopyPixels(rect, bytes, bytesPerPixel, 0);

      if (bitmap.Format == PixelFormats.Bgra32)
      {
        color = Color.FromArgb(bytes[3], bytes[2], bytes[1], bytes[0]);
      }
      else if (bitmap.Format == PixelFormats.Bgr32)
      {
        color = Color.FromRgb(bytes[2], bytes[1], bytes[0]);
      }
      // handle other required formats
      else
      {
        color = Colors.Black;
      }

      return color;
    }
  }
}
