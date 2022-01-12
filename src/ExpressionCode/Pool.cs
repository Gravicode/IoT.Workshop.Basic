// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Pool
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System.Collections;

namespace ExpressionCode
{
  internal class Pool
  {
    private readonly Stack pool = new Stack();
    private readonly Pool.FactoryFunc factory;
    private readonly Pool.ResetFunc reset;

    public int AcquireCount { get; private set; }

    public int CreateCount { get; private set; }

    public Pool(Pool.FactoryFunc factory, Pool.ResetFunc reset)
    {
      this.factory = factory;
      this.reset = reset;
    }

    public object Acquire()
    {
      ++this.AcquireCount;
      if (this.pool.Count > 0)
        return this.pool.Pop();
      ++this.CreateCount;
      return this.factory();
    }

    public void Release(object instance)
    {
      this.reset(instance);
      this.pool.Push(instance);
    }

    public void Clear()
    {
      foreach (object instance in this.pool)
        this.reset(instance);
      this.pool.Clear();
    }

    public delegate object FactoryFunc();

    public delegate void ResetFunc(object instance);
  }
}
