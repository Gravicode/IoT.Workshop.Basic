// Decompiled with JetBrains decompiler
// Type: Database.SQLite.OpenException
// Assembly: Database.SQLite, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0CBA93C9-E337-4603-AF31-70FB34A6D058
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Database.SQLite.dll

using System;

namespace Database.SQLite
{
  [Serializable]
  public class OpenException : Exception
  {
    internal OpenException()
    {
    }

    internal OpenException(string message)
      : base(message)
    {
    }

    internal OpenException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
