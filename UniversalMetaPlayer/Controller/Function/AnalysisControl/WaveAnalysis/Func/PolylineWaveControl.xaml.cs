using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

using UMP.Core.Player;
using UMP.Utility;

namespace UMP.Controller.Function.AnalysisControl.WaveAnalysis.Func
{
  public partial class PolylineWaveControl : UserControl, IWaveFormRenderer
  {
    int renderPosition;
    double yTranslate;
    double yScale;
    int blankZone;

    readonly Polyline topLine = new Polyline();
    readonly Polyline bottomLine = new Polyline();

    public PolylineWaveControl()
    {
      yTranslate = 40;
      yScale = 40;
      blankZone = 5;

      SizeChanged += OnSizeChanged;
      InitializeComponent();

      this.BorderBrush = new SolidColorBrush(ThemeHelper.PrimaryColor);
      ThemeHelper.ThemeChangedEvent += (e) => this.BorderBrush = new SolidColorBrush(e.PrimaryColor);

      topLine.Stroke = Brushes.Yellow;
      bottomLine.Stroke = Brushes.Yellow;
      topLine.StrokeThickness = 1;
      bottomLine.StrokeThickness = 1;
      mainCanvas.Children.Add(topLine);
      mainCanvas.Children.Add(bottomLine);

      MainMediaPlayer.PropertyChanged += (_, e) =>
      {
        if (e.PropertyName == "MainPlayerInitialized")
          Reset();
      };
    }

    void OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
      // We will remove everything as we are going to rescale vertically
      renderPosition = 0;
      ClearAllPoints();

      yTranslate = ActualHeight / 2;
      yScale = ActualHeight / 2;
    }

    private void ClearAllPoints()
    {
      topLine.Points.Clear();
      bottomLine.Points.Clear();
    }

    public void AddValue(float maxValue, float minValue)
    {
      int pixelWidth = (int)ActualWidth;
      if (pixelWidth > 0)
      {
        CreatePoint(maxValue, minValue);

        if (renderPosition > ActualWidth)
          renderPosition = 0;

        int erasePosition = (renderPosition + blankZone) % pixelWidth;
        if (erasePosition < topLine.Points.Count)
        {
          double yPos = SampleToYPosition(0);
          topLine.Points[erasePosition] = new Point(erasePosition, yPos);
          bottomLine.Points[erasePosition] = new Point(erasePosition, yPos);
        }
      }
    }

    private double SampleToYPosition(float value)
    {
      return yTranslate + value * yScale;
    }

    private void CreatePoint(float topValue, float bottomValue)
    {
      double topLinePos = SampleToYPosition(topValue);
      double bottomLinePos = SampleToYPosition(bottomValue);
      if (renderPosition >= topLine.Points.Count)
      {
        topLine.Points.Add(new Point(renderPosition, topLinePos));
        bottomLine.Points.Add(new Point(renderPosition, bottomLinePos));
      }
      else
      {
        topLine.Points[renderPosition] = new Point(renderPosition, topLinePos);
        bottomLine.Points[renderPosition] = new Point(renderPosition, bottomLinePos);
      }
      renderPosition++;
    }

    /// <summary>
    /// Clears the waveform and repositions on the left
    /// </summary>
    public void Reset()
    {
      renderPosition = 0;
      ClearAllPoints();
    }
  }
}
