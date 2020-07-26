using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UMP.Utility
{
  public static class ImageProcessing
  {
    public static Color GetAverageColor(BitmapSource image, int offset = 1)
    {
      //Used for tally
      int r = 0;
      int g = 0;
      int b = 0;

      int total = 0;

      for (int x = 0; x < image.Width; x += offset)
      {
        for (int y = 0; y < image.Height; y += offset)
        {
          Color c = GetPixelColor(image, x, y);

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
