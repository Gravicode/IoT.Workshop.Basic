// Decompiled with JetBrains decompiler
// Type: ExpressionCode.StringTable
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;

namespace ExpressionCode
{
  internal class StringTable
  {
    private static readonly StringTable st = new StringTable();
    private string[] entries;
    private int count;

    public static string Intern(string key) => StringTable.st.Resolve(key);

    public static void Clear() => StringTable.st.Reset();

    public static string[] All() => StringTable.st.entries;

    private void Reset()
    {
      this.count = 0;
      this.entries = (string[]) null;
    }

    private string Resolve(string key)
    {
      if (this.entries == null)
      {
        this.InsertAt(0, key);
      }
      else
      {
        int index = this.Search(key);
        if (index >= 0)
          return this.entries[index];
        this.InsertAt(~index, key);
      }
      return key;
    }

    private int Search(string key) => this.Search(key, 0, this.count);

    private int Search(string key, int index, int length)
    {
      int num1 = index;
      int num2 = index + length - 1;
      while (num1 <= num2)
      {
        int index1 = num1 + num2 >> 1;
        int num3 = string.Compare(this.entries[index1], key);
        if (num3 == 0)
          return index1;
        if (num3 < 0)
          num1 = index1 + 1;
        else
          num2 = index1 - 1;
      }
      return ~num1;
    }

    private void InsertAt(int index, string key)
    {
      this.Grow(1);
      if (index < this.count)
        Array.Copy((Array) this.entries, index, (Array) this.entries, index + 1, this.count - index);
      this.entries[index] = key;
      ++this.count;
    }

    private void Grow(int required)
    {
      if (this.entries == null)
      {
        this.entries = new string[Math.Max(4, required)];
      }
      else
      {
        int val1 = this.count + required;
        if (val1 < this.entries.Length)
          return;
        string[] strArray = new string[Math.Max(val1, this.count * 5 / 3)];
        Array.Copy((Array) this.entries, (Array) strArray, this.count);
        this.entries = strArray;
      }
    }
  }
}
