using NAudio.Wave;

namespace UMP.Core.Model.Func
{
  public interface ISamplePlugin
  {
    public WaveFormat WaveFormat { get; }

    public int Read(int samplesRead, float[] buffer, int offset, int count);

    public bool IsEnabled { get; set; }
  }
}
