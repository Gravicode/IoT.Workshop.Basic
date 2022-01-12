// Decompiled with JetBrains decompiler
// Type: Database.SQLite.ResultSet
// Assembly: Database.SQLite, Version=2.1.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0CBA93C9-E337-4603-AF31-70FB34A6D058
// Assembly location: D:\experiment\sqliteapp1\sqliteapp1\bin\Debug\Database.SQLite.dll

using System;
using System.Collections;

namespace Database.SQLite
{
  public class ResultSet
  {
    private int rowCount;
    private int columnCount;
    private string[] columnNames;
    private ArrayList data;

    public int RowCount => this.rowCount;

    public int ColumnCount => this.columnCount;

    public string[] ColumnNames => this.columnNames;

    public ArrayList Data => this.data;

    public ArrayList this[int row] => row >= 0 && row < this.rowCount ? (ArrayList) this.Data[row] : throw new ArgumentOutOfRangeException(nameof (row));

    public object this[int row, int column]
    {
      get
      {
        if (row < 0 || row >= this.rowCount)
          throw new ArgumentOutOfRangeException(nameof (row));
        if (column < 0 || column >= this.columnCount)
          throw new ArgumentOutOfRangeException(nameof (column));
        return ((ArrayList) this.Data[row])[column];
      }
    }

    internal ResultSet(string[] columnNames)
    {
      if (columnNames == null)
        throw new ArgumentNullException(nameof (columnNames));
      if (columnNames.Length == 0)
        throw new ArgumentException("At least one column must be provided.", nameof (columnNames));
      this.data = new ArrayList();
      this.columnNames = new string[columnNames.Length];
      this.columnCount = columnNames.Length;
      this.rowCount = 0;
      Array.Copy((Array) columnNames, (Array) this.columnNames, columnNames.Length);
    }

    internal void AddRow(ArrayList row)
    {
      if (row.Count != this.columnCount)
        throw new ArgumentException("Row must contain exactly as many members as the number of columns in this result set.", nameof (row));
      ++this.rowCount;
      this.data.Add((object) row);
    }
  }
}
