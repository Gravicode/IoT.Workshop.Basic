// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Scope
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using ExpressionCode.Debugger;
using System;
using System.Collections;
using System.Text;

namespace ExpressionCode
{
  internal class Scope
  {
    private readonly SymTab symbolTable = new SymTab();
    private Scope parent;
    private static readonly Stack scopeCache = new Stack();

    public string Name { get; private set; }

    public static Scope FromCache(string name, Scope parentScope)
    {
      if (Scope.scopeCache.Count == 0)
        return new Scope(name, parentScope);
      Scope scope = (Scope) Scope.scopeCache.Pop();
      scope.parent = parentScope;
      scope.Name = name;
      return scope;
    }

    public static void Release(Scope scope)
    {
      scope.symbolTable.Clear();
      scope.parent = (Scope) null;
      scope.Name = string.Empty;
      Scope.scopeCache.Push((object) scope);
      //GC.Collect();
    }

    private Scope(string name, Scope parent)
    {
      this.Name = name;
      this.parent = parent;
    }

    public object Get(string key)
    {
      for (Scope scope = this; scope != null; scope = scope.parent)
      {
        if (scope.symbolTable.Contains(key))
          return scope.symbolTable[key];
      }
      return (object) null;
    }

    public bool Exists(string key) => this.Get(key) != null;

    public void Add(string key, object value) => this.symbolTable[key] = value;

    public bool Assign(string key, object value)
    {
      for (Scope scope = this; scope != null; scope = scope.parent)
      {
        if (scope.symbolTable.Contains(key))
        {
          scope.symbolTable[key] = value;
          return true;
        }
      }
      return false;
    }

    public string[] GetAllFunctions()
    {
      StringBuilder stringBuilder = new StringBuilder();
      ArrayList arrayList = new ArrayList();
      for (Scope scope = this; scope != null; scope = scope.parent)
      {
        foreach (string key in scope.symbolTable.Keys)
        {
          switch (scope.symbolTable[key])
          {
            case FuncDeclAst funcDeclAst2:
              stringBuilder.Clear();
              stringBuilder.Append(funcDeclAst2.functionName);
              stringBuilder.Append("(");
              for (int index = 0; index < funcDeclAst2.parameters.Length; ++index)
              {
                if (index > 0)
                  stringBuilder.Append(",");
                stringBuilder.Append(funcDeclAst2.parameters[index]);
              }
              stringBuilder.Append(")");
              arrayList.Add((object) stringBuilder.ToString());
              break;
            case BuiltInDeclAst builtInDeclAst2:
              stringBuilder.Clear();
              stringBuilder.Append(builtInDeclAst2.functionName);
              stringBuilder.Append("(");
              for (int index = 0; index < builtInDeclAst2.ArgumentBuffer.Length; ++index)
              {
                if (index > 0)
                  stringBuilder.Append(",");
                stringBuilder.Append("arg");
                stringBuilder.Append(index);
              }
              stringBuilder.Append(")");
              arrayList.Add((object) stringBuilder.ToString());
              break;
          }
        }
      }
      string[] array = (string[]) arrayList.ToArray(typeof (string));
      Scope.Sort(array);
      return array;
    }

    public Variable[] GetAllVariables()
    {
      ArrayList arrayList = new ArrayList();
      for (Scope scope = this; scope != null; scope = scope.parent)
      {
        bool isConstant = scope.Name == "<constants>";
        foreach (string key in scope.symbolTable.Keys)
        {
          object obj = scope.symbolTable[key];
          switch (obj)
          {
            case FuncDeclAst _:
            case BuiltInDeclAst _:
              continue;
            default:
              arrayList.Add((object) new Variable(key, obj, isConstant));
              continue;
          }
        }
      }
      Variable[] array = (Variable[]) arrayList.ToArray(typeof (Variable));
      Scope.Sort(array);
      return array;
    }

    public Variable[] GetLocals()
    {
      ArrayList arrayList = new ArrayList();
      Scope scope = this;
      bool flag = !string.IsNullOrEmpty(scope.Name);
      do
      {
        foreach (string key in scope.symbolTable.Keys)
        {
          object obj = scope.symbolTable[key];
          switch (obj)
          {
            case FuncDeclAst _:
            case BuiltInDeclAst _:
              continue;
            default:
              arrayList.Add((object) new Variable(key, obj, false));
              continue;
          }
        }
        if (!flag)
        {
          scope = scope.parent;
          flag = !string.IsNullOrEmpty(scope.Name);
        }
        else
          break;
      }
      while (((scope == null ? 0 : (string.IsNullOrEmpty(scope.Name) ? 1 : 0)) | (flag ? 1 : 0)) != 0);
      Variable[] array = (Variable[]) arrayList.ToArray(typeof (Variable));
      Scope.Sort(array);
      return array;
    }

    public Variable[][] GetAllInScope(Scope until)
    {
      ArrayList arrayList = new ArrayList();
      for (Scope scope = this; scope != null && scope != until; scope = scope.parent)
        arrayList.Add((object) scope.GetLocals());
      Variable[][] variableArray = new Variable[arrayList.Count][];
      for (int index = 0; index < arrayList.Count; ++index)
        variableArray[index] = (Variable[]) arrayList[index];
      return variableArray;
    }

    private static void Sort(Variable[] variables)
    {
      for (int index1 = 1; index1 < variables.Length; ++index1)
      {
        Variable variable = variables[index1];
        int index2;
        for (index2 = index1 - 1; index2 >= 0 && string.Compare(variables[index2].Name, variable.Name) > 0; --index2)
          variables[index2 + 1] = variables[index2];
        variables[index2 + 1] = variable;
      }
    }

    private static void Sort(string[] strings)
    {
      for (int index1 = 1; index1 < strings.Length; ++index1)
      {
        string strB = strings[index1];
        int index2;
        for (index2 = index1 - 1; index2 >= 0 && string.Compare(strings[index2], strB) > 0; --index2)
          strings[index2 + 1] = strings[index2];
        strings[index2 + 1] = strB;
      }
    }
  }
}
