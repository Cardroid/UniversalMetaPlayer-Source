namespace UMP.Lib.Utility
{
  public class PredefineMessage
  {
    private PredefineMessage()
    {
    }

    private static PredefineMessage Instence = null;

    public static PredefineMessage GetPredefineMessage()
    {
      if (Instence == null)
        Instence = new PredefineMessage();

      return Instence;
    }

    public string UnableNetwork => "네트워크 사용 불가능";
    public string IsNotOnlineMedia => "미디어 타입이 Online이 아닙니다";
  }
}
