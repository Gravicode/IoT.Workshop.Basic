// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Stdlib
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Collections;
using System.Threading;

namespace ExpressionCode
{
  internal static class Stdlib
  {
    private static Random rnd = new Random();
    private static IConsole console;
    public static readonly int TRUE = 1;
    public static readonly int FALSE = 0;
    private const double TicksPerMillisecond = 10000.0;

    internal static void SetConsole(IConsole newConsole) => Stdlib.console = newConsole;

    internal static IConsole Console => Stdlib.console;

    public static ArrayValue Array(int capacity)
    {
      ArrayValue arrayValue = new ArrayValue(capacity);
      for (int index = 0; index < capacity; ++index)
        arrayValue.Add((object) 0);
      return arrayValue;
    }

    public static int Len(object o)
    {
      switch (o)
      {
        case string str:
          return str.Length;
        case ArrayList arrayList:
          return arrayList.Count;
        default:
          throw new InterpreterException("Expected string or array");
      }
    }

    public static object Left(object o, int count)
    {
      switch (o)
      {
        case string str:
          if (count > str.Length)
            count = str.Length;
          return (object) str.Substring(0, count);
        case ArrayList arrayList:
          if (count > arrayList.Count)
            count = arrayList.Count;
          ArrayValue arrayValue = new ArrayValue(count);
          for (int index = 0; index < count; ++index)
            arrayValue.Add(arrayList[index]);
          return (object) arrayValue;
        default:
          throw new InterpreterException("Expected string or array");
      }
    }

    public static object Right(object o, int count)
    {
      switch (o)
      {
        case string str:
          if (count > str.Length)
            count = str.Length;
          return (object) str.Substring(str.Length - count);
        case ArrayList arrayList:
          if (count > arrayList.Count)
            count = arrayList.Count;
          ArrayValue arrayValue = new ArrayValue(count);
          for (int index = arrayList.Count - count; index < arrayList.Count; ++index)
            arrayValue.Add(arrayList[index]);
          return (object) arrayValue;
        default:
          throw new InterpreterException("Expected string or array");
      }
    }

    public static object Mid(object o, int index, int count)
    {
      switch (o)
      {
        case string str:
          if (index > str.Length)
            return (object) string.Empty;
          if (count < 0 || index + count >= str.Length)
            count = str.Length - index;
          return (object) str.Substring(index, count);
        case ArrayList arrayList:
          if (index > arrayList.Count)
            return (object) ArrayValue.Empty;
          if (count < 0 || index + count >= arrayList.Count)
            count = arrayList.Count - index;
          ArrayValue arrayValue = new ArrayValue(count);
          for (int index1 = index; index1 < index + count; ++index1)
            arrayValue.Add(arrayList[index1]);
          return (object) arrayValue;
        default:
          throw new InterpreterException("Expected string or array");
      }
    }

    public static int IndexOf(object o, object value)
    {
      switch (o)
      {
        case string str:
          return str.IndexOf((string) value);
        case ArrayList arrayList:
          return arrayList.IndexOf(value);
        default:
          throw new InterpreterException("Expected string or array");
      }
    }

    public static double Val(string s)
    {
      double result;
      return double.TryParse(s, out result) ? result : double.NaN;
    }

    public static string StrFmt(double d, string format) => d.ToString(format);

    public static int IsNan(double d) => !double.IsNaN(d) ? 0 : 1;

    public static double Abs(double d) => Math.Abs(d);

    public static double Sqrt(double d) => Math.Sqrt(d);

    public static double Sin(double rad) => Math.Sin(rad);

    public static double Cos(double rad) => Math.Cos(rad);

    public static double Tan(double rad) => Math.Tan(rad);

    public static double Acos(double a) => Math.Acos(a);

    public static double Asin(double a) => Math.Asin(a);

    public static double Atan(double a) => Math.Atan(a);

    public static double Atan2(double y, double x) => Math.Atan2(y, x);

    public static double Trunc(double d) => Math.Truncate(d);

    public static double Round(double d) => Math.Round(d);

    public static double Rnd() => Stdlib.rnd.NextDouble();

    public static void Delay(int ms) => Thread.Sleep(ms);

    internal static void Locate(int row, int col)
    {
      if (Stdlib.console == null)
        throw new InterpreterException("Not supported");
      Stdlib.console.Locate(row, col);
    }

    internal static void Print(object o)
    {
      if (Stdlib.console == null)
        throw new InterpreterException("Not supported");
      Stdlib.console.Print(o.ToString());
    }

    internal static void Cls()
    {
      if (Stdlib.console == null)
        throw new InterpreterException("Not supported");
      Stdlib.console?.Cls();
    }

    public static void Append(ArrayList arr, object value) => arr.Add(value);

    public static void RemoveAt(ArrayList arr, int index) => arr.RemoveAt(index);

    public static void InsertAt(ArrayList arr, int index, object value) => arr.Insert(index, value);

    public static double Millis() => (double) DateTime.UtcNow.Ticks / 10000.0;
  }
}
