// Decompiled with JetBrains decompiler
// Type: ExpressionCode.ArrayValue
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System.Collections;
using System.Text;

namespace ExpressionCode
{
  internal class ArrayValue : ArrayList
  {
    public static readonly ArrayValue Empty = new ArrayValue();

    public ArrayValue()
    {
    }

    public ArrayValue(ICollection c)
     
    {
           foreach(var item in c)
            {
                base.Add(item); 
            }
    }

    public ArrayValue(int capacity)
      
    {
            base.Capacity = capacity;
    }

    public override string ToString() => ArrayValue.ToString((ICollection) this);

    internal static string ToString(ICollection collection)
    {
      StringBuilder stringBuilder = new StringBuilder();
      bool flag = false;
      stringBuilder.Append('[');
      foreach (object obj in (IEnumerable) collection)
      {
        if (flag)
          stringBuilder.Append(',');
        stringBuilder.Append(obj.ToString());
        flag = true;
      }
      stringBuilder.Append(']');
      return stringBuilder.ToString();
    }
  }

    public static class MyExtensions
    {
        public static void AddRange(this ArrayList arr,params object[] items)
        {
            foreach(var item in items)
            {
                arr.Add(item);
            }
        }
    }

}
