using System.Drawing;

using Colorful;

namespace UMP.CLI
{
  public class Program
  {
    static void Main(string[] args)
    {
      string helloWorld = "Hello World!";

      //helloWorld = helloWorld.Remove(helloWorld.Length - 2, 2);

      Console.WriteLineStyled(helloWorld, new StyleSheet(Color.Red));
      Console.ReadLine();
    }
  }
}
