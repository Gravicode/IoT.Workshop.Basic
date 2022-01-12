// Decompiled with JetBrains decompiler
// Type: ExpressionCode.CharExtensions
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal static class CharExtensions
  {
    public static bool IsDigit(this char ch) => ch >= '0' && ch <= '9';

    public static bool IsLetter(this char ch)
    {
      if (ch >= 'a' && ch <= 'z')
        return true;
      return ch >= 'A' && ch <= 'Z';
    }

    public static bool IsLetterOrDigit(this char ch) => ch.IsDigit() || ch.IsLetter();

    public static bool IsIdentifierChar(this char ch) => ch.IsDigit() || ch.IsLetter() || ch == '_';

    public static bool IsPrintable(this char ch) => ch >= ' ';

    public static bool IsWhiteSpace(this char ch) => ch == ' ' || ch == '\t';
  }
}
