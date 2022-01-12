// Decompiled with JetBrains decompiler
// Type: ExpressionCode.SymTab
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Collections;

namespace ExpressionCode
{
  internal class SymTab
  {
    private SymTab.KV[] entries;
    private int count;

    public void Clear()
    {
      this.count = 0;
      this.entries = (SymTab.KV[]) null;
    }

    public object Get(string key)
    {
      int index = this.Search(key);
      return index >= 0 ? this.entries[index].value : (object) null;
    }

    public void Set(string key, object value)
    {
      if (this.entries == null)
      {
        this.InsertAt(0, key, value);
      }
      else
      {
        int index = this.Search(key);
        if (index >= 0)
          this.entries[index].value = value;
        else
          this.InsertAt(~index, key, value);
      }
    }

    public bool Contains(string key) => this.Search(key) >= 0;

    private int Search(string key) => this.Search(key, 0, this.count);

    private int Search(string key, int index, int length)
    {
      int num1 = index;
      int num2 = index + length - 1;
      while (num1 <= num2)
      {
        int index1 = num1 + num2 >> 1;
        int num3 = string.Compare(this.entries[index1].key, key);
        if (num3 == 0)
          return index1;
        if (num3 < 0)
          num1 = index1 + 1;
        else
          num2 = index1 - 1;
      }
      return ~num1;
    }

    private void InsertAt(int index, string key, object value)
    {
      this.Grow(1);
      if (index < this.count)
        Array.Copy((Array) this.entries, index, (Array) this.entries, index + 1, this.count - index);
      this.entries[index] = new SymTab.KV(key, value);
      ++this.count;
    }

    public object this[string key]
    {
      get => this.Get(key);
      set => this.Set(key, value);
    }

    public string[] Keys
    {
      get
      {
        string[] strArray = new string[this.count];
        for (int index = 0; index < this.count; ++index)
          strArray[index] = this.entries[index].key;
        return strArray;
      }
    }

    private void Grow(int required)
    {
      if (this.entries == null)
      {
        this.entries = new SymTab.KV[Math.Max(4, required)];
      }
      else
      {
        int val1 = this.count + required;
        if (val1 < this.entries.Length)
          return;
        SymTab.KV[] kvArray = new SymTab.KV[Math.Max(val1, this.count * 5 / 3)];
        Array.Copy((Array) this.entries, (Array) kvArray, this.count);
        this.entries = kvArray;
      }
    }

    private class KeyComparer : IComparer
    {
      public static IComparer Default = (IComparer) new SymTab.KeyComparer();

      public int Compare(object x, object y) => string.Compare(((SymTab.KV) x).key, (string) y);
    }

    private class KV
    {
      public string key;
      public object value;

      public KV(string key, object value)
      {
        this.key = key;
        this.value = value;
      }
    }
  }
}
