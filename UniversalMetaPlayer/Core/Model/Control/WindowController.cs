using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using UMP.Controller.WindowHelper;

namespace UMP.Core.Model.Control
{
  public class WindowController
  {
    public WindowController()
    {
      WindowsDictionary = new Dictionary<WindowKind, UserWindowProperty>();
    }

    private Dictionary<WindowKind, UserWindowProperty> WindowsDictionary { get; }

    public IUserWindowProperty this[WindowKind windowKind]
    {
      get
      {
        if (WindowsDictionary.TryGetValue(windowKind, out UserWindowProperty windowProperty))
          return windowProperty;
        else
          return null;
      }
    }

    public IUserWindowProperty Create(WindowKind windowKind, bool force = false)
    {
      if (WindowsDictionary.TryGetValue(windowKind, out UserWindowProperty windowProperty))
      {
        if (force)
          windowProperty.Reset();

        return windowProperty;
      }
      else
      {
        var winproperty = new UserWindowProperty(windowKind);
        WindowsDictionary[windowKind] = winproperty;
        return winproperty;
      }
    }

    public IUserWindowProperty Open(WindowKind windowKind)
    {
      UserWindowProperty windowProperty;
      if (WindowsDictionary.TryGetValue(windowKind, out windowProperty))
        windowProperty.Open();
      else
      {
        windowProperty = new UserWindowProperty(windowKind);
        WindowsDictionary[windowKind] = windowProperty;
        windowProperty.Open();
      }
      return windowProperty;
    }

    public void OpenAll()
    {
      foreach (var item in WindowsDictionary.Values)
        item.Open();
    }

    public void Close(WindowKind windowKind)
    {
      if (WindowsDictionary.TryGetValue(windowKind, out UserWindowProperty windowProperty))
        windowProperty.Close();
    }

    public void CloseAll()
    {
      foreach (var item in WindowsDictionary.Values)
        item.Close();
    }

    public void Reset(WindowKind windowKind)
    {
      if (WindowsDictionary.TryGetValue(windowKind, out UserWindowProperty windowProperty))
        windowProperty.Reset();
    }

    public void Reset(bool force = false)
    {
      foreach (var item in WindowsDictionary)
      {
        if (force)
          item.Value.Reset();
        else
        {
          if (!item.Value.IsLocked)
          {
            item.Value.Reset();
            WindowsDictionary.Remove(item.Key);
          }
        }
      }
      if (force)
        WindowsDictionary.Clear();
    }

    private class UserWindowProperty : IUserWindowProperty
    {
      public UserWindowProperty(WindowKind windowKind) { this.WindowKind = windowKind; }

      public WindowKind WindowKind { get; }
      public UserWindow Window { get; private set; }

      public bool IsClosed => this.Window == null;
      public bool IsLocked
      {
        get => _IsLocked;
        set
        {
          if (_IsLocked != value)
          {
            _IsLocked = value;

            if (IsClosed)
              CreateWindow();

            if (_IsLocked)
              Window.Closing += LockMethod;
            else
              Window.Closing -= LockMethod;
          }
        }
      }
      private bool _IsLocked = false;

      private void LockMethod(object sender, System.ComponentModel.CancelEventArgs e)
      {
        this.Window.Visibility = Visibility.Collapsed;
        e.Cancel = true;
      }

      private void CreateWindow()
      {
        Reset();

        this.Window = WindowKindHelper.CreateUserWindow(this.WindowKind);
        this.Window.Closed += (_, e) => this.Window = null;
      }

      public void Reset()
      {
        if (!IsClosed)
          this.Window.Close();
      }

      public void Open()
      {
        if (IsClosed)
          CreateWindow();

        if (!this.Window.IsVisible)
        {
          if (this.Window.ShowActivated)
          {
            this.Window.Visibility = Visibility.Visible;
            this.Window.Activate();
          }
          else
            this.Window.Show();
        }
      }

      public void Close()
      {
        if (!IsClosed)
        {
          if (IsLocked)
            this.Window.Visibility = Visibility.Collapsed;
          else
            this.Window.Close();
        }
      }
    }
  }

  public interface IUserWindowProperty
  {
    public WindowKind WindowKind { get; }
    public UserWindow Window { get; }
    public bool IsLocked { get; set; }
    public bool IsClosed { get; }
  }
}
