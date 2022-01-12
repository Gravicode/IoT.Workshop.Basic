// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Errors
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class Errors
  {
    public const string Unexpected = "Unexpected '{0}'";
    public const string NegateNonNumeric = "You cannot negate a non-numeric value";
    public const string AssignToConstant = "You cannot change the value of the constant '{0}'";
    public const string InvalidConstDeclaration = "Constants must be declared before any other statements";
    public const string IdentifierNotDeclared = "Identifier not declared '{0}'";
    public const string FunctionNotDeclared = "Function not declared '{0}'";
    public const string ArgumentCountMismatch = "Argument count mismatch, Expected '{0}' got '{1}'";
    public const string ArgumentTypeMismatch = "Argument type mismatch for argument '{0}' of '{1}'";
    public const string TypeMismatch = "Expected '{0}' type";
    public const string InvalidBuiltInReturnType = "Invalid return type for builtin method '{0}'";
    public const string InvalidBuiltInParameterType = "Invalid parameter type for builtin method '{0}' argument '{1}'";
    public const string Expected = "Expected {0}";
    public const string OnlyIndexArraysOrStrings = "Only index arrays or strings";
    public const string IndexOutOfBounds = "Index out of bounds";
    public const string InvalidOperator = "Invalid operator";
    public const string IndexMustBeNumeric = "Index must be numeric";
    public const string NoResult = "No result returned";
    public const string CallbackNotAFunction = "Callback is not a function";
    public const string SyntaxError = "Syntax error";
    public const string InvalidBinaryNumber = "Invalid binary number";
    public const string InvalidHexNumber = "Invalid hex number";
    public const string InvalidNumber = "Invalid number";

    public static object Raise(string message, Scanner scanner, params object[] args) => Errors.Raise(message, scanner.Line, scanner.Col, args);

    public static object Raise(string message, Token token, params object[] args) => Errors.Raise(message, token.line, token.col, args);

    public static object Raise(string message, AstNode ast, params object[] args) => Errors.Raise(message, ast.Line, ast.Col, args);

    public static object Raise(string message, ushort line, ushort col, params object[] args) => throw new InterpreterException(string.Format("Error at {0}:{1} {2}", new object[3]
    {
      (object) line,
      (object) col,
      (object) string.Format(message, args)
    }), line, col);
  }
}
