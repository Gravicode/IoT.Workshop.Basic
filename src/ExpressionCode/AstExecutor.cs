// Decompiled with JetBrains decompiler
// Type: ExpressionCode.AstExecutor
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using ExpressionCode.Debugger;
using System;
using System.Collections;
using System.Reflection;

namespace ExpressionCode
{
    internal class AstExecutor
    {
        private Stack scopeStack = new Stack();
        private Scope constants;
        private Scope globals;
        private Scope scriptScope;
        private Scope currentScope;
        private bool stopExecution;
        private int stopCheckCounter;
        private static AstExecutor.ExecuteFunc[] execTable = new AstExecutor.ExecuteFunc[37];
        private int stepOver;
        private int lastLine = -1;
        private DebugAction lastAction = DebugAction.StepInto;
        private Hashtable breakpoints = new Hashtable();

        public AstExecutor()
        {
            this.constants = Scope.FromCache("<constants>", (Scope)null);
            this.globals = Scope.FromCache("<globals>", this.constants);
            this.scriptScope = Scope.FromCache("<script>", this.globals);
            this.currentScope = this.scriptScope;
        }

        static AstExecutor()
        {
            AstExecutor.execTable[3] = AstExecutor.execTable[22] = (AstExecutor.ExecuteFunc)((e, ast) => (object)null);
            AstExecutor.execTable[2] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((ScriptAst)ast));
            AstExecutor.execTable[5] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((StatementBlockAst)ast));
            AstExecutor.execTable[25] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((FloatAst)ast));
            AstExecutor.execTable[26] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((IntAst)ast));
            AstExecutor.execTable[27] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((StringAst)ast));
            AstExecutor.execTable[28] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((ArrayLiteralAst)ast));
            AstExecutor.execTable[29] = (AstExecutor.ExecuteFunc)((e, ast) => e.ExecutePre((IndexAccessorAst)ast));
            AstExecutor.execTable[30] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((IndexAccessorAst)ast));
            AstExecutor.execTable[24] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((NegateAst)ast));
            AstExecutor.execTable[6] = (AstExecutor.ExecuteFunc)((e, ast) => e.ExecutePre((VarDeclAst)ast));
            AstExecutor.execTable[7] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((VarDeclAst)ast));
            AstExecutor.execTable[13] = (AstExecutor.ExecuteFunc)((e, ast) => e.ExecutePre((AssignmentAst)ast));
            AstExecutor.execTable[14] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((AssignmentAst)ast));
            AstExecutor.execTable[31] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((VariableAccessAst)ast));
            AstExecutor.execTable[32] = (AstExecutor.ExecuteFunc)((e, ast) => ((ConstValueAst)ast).value);
            AstExecutor.execTable[15] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((IfAst)ast));
            AstExecutor.execTable[16] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((IfElseAst)ast));
            AstExecutor.execTable[17] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((IfElseIfAst)ast));
            AstExecutor.execTable[18] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((WhileAst)ast));
            AstExecutor.execTable[11] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((BreakAst)ast));
            AstExecutor.execTable[12] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((ContinueAst)ast));
            AstExecutor.execTable[19] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((LocateAst)ast));
            AstExecutor.execTable[20] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((PrintAst)ast));
            AstExecutor.execTable[21] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((ClsAst)ast));
            AstExecutor.execTable[8] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((FuncDeclAst)ast));
            AstExecutor.execTable[33] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((FuncCallAst)ast));
            AstExecutor.execTable[10] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((ReturnAst)ast));
            AstExecutor.execTable[34] = (AstExecutor.ExecuteFunc)((e, ast) => e.ExecutePre((BinOpAst)ast));
            AstExecutor.execTable[35] = (AstExecutor.ExecuteFunc)((e, ast) => e.ExecuteConst((BinOpAst)ast));
            AstExecutor.execTable[36] = (AstExecutor.ExecuteFunc)((e, ast) => e.Execute((BinOpAst)ast));
            AstExecutor.execTable[4] = (AstExecutor.ExecuteFunc)((e, ast) =>
           {
               ExpressionAst expression = ((ExpressionStatementAst)ast).expression;
               if (e.DebugCallback != null)
                   e.StepDebugger((AstNode)expression);
               return AstExecutor.execTable[(int)expression.NodeType](e, (AstNode)expression);
           });
        }

        public void Reset()
        {
            this.scopeStack.Clear();
            this.breakpoints.Clear();
            this.scriptScope = Scope.FromCache("<script>", this.globals);
            this.currentScope = this.scriptScope;
            this.scopeStack.Push((object)this.currentScope);
        }

        public void Execute(AstNode ast)
        {
            lock (this.currentScope)
                this.stopExecution = false;
            object obj = AstExecutor.execTable[(int)ast.NodeType](this, ast);
        }

        public void AddConstant(string key, object value) => this.constants.Add(key, value);

        public bool IsConstant(string key) => this.constants.Exists(key);

        public void LoadBuiltIn(Type builtins)
        {
            foreach (MethodInfo method in builtins.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
                this.globals.Add(StringTable.Intern(method.Name.ToLower()), (object)new BuiltInDeclAst((object)null, method));
            foreach (FieldInfo field in builtins.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public))
                this.AddConstant(StringTable.Intern(field.Name.ToLower()), field.GetValue((object)null));
        }

        public void LoadBuiltIn(object instance)
        {
            Type type = instance.GetType();
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public))              
                this.globals.Add(StringTable.Intern(method.Name.ToLower()), (object)new BuiltInDeclAst(instance, method));
            foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                this.AddConstant(StringTable.Intern(field.Name.ToLower()), field.GetValue(instance));
        }

        public object GetFunction(string functionName)
        {
            object obj = this.currentScope.Get(functionName.ToLower());
            if (obj is FuncDeclAst)
                return obj;
            Errors.Raise("Function not declared '{0}'", (ushort)1, (ushort)1, (object)functionName);
            return (object)null;
        }

        public object GetValue(string variableName) => this.scriptScope.Get(variableName.ToLower());

        public void SetValue(string variableName, object value)
        {
            if (value is ICollection c && !(value is ArrayList))
                value = (object)new ArrayValue(c);
            this.scriptScope.Add(variableName, value);
        }

        public object Invoke(string functionName, params object[] args) => this.Invoke(this.currentScope.Get(functionName.ToLower()), args);

        public object Invoke(object function, params object[] args)
        {
            switch (function)
            {
                case FuncDeclAst funcDecl1:
                    return this.Invoke(funcDecl1, args);
                case BuiltInDeclAst funcDecl2:
                    return this.Invoke(funcDecl2, args);
                default:
                    Errors.Raise("Identifier not declared '{0}'", (ushort)1, (ushort)1, function);
                    return (object)null;
            }
        }

        private object Invoke(FuncDeclAst funcDecl, object[] args)
        {
            if (funcDecl.parameters.Length != args.Length)
                Errors.Raise("Argument count mismatch, Expected '{0}' got '{1}'", (ushort)1, (ushort)1, (object)funcDecl.parameters.Length, (object)args.Length);
            Scope newScope = Scope.FromCache(string.Intern(funcDecl.functionName), funcDecl.parentScope);
            if (funcDecl.parameters.Length != 0)
            {
                for (int index = 0; index < funcDecl.parameters.Length; ++index)
                    newScope.Add(funcDecl.parameters[index], args[index]);
            }
            this.EnterScope(newScope);
            try
            {
                object obj = AstExecutor.execTable[(int)funcDecl.body.NodeType](this, (AstNode)funcDecl.body);
                if (obj is AstExecutor.ReturnValue returnValue2)
                    obj = returnValue2.value;
                return obj;
            }
            finally
            {
                this.LeaveScope();
            }
        }

        private object Invoke(BuiltInDeclAst funcDecl, object[] args)
        {
            Type[] parameterTypes = funcDecl.parameterTypes;
            if (parameterTypes.Length != args.Length)
                Errors.Raise("Argument count mismatch, Expected '{0}' got '{1}'", (ushort)1, (ushort)1, (object)parameterTypes.Length, (object)args.Length);
            object[] parameters = (object[])null;
            if (parameterTypes.Length != 0)
            {
                parameters = funcDecl.ArgumentBuffer;
                for (int index = 0; index < parameterTypes.Length; ++index)
                {
                    Type targetType = parameterTypes[index];
                    object obj = args[index];
                    if ((object)obj.GetType() != (object)targetType)
                    {
                        if (!this.CoerceType(obj, targetType, ref parameters[index]))
                            Errors.Raise("Argument type mismatch for argument '{0}' of '{1}'", (ushort)1, (ushort)1, (object)index, (object)funcDecl.method.Name);
                    }
                    else
                        parameters[index] = obj;
                }
            }
            object obj1 = (object)null;
            try
            {
                obj1 = funcDecl.method.Invoke(funcDecl.instance, parameters);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Errors.Raise(ex.InnerException.Message, (ushort)1, (ushort)1);
                Errors.Raise(ex.Message, (ushort)1, (ushort)1);
            }
            return obj1;
        }

        public void Stop()
        {
            lock (this.currentScope)
                this.stopExecution = true;
        }

        private void CheckStopExecution()
        {
            lock (this.currentScope)
            {
                if (this.stopExecution)
                    throw new AbortExecutionException();
            }
        }

        private object Execute(ScriptAst ast)
        {
            this.stepOver = 0;
            this.Execute(ast.constants);
            this.Execute(ast.script);
            return (object)null;
        }

        private object Execute(StatementBlockAst block)
        {
            if ((this.stopCheckCounter++ & 15) == 0)
                this.CheckStopExecution();
            foreach (StatementAst statement in block.statements)
            {
                object obj = AstExecutor.execTable[(int)statement.NodeType](this, (AstNode)statement);
                switch (obj)
                {
                    case AstExecutor.ContinueLoop _:
                    case AstExecutor.BreakLoop _:
                    case AstExecutor.ReturnValue _:
                        return obj;
                    default:
                        continue;
                }
            }
            if (this.DebugCallback != null && block.endBody != null)
                this.StepDebugger((AstNode)block.endBody);
            return (object)null;
        }

        private object Execute(FloatAst ast) => (object)ast.value;

        private object Execute(IntAst ast) => (object)ast.value;

        private object Execute(StringAst ast) => (object)ast.value;

        private object Execute(ArrayLiteralAst ast)
        {
            ArrayValue arrayValue = new ArrayValue(ast.expressions.Length);
            foreach (ExpressionAst expression in ast.expressions)
                arrayValue.Add(AstExecutor.execTable[(int)expression.NodeType](this, (AstNode)expression));
            return (object)arrayValue;
        }

        private object ExecutePre(IndexAccessorAst ast)
        {
            if (ast.left is VariableAccessAst left && this.IsConstant(left.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.left.NodeType](this, (AstNode)ast.left);
                ast.left = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast.left);
            }
            if (ast.index is VariableAccessAst index && this.IsConstant(index.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.index.NodeType](this, (AstNode)ast.index);
                ast.index = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast.index);
            }
            ast.NodeType = AstNodeType.IndexAccessor;
            return this.Execute(ast);
        }

        private object Execute(IndexAccessorAst ast)
        {
            object obj1 = AstExecutor.execTable[(int)ast.left.NodeType](this, (AstNode)ast.left);
            object obj2 = AstExecutor.execTable[(int)ast.index.NodeType](this, (AstNode)ast.index);
            try
            {
                if (obj2 is double)
                    obj2 = (object)(int)(double)obj2;
                switch (obj1)
                {
                    case Array array2:
                        return array2.GetValue((int)obj2);
                    case string str2:
                        return (object)str2[(int)obj2];
                    case ArrayList arrayList2:
                        return arrayList2[(int)obj2];
                    default:
                        Errors.Raise("Only index arrays or strings", (AstNode)ast.left);
                        break;
                }
            }
            catch (Exception ex)
            {
                switch (ex)
                {
                    case ArgumentOutOfRangeException _:
                    case IndexOutOfRangeException _:
                        Errors.Raise("Index out of bounds", (AstNode)ast.index);
                        break;
                }
            }
            return (object)null;
        }

        private object Execute(NegateAst ast)
        {
            switch (AstExecutor.execTable[(int)ast.inner.NodeType](this, (AstNode)ast.inner))
            {
                case int num1:
                    return (object)-num1;
                case double num2:
                    return (object)-num2;
                default:
                    Errors.Raise("You cannot negate a non-numeric value", (AstNode)ast.inner);
                    return (object)null;
            }
        }

        private object ExecutePre(VarDeclAst ast)
        {
            if (!ast.IsConst && this.IsConstant(ast.variableName))
                Errors.Raise("You cannot change the value of the constant '{0}'", (AstNode)ast, (object)ast.variableName);
            if (ast.expression is VariableAccessAst expression && this.IsConstant(expression.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
                ast.expression = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast.expression);
            }
            ast.NodeType = AstNodeType.VarDecl;
            return this.Execute(ast);
        }

        private object Execute(VarDeclAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.expression);
            }
            object obj = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
            if (obj == null)
                Errors.Raise("No result returned", (AstNode)ast.expression);
            if (ast.IsConst)
                this.AddConstant(ast.variableName, obj);
            else
                this.currentScope.Add(ast.variableName, obj);
            return (object)null;
        }

        private object ExecutePre(AssignmentAst ast)
        {
            if (ast.expression is VariableAccessAst expression && this.IsConstant(expression.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
                ast.expression = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast.expression);
            }
            ast.NodeType = AstNodeType.Assignement;
            return this.Execute(ast);
        }

        private object Execute(AssignmentAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            if (ast.indexer != null)
            {
                IndexAccessorAst indexAccessorAst1 = ast.indexer;
                try
                {
                    ArrayValue arrayValue6;
                    var index8 = 0;
                    while (true)
                    {

                        object obj1 = AstExecutor.execTable[(int)indexAccessorAst1.left.NodeType](this, (AstNode)indexAccessorAst1.left);
                        object obj2 = AstExecutor.execTable[(int)indexAccessorAst1.index.NodeType](this, (AstNode)indexAccessorAst1.index);
                        if (obj1 is ArrayValue arrayValueX)
                        {
                            arrayValue6 = arrayValueX;
                            index8 = 0;
                            switch (obj2)
                            {
                                case double num8:
                                    index8 = (int)num8;
                                    goto label_8;
                                case int obj_int:
                                    index8 = obj_int;
                                label_8:
                                    if (arrayValue6[index8] is IndexAccessorAst indexAccessorAst11)
                                    {
                                        indexAccessorAst1 = indexAccessorAst11;
                                        continue;
                                    }
                                    goto label_10;
                                default:
                                    Errors.Raise("Index must be numeric", (AstNode)indexAccessorAst1.index);
                                    goto label_8;
                            }
                        }
                        else
                            Errors.Raise("Invalid operator", (AstNode)indexAccessorAst1);
                    }
                label_10:
                    arrayValue6[index8] = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
                }
                catch (Exception ex)
                {
                    switch (ex)
                    {
                        case ArgumentOutOfRangeException _:
                        case IndexOutOfRangeException _:
                            Errors.Raise("Index out of bounds", (AstNode)indexAccessorAst1);
                            break;
                    }
                    throw;
                }
            }
            else
            {
                if (this.IsConstant(ast.variableName))
                    Errors.Raise("You cannot change the value of the constant '{0}'", (AstNode)ast, (object)ast.variableName);
                if (this.DebugCallback != null)
                    this.StepDebugger((AstNode)ast.expression);
                object obj = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
                if (obj == null)
                    Errors.Raise("No result returned", (AstNode)ast.expression);
                if (!this.currentScope.Assign(ast.variableName, obj))
                    Errors.Raise("Identifier not declared '{0}'", (AstNode)ast, (object)ast.variableName);
            }
            return (object)null;
        }

        private object Execute(VariableAccessAst ast)
        {
            object obj = this.currentScope.Get(ast.variableName);
            if (obj == null)
                Errors.Raise("Identifier not declared '{0}'", (AstNode)ast, (object)ast.variableName);
            return obj;
        }

        private object Execute(IfAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.condition);
            }
            object obj1 = AstExecutor.execTable[(int)ast.condition.NodeType](this, (AstNode)ast.condition);
            if (obj1 == null)
                Errors.Raise("No result returned", (AstNode)ast.condition);
            if (!this.IsTrue(obj1))
                return (object)null;
            this.EnterScope(Scope.FromCache((string)null, this.currentScope));
            try
            {
                object obj2 = AstExecutor.execTable[(int)ast.body.NodeType](this, (AstNode)ast.body);
                switch (obj2)
                {
                    case AstExecutor.BreakLoop _:
                    case AstExecutor.ContinueLoop _:
                    case AstExecutor.ReturnValue _:
                        return obj2;
                    default:
                        return (object)true;
                }
            }
            finally
            {
                this.LeaveScope();
            }
        }

        private object Execute(IfElseAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.condition);
            }
            object obj1 = AstExecutor.execTable[(int)ast.condition.NodeType](this, (AstNode)ast.condition);
            if (obj1 == null)
                Errors.Raise("No result returned", (AstNode)ast.condition);
            if (this.IsTrue(obj1))
            {
                this.EnterScope(Scope.FromCache((string)null, this.currentScope));
                try
                {
                    object obj2 = AstExecutor.execTable[(int)ast.body.NodeType](this, (AstNode)ast.body);
                    switch (obj2)
                    {
                        case AstExecutor.BreakLoop _:
                        case AstExecutor.ContinueLoop _:
                        case AstExecutor.ReturnValue _:
                            return obj2;
                        default:
                            return (object)true;
                    }
                }
                finally
                {
                    this.LeaveScope();
                }
            }
            else
            {
                this.EnterScope(Scope.FromCache((string)null, this.currentScope));
                try
                {
                    return AstExecutor.execTable[(int)ast.elseBody.NodeType](this, (AstNode)ast.elseBody);
                }
                finally
                {
                    this.LeaveScope();
                }
            }
        }

        private object Execute(IfElseIfAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.condition);
            }
            object obj1 = AstExecutor.execTable[(int)ast.condition.NodeType](this, (AstNode)ast.condition);
            if (obj1 == null)
                Errors.Raise("No result returned", (AstNode)ast.condition);
            if (this.IsTrue(obj1))
            {
                this.EnterScope(Scope.FromCache((string)null, this.currentScope));
                try
                {
                    object obj2 = AstExecutor.execTable[(int)ast.body.NodeType](this, (AstNode)ast.body);
                    switch (obj2)
                    {
                        case AstExecutor.BreakLoop _:
                        case AstExecutor.ContinueLoop _:
                        case AstExecutor.ReturnValue _:
                            return obj2;
                        default:
                            return (object)true;
                    }
                }
                finally
                {
                    this.LeaveScope();
                }
            }
            else
            {
                foreach (IfAst elseIf in ast.elseIfList)
                {
                    object obj2 = AstExecutor.execTable[(int)elseIf.NodeType](this, (AstNode)elseIf);
                    switch (obj2)
                    {
                        case AstExecutor.ContinueLoop _:
                        case AstExecutor.BreakLoop _:
                        case AstExecutor.ReturnValue _:
                            return obj2;
                        case bool flag4:
                            if (flag4)
                                return (object)true;
                            break;
                    }
                }
                if (ast.elseBody == null)
                    return (object)null;
                this.EnterScope(Scope.FromCache((string)null, this.currentScope));
                try
                {
                    return AstExecutor.execTable[(int)ast.elseBody.NodeType](this, (AstNode)ast.elseBody);
                }
                finally
                {
                    this.LeaveScope();
                }
            }
        }

        private object Execute(WhileAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.condition);
            }
            object obj1 = AstExecutor.execTable[(int)ast.condition.NodeType](this, (AstNode)ast.condition);
            if (obj1 == null)
                Errors.Raise("No result returned", (AstNode)ast.condition);
            if (this.IsTrue(obj1))
            {
                try
                {
                    this.EnterScope(Scope.FromCache((string)null, this.currentScope));
                    do
                    {
                        object obj2 = AstExecutor.execTable[(int)ast.body.NodeType](this, (AstNode)ast.body);
                        if (!(obj2 is AstExecutor.BreakLoop))
                        {
                            if (obj2 is AstExecutor.ReturnValue)
                                return obj2;
                            if (this.DebugCallback != null)
                            {
                                this.StepDebugger((AstNode)ast);
                                this.StepDebugger((AstNode)ast.condition);
                            }
                        }
                        else
                            break;
                    }
                    while (this.IsTrue(AstExecutor.execTable[(int)ast.condition.NodeType](this, (AstNode)ast.condition)));
                }
                finally
                {
                    this.LeaveScope();
                }
            }
            return (object)null;
        }

        private object Execute(FuncDeclAst ast)
        {
            ast.parentScope = this.currentScope;
            this.currentScope.Add(ast.functionName, (object)ast);
            return (object)null;
        }

        private object Execute(FuncCallAst ast)
        {
            AstNode astNode = (AstNode)AstExecutor.execTable[(int)ast.functionExpression.NodeType](this, (AstNode)ast.functionExpression);
            switch (astNode.NodeType)
            {
                case AstNodeType.FuncDecl:
                    return this.ExecuteFunction((FuncDeclAst)astNode, ast);
                case AstNodeType.BuiltInDecl:
                    return this.ExecuteBuiltIn((BuiltInDeclAst)astNode, ast);
                default:
                    Errors.Raise("Function not declared '{0}'", (AstNode)ast);
                    return (object)null;
            }
        }

        private object ExecuteFunction(FuncDeclAst funcDecl, FuncCallAst call)
        {
            if (funcDecl.parameters.Length != call.arguments.Length)
                Errors.Raise("Argument count mismatch, Expected '{0}' got '{1}'", (AstNode)call, (object)funcDecl.parameters.Length, (object)call.arguments.Length);
            Scope newScope = Scope.FromCache(string.Intern(funcDecl.functionName), funcDecl.parentScope);
            if (funcDecl.parameters.Length != 0)
            {
                for (int index = 0; index < funcDecl.parameters.Length; ++index)
                {
                    ExpressionAst expressionAst = call.arguments[index];
                    if (this.DebugCallback != null)
                        this.StepDebugger((AstNode)expressionAst);
                    object obj = AstExecutor.execTable[(int)call.arguments[index].NodeType](this, (AstNode)expressionAst);
                    if (obj == null)
                        Errors.Raise("No result returned", (AstNode)expressionAst);
                    newScope.Add(funcDecl.parameters[index], obj);
                }
                if (this.DebugCallback != null)
                    this.StepDebugger((AstNode)call);
            }
            this.EnterScope(newScope);
            try
            {
                object obj = AstExecutor.execTable[(int)funcDecl.body.NodeType](this, (AstNode)funcDecl.body);
                if (this.stepOver == call.siteId)
                    this.stepOver = 0;
                if (obj is AstExecutor.ReturnValue returnValue2)
                    obj = returnValue2.value;
                return obj;
            }
            finally
            {
                this.LeaveScope();
            }
        }

        private object ExecuteBuiltIn(BuiltInDeclAst funcDecl, FuncCallAst call)
        {
            Type[] parameterTypes = funcDecl.parameterTypes;
            if (parameterTypes.Length != call.arguments.Length)
                Errors.Raise("Argument count mismatch, Expected '{0}' got '{1}'", (AstNode)call, (object)parameterTypes.Length, (object)call.arguments.Length);
            object[] parameters = (object[])null;
            if (parameterTypes.Length != 0)
            {
                parameters = funcDecl.ArgumentBuffer;
                for (int index = 0; index < parameterTypes.Length; ++index)
                {
                    Type targetType = parameterTypes[index];
                    ExpressionAst expressionAst = call.arguments[index];
                    if (this.DebugCallback != null)
                        this.StepDebugger((AstNode)expressionAst);
                    object obj = AstExecutor.execTable[(int)call.arguments[index].NodeType](this, (AstNode)expressionAst);
                    if (obj == null)
                        Errors.Raise("No result returned", (AstNode)expressionAst);
                    if ((object)obj.GetType() != (object)targetType)
                    {
                        if (!this.CoerceType(obj, targetType, ref parameters[index]))
                            Errors.Raise("Argument type mismatch for argument '{0}' of '{1}'", (AstNode)call, (object)index, (object)funcDecl.method.Name);
                    }
                    else
                        parameters[index] = obj;
                }
                if (this.DebugCallback != null)
                    this.StepDebugger((AstNode)call);
            }
            object obj1 = (object)null;
            try
            {
                obj1 = funcDecl.method.Invoke(funcDecl.instance, parameters);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Errors.Raise(ex.InnerException.Message, (AstNode)call);
                Errors.Raise(ex.Message, (AstNode)call);
            }
            if (this.stepOver == call.siteId)
                this.stepOver = 0;
            return obj1;
        }

        private bool CoerceType(object value, Type targetType, ref object coercedValue)
        {
            Type type = value.GetType();
            if ((object)targetType != (object)typeof(object) && (object)type != (object)targetType)
            {
                if ((object)targetType == (object)typeof(int) && (object)type == (object)typeof(double))
                {
                    coercedValue = (object)(int)(double)value;
                    return true;
                }
                if ((object)targetType == (object)typeof(double) && (object)value.GetType() == (object)typeof(int))
                {
                    coercedValue = (object)(double)(int)value;
                    return true;
                }
                if ((object)targetType != (object)typeof(ArrayList) || (object)value.GetType() != (object)typeof(ArrayValue))
                    return false;
                coercedValue = (object)(ArrayList)value;
                return true;
            }
            coercedValue = value;
            return true;
        }

        private object Execute(ReturnAst ast)
        {
            if (this.DebugCallback != null)
            {
                this.StepDebugger((AstNode)ast);
                this.StepDebugger((AstNode)ast.expression);
            }
            object obj = AstExecutor.execTable[(int)ast.expression.NodeType](this, (AstNode)ast.expression);
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            return (object)AstExecutor.ReturnValue.Return(obj);
        }

        private object Execute(BreakAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            return (object)AstExecutor.BreakLoop.Value;
        }

        private object Execute(ContinueAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            return (object)AstExecutor.ContinueLoop.Value;
        }

        private object Execute(LocateAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            object coercedValue1 = AstExecutor.execTable[(int)ast.screenRow.NodeType](this, (AstNode)ast.screenRow);
            object coercedValue2 = AstExecutor.execTable[(int)ast.screenCol.NodeType](this, (AstNode)ast.screenCol);
            if (!this.CoerceType(coercedValue1, typeof(int), ref coercedValue1))
                Errors.Raise("Expected '{0}' type", (AstNode)ast.screenRow, (object)"numberic");
            if (!this.CoerceType(coercedValue2, typeof(int), ref coercedValue2))
                Errors.Raise("Expected '{0}' type", (AstNode)ast.screenCol, (object)"numberic");
            try
            {
                Stdlib.Locate((int)coercedValue1, (int)coercedValue2);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Errors.Raise(ex.InnerException.Message, (AstNode)ast);
                Errors.Raise(ex.Message, (AstNode)ast);
            }
            return (object)null;
        }

        private object Execute(PrintAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            try
            {
                if (ast.argument != null)
                {
                    object obj = AstExecutor.execTable[(int)ast.argument.NodeType](this, (AstNode)ast.argument);
                    if (obj is ArrayList arrayList4)
                        obj = (object)ArrayValue.ToString((ICollection)arrayList4);
                    Stdlib.Print(obj ?? (object)string.Empty);
                }
                else
                    Stdlib.Print((object)string.Empty);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Errors.Raise(ex.InnerException.Message, (AstNode)ast);
                Errors.Raise(ex.Message, (AstNode)ast);
            }
            return (object)null;
        }

        private object Execute(ClsAst ast)
        {
            if (this.DebugCallback != null)
                this.StepDebugger((AstNode)ast);
            try
            {
                Stdlib.Cls();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    Errors.Raise(ex.InnerException.Message, (AstNode)ast);
                Errors.Raise(ex.Message, (AstNode)ast);
            }
            return (object)null;
        }

        private static bool IsConstant(AstNode x) => x.NodeType == AstNodeType.Float || x.NodeType == AstNodeType.Int || (x.NodeType == AstNodeType.String || x.NodeType == AstNodeType.ConstValue) || x.NodeType == AstNodeType.BinOpConst;

        private object ExecutePre(BinOpAst ast)
        {
            if (ast.left is VariableAccessAst left && this.IsConstant(left.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.left.NodeType](this, (AstNode)ast.left);
                ast.left = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast);
            }
            if (ast.right is VariableAccessAst right && this.IsConstant(right.variableName))
            {
                object obj = AstExecutor.execTable[(int)ast.right.NodeType](this, (AstNode)ast.right);
                ast.right = (ExpressionAst)new ConstValueAst(obj, (AstNode)ast);
            }
            object obj1 = this.Execute(ast);
            if (AstExecutor.IsConstant((AstNode)ast.left) && AstExecutor.IsConstant((AstNode)ast.right))
            {
                ast.NodeType = AstNodeType.BinOpConst;
                ast.left = (ExpressionAst)new ConstValueAst(obj1, (AstNode)ast);
                ast.op = TokenType.None;
                ast.right = (ExpressionAst)null;
            }
            else
                ast.NodeType = AstNodeType.BinOp;
            return obj1;
        }

        private object ExecuteConst(BinOpAst ast) => ((ConstValueAst)ast.left).value;

        private object Execute(BinOpAst ast)
        {
            if (ast.op == TokenType.And || ast.op == TokenType.Or)
            {
                bool flag = this.IsTrue(AstExecutor.execTable[(int)ast.left.NodeType](this, (AstNode)ast.left));
                if (ast.op == TokenType.And && !flag)
                    return (object)0;
                return ast.op == TokenType.Or & flag ? (object)1 : (object)(this.IsTrue(AstExecutor.execTable[(int)ast.right.NodeType](this, (AstNode)ast.right)) ? 1 : 0);
            }
            object obj1 = AstExecutor.execTable[(int)ast.left.NodeType](this, (AstNode)ast.left);
            object obj2 = AstExecutor.execTable[(int)ast.right.NodeType](this, (AstNode)ast.right);
            switch (obj1)
            {
                case int num6 when obj2 is int num5:
                    switch (ast.op)
                    {
                        case TokenType.Plus:
                            return (object)(num6 + num5);
                        case TokenType.Minus:
                            return (object)(num6 - num5);
                        case TokenType.Times:
                            return (object)(num6 * num5);
                        case TokenType.Divide:
                            return (object)(num6 / num5);
                        case TokenType.Mod:
                            return (object)(num6 % num5);
                        case TokenType.Eq:
                            return (object)(num6 == num5 ? 1 : 0);
                        case TokenType.Neq:
                            return (object)(num6 != num5 ? 1 : 0);
                        case TokenType.Lt:
                            return (object)(num6 < num5 ? 1 : 0);
                        case TokenType.Leq:
                            return (object)(num6 <= num5 ? 1 : 0);
                        case TokenType.Gt:
                            return (object)(num6 > num5 ? 1 : 0);
                        case TokenType.Geq:
                            return (object)(num6 >= num5 ? 1 : 0);
                    }
                    break;
                case double num8 when obj2 is double num7:
                    switch (ast.op)
                    {
                        case TokenType.Plus:
                            return (object)(num8 + num7);
                        case TokenType.Minus:
                            return (object)(num8 - num7);
                        case TokenType.Times:
                            return (object)(num8 * num7);
                        case TokenType.Divide:
                            return (object)(num8 / num7);
                        case TokenType.Mod:
                            return (object)((int)num8 % (int)num7);
                        case TokenType.Eq:
                            return (object)(num8 == num7 ? 1 : 0);
                        case TokenType.Neq:
                            return (object)(num8 != num7 ? 1 : 0);
                        case TokenType.Lt:
                            return (object)(num8 < num7 ? 1 : 0);
                        case TokenType.Leq:
                            return (object)(num8 <= num7 ? 1 : 0);
                        case TokenType.Gt:
                            return (object)(num8 > num7 ? 1 : 0);
                        case TokenType.Geq:
                            return (object)(num8 >= num7 ? 1 : 0);
                    }
                    break;
                case int _:
                case double _ when obj2 is int || obj2 is double:
                    double num1 = obj1 is int num9 ? (double)num9 : (double)obj1;
                    double num3 = obj2 is int num10 ? (double)num10 : (double)obj2;
                    switch (ast.op)
                    {
                        case TokenType.Plus:
                            return (object)(num1 + num3);
                        case TokenType.Minus:
                            return (object)(num1 - num3);
                        case TokenType.Times:
                            return (object)(num1 * num3);
                        case TokenType.Divide:
                            return (object)(num1 / num3);
                        case TokenType.Mod:
                            return (object)((int)num1 % (int)num3);
                        case TokenType.Eq:
                            return (object)(num1 == num3 ? 1 : 0);
                        case TokenType.Neq:
                            return (object)(num1 != num3 ? 1 : 0);
                        case TokenType.Lt:
                            return (object)(num1 < num3 ? 1 : 0);
                        case TokenType.Leq:
                            return (object)(num1 <= num3 ? 1 : 0);
                        case TokenType.Gt:
                            return (object)(num1 > num3 ? 1 : 0);
                        case TokenType.Geq:
                            return (object)(num1 >= num3 ? 1 : 0);
                    }
                    break;
                default:
                    if (obj1 is string || obj2 is string)
                    {
                        string strA = obj1.ToString();
                        string strB = obj2.ToString();
                        switch (ast.op)
                        {
                            case TokenType.Plus:
                                return (object)(strA + strB);
                            case TokenType.Eq:
                                return (object)(strA == strB ? 1 : 0);
                            case TokenType.Neq:
                                return (object)(strA != strB ? 1 : 0);
                            case TokenType.Lt:
                                return (object)(string.Compare(strA, strB) < 0 ? 1 : 0);
                            case TokenType.Leq:
                                return (object)(string.Compare(strA, strB) <= 0 ? 1 : 0);
                            case TokenType.Gt:
                                return (object)(string.Compare(strA, strB) > 0 ? 1 : 0);
                            case TokenType.Geq:
                                return (object)(string.Compare(strA, strB) >= 0 ? 1 : 0);
                        }
                    }
                    else
                    {
                        if (obj1 is ArrayValue arrayValue7 && obj2 is ArrayValue arrayValue8 && ast.op == TokenType.Plus)
                        {
                            ArrayList arrayList = new ArrayList();
                            arrayList.Capacity = arrayValue7.Count + arrayValue8.Count;
                            arrayList.AddRange((ICollection)arrayValue7);
                            arrayList.AddRange((ICollection)arrayValue8);
                            return (object)new ArrayValue((ICollection)arrayList);
                        }
                        break;
                    }
                    break;
            }
            Errors.Raise("Invalid operator", (AstNode)ast);
            return (object)null;
        }

        private bool IsTrue(object value)
        {
            switch (value)
            {
                case double num1:
                    return num1 != 0.0;
                case int num2:
                    return (uint)num2 > 0U;
                default:
                    return false;
            }
        }

        private void EnterScope(Scope newScope)
        {
            this.scopeStack.Push((object)this.currentScope);
            this.currentScope = newScope;
        }

        private void LeaveScope()
        {
            if (this.scopeStack.Count == 0)
                throw new InvalidOperationException();
            Scope.Release(this.currentScope);
            this.currentScope = (Scope)this.scopeStack.Pop();
        }

        public DebugCallback DebugCallback { get; set; }

        public ushort SrcLine { get; private set; }

        public ushort SrcCol { get; private set; }

        public ushort SrcLength { get; private set; }

        public string[] GetCallStack()
        {
            ArrayList arrayList = new ArrayList();
            if (!string.IsNullOrEmpty(this.currentScope.Name) && this.currentScope.Name[0] != '<')
                arrayList.Add((object)this.currentScope.Name);
            foreach (Scope scope in this.scopeStack)
            {
                if (!string.IsNullOrEmpty(scope.Name) && scope.Name[0] != '<')
                    arrayList.Add((object)scope.Name);
            }
            return (string[])arrayList.ToArray(typeof(string));
        }

        public Variable[] GetGlobalVariables() => this.scriptScope.GetAllVariables();

        public string[] GetAllFunctions() => this.currentScope.GetAllFunctions();

        public Variable[] GetLocals() => this.currentScope.GetLocals();

        public Variable[][] GetAllInScope() => this.currentScope.GetAllInScope(this.globals);

        protected DebugAction ExecuteDebugCallback(AstNode ast)
        {
            if (ast != null)
            {
                this.SrcLine = ast.Line;
                this.SrcCol = ast.Col;
                this.SrcLength = ast.Length;
            }
            return this.DebugCallback();
        }

        private void StepDebugger(AstNode ast)
        {
            this.HitBreakpoint = (int)ast.Line != this.lastLine && this.breakpoints.Contains((object)ast.Line);
            if ((this.lastAction != DebugAction.Continue || this.HitBreakpoint) && (this.stepOver == 0 || this.HitBreakpoint))
            {
                if (this.HitBreakpoint)
                    this.stepOver = 0;
                DebugAction debugAction = this.ExecuteDebugCallback(ast);
                this.lastAction = debugAction;
                this.HitBreakpoint = false;
                if (debugAction != DebugAction.StepOver)
                {
                    if (debugAction == DebugAction.Stop)
                        throw new AbortExecutionException();
                }
                else if (ast is FuncCallAst funcCallAst4)
                    this.stepOver = funcCallAst4.siteId;
            }
            this.lastLine = (int)ast.Line;
        }

        public bool HitBreakpoint { get; private set; }

        public ushort[] GetBreakpoints()
        {
            ushort[] numArray = new ushort[this.breakpoints.Count];
            int num = 0;
            foreach (object key in (IEnumerable)this.breakpoints.Keys)
                numArray[num++] = (ushort)key;
            return numArray;
        }

        public void SetBreakpoints(ushort[] lines)
        {
            foreach (int line in lines)
                this.breakpoints[(object)(ushort)line] = (object)true;
        }

        public void SetBreakpoint(ushort line) => this.breakpoints[(object)line] = (object)true;

        public void ClearBreakpoint(ushort line)
        {
            if (!this.breakpoints.Contains((object)line))
                return;
            this.breakpoints.Remove((object)line);
        }

        private delegate object ExecuteFunc(AstExecutor e, AstNode ast);

        private class ReturnValue
        {
            private static AstExecutor.ReturnValue instance = new AstExecutor.ReturnValue();
            public object value;

            public static AstExecutor.ReturnValue Return(object value)
            {
                AstExecutor.ReturnValue.instance.value = value;
                return AstExecutor.ReturnValue.instance;
            }

            private ReturnValue()
            {
            }
        }

        private class BreakLoop
        {
            public static readonly AstExecutor.BreakLoop Value = new AstExecutor.BreakLoop();

            private BreakLoop()
            {
            }
        }

        private class ContinueLoop
        {
            public static readonly AstExecutor.ContinueLoop Value = new AstExecutor.ContinueLoop();

            private ContinueLoop()
            {
            }
        }
    }
}
