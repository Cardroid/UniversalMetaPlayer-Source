using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace UMP.Utils.Effect
{
  /// <summary>
  /// Aero Glass 효과정의
  /// </summary>
  public class AeroGlass
  {
    #region 사용방법
    //private void Window_Loaded(object sender, RoutedEventArgs e)
    //{
    //  var transparencyConverter = new TransparencyConverter(this);
    //  transparencyConverter.EnableBlur();
    //}
    #endregion

    private readonly Window _window;

    public AeroGlass(Window window)
    {
      _window = window;
    }

    internal void EnableBlur()
    {
      var windowHelper = new WindowInteropHelper(_window);

      var accent = new AccentPolicy();
      var accentStructSize = Marshal.SizeOf(accent);
      accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

      var accentPtr = Marshal.AllocHGlobal(accentStructSize);
      Marshal.StructureToPtr(accent, accentPtr, false);

      var data = new WindowCompositionAttributeData();
      data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
      data.SizeOfData = accentStructSize;
      data.Data = accentPtr;

      SetWindowCompositionAttribute(windowHelper.Handle, ref data);

      Marshal.FreeHGlobal(accentPtr);
    }

    [DllImport("user32.dll")]
    internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);

    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
      public WindowCompositionAttribute Attribute;
      public IntPtr Data;
      public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
      // ...
      WCA_ACCENT_POLICY = 19
      // ...
    }

    internal enum AccentState
    {
      ACCENT_DISABLED = 0,
      ACCENT_ENABLE_GRADIENT = 1,
      ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
      ACCENT_ENABLE_BLURBEHIND = 3,
      ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
      public AccentState AccentState;
      public int AccentFlags;
      public int GradientColor;
      public int AnimationId;
    }
  }
}
