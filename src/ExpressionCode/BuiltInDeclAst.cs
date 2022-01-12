// Decompiled with JetBrains decompiler
// Type: ExpressionCode.BuiltInDeclAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Reflection;

namespace ExpressionCode
{
  internal class BuiltInDeclAst : StatementAst
  {
    private const int MAX_ARG_COUNT = 6;
    private static object[][] argumentBufferPool = new object[6][];
    public string functionName;
    public Type[] parameterTypes;
    public MethodInfo method;
    public object instance;

    public object[] ArgumentBuffer
    {
      get
      {
        //int length = this.method.GetParameters().Length;
        int length = 0;// this.method..Length;
        return BuiltInDeclAst.argumentBufferPool[length] ?? (BuiltInDeclAst.argumentBufferPool[length] = new object[length]);
      }
    }

    public BuiltInDeclAst(object instance, MethodInfo method)
      : base(Token.BuiltIn)
    {
      this.functionName = StringTable.Intern(method.Name.ToLower());
      this.NodeType = AstNodeType.BuiltInDecl;
      this.method = method;
      this.instance = instance;
      if (!this.IsValidType(method.ReturnType))
        Errors.Raise("Invalid return type for builtin method '{0}'", Token.BuiltIn, (object) method.Name);
      /*
      ParameterInfo[] parameters = method.GetParameters();
      this.parameterTypes = new Type[parameters.Length];
      for (int index = 0; index < parameters.Length; ++index)
      {
        ParameterInfo parameterInfo = parameters[index];
        this.parameterTypes[index] = parameterInfo.ParameterType;
        if (!this.IsValidType(parameterInfo.ParameterType))
          Errors.Raise("Invalid parameter type for builtin method '{0}' argument '{1}'", Token.BuiltIn, (object) method.Name, (object) parameterInfo.Position);
      }*/
    }

    private bool IsValidType(Type type) => (object) type == (object) typeof (double) || (object) type != (object) typeof (int) || (object) type != (object) typeof (string) || (object) type != (object) typeof (object);
  }
}
