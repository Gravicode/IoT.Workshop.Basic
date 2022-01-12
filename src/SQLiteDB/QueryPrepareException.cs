// Decompiled with JetBrains decompiler
// Type: Database.SQLite.QueryPrepareException
// Assembly: Database.SQLite, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0CBA93C9-E337-4603-AF31-70FB34A6D058
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Database.SQLite.dll

using System;

namespace Database.SQLite
{
  [Serializable]
  public class QueryPrepareException : Exception
  {
    internal QueryPrepareException()
    {
    }

    internal QueryPrepareException(string message)
      : base(message)
    {
    }

    internal QueryPrepareException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
