// Decompiled with JetBrains decompiler
// Type: Database.SQLite.SQLiteDatabase
// Assembly: Database.SQLite, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0CBA93C9-E337-4603-AF31-70FB34A6D058
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Database.SQLite.dll

using System;
using System.Collections;
using System.IO;
using System.Runtime.CompilerServices;

namespace Database.SQLite
{
  public class SQLiteDatabase : IDisposable
  {
    private const int SQLITE_OK = 0;
    private const int SQLITE_ROW = 100;
    private const int SQLITE_DONE = 101;
    private const int SQLITE_INTEGER = 1;
    private const int SQLITE_FLOAT = 2;
    private const int SQLITE_TEXT = 3;
    private const int SQLITE_BLOB = 4;
    private const int SQLITE_NULL = 5;
    private bool disposed;
    private int nativePointer;

    public SQLiteDatabase()
    {
      this.nativePointer = 0;
      this.disposed = false;
      if (this.NativeOpen(":memory:") != 0)
        throw new OpenException();
    }

    public SQLiteDatabase(string file)
    {
      this.nativePointer = 0;
      this.disposed = false;
      file = Path.GetFullPath(file);
      if (file == null)
        throw new ArgumentException("You must provide a valid file.", nameof (file));
      if (this.NativeOpen(file) != 0)
        throw new OpenException();
    }

    ~SQLiteDatabase() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void ExecuteNonQuery(string query)
    {
      if (this.disposed)
        throw new ObjectDisposedException("Object disposed.");
      int handle = query != null ? this.PrepareSqlStatement(query) : throw new ArgumentNullException(nameof (query));
      if (this.NativeStep(handle) != 101)
        throw new QueryExecutionException(this.NativeErrorMessage());
      this.FinalizeSqlStatment(handle);
    }

    public ResultSet ExecuteQuery(string query)
    {
      if (this.disposed)
        throw new ObjectDisposedException("Object disposed.");
      int handle = query != null ? this.PrepareSqlStatement(query) : throw new ArgumentNullException(nameof (query));
      int length1 = this.NativeColumnCount(handle);
      string[] columnNames = new string[length1];
      for (int column = 0; column < length1; ++column)
        columnNames[column] = this.NativeColumnName(handle, column);
      ResultSet resultSet = new ResultSet(columnNames);
      while (this.NativeStep(handle) == 100)
      {
        ArrayList row = new ArrayList();
        for (int column = 0; column < length1; ++column)
        {
          switch (this.NativeColumnType(handle, column))
          {
            case 1:
              row.Add((object) this.NativeColumnLong(handle, column));
              break;
            case 2:
              row.Add((object) this.NativeColumnDouble(handle, column));
              break;
            case 3:
              row.Add((object) this.NativeColumnText(handle, column));
              break;
            case 4:
              int length2 = this.NativeColumnBlobLength(handle, column);
              if (length2 == 0)
              {
                row.Add((object) null);
                break;
              }
              byte[] buffer = new byte[length2];
              this.NativeColumnBlobData(handle, column, buffer);
              row.Add((object) buffer);
              break;
            case 5:
              row.Add((object) null);
              break;
          }
        }
        resultSet.AddRow(row);
      }
      this.FinalizeSqlStatment(handle);
      return resultSet;
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.NativeClose();
      this.disposed = true;
    }

    private int PrepareSqlStatement(string query)
    {
      if (this.disposed)
        throw new ObjectDisposedException("Object disposed.");
      int handle;
      if (this.NativePrepare(query, query.Length, out handle) != 0)
        throw new QueryPrepareException(this.NativeErrorMessage());
      return handle;
    }

    private void FinalizeSqlStatment(int handle)
    {
      if (this.disposed)
        throw new ObjectDisposedException("Object disposed.");
      if (this.NativeFinalize(handle) != 0)
        throw new QueryFinalizationException(this.NativeErrorMessage());
    }

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeOpen(string filename);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeClose();

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativePrepare(string query, int queryLength, out int handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern string NativeErrorMessage();

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeStep(int handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeFinalize(int handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeColumnCount(int handle);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern string NativeColumnName(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeColumnType(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern long NativeColumnLong(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern string NativeColumnText(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern double NativeColumnDouble(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern int NativeColumnBlobLength(int handle, int column);

    [MethodImpl(MethodImplOptions.InternalCall)]
    private extern void NativeColumnBlobData(int handle, int column, byte[] buffer);
  }
}
