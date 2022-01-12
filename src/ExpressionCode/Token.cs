// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Token
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Text;

namespace ExpressionCode
{
  internal class Token
  {
    private static readonly Pool tokenPool = new Pool((Pool.FactoryFunc) (() => (object) new Token()), (Pool.ResetFunc) (t => ((Token) t).Reset()));
    public static readonly Token BuiltIn = new Token();
    protected static readonly StringBuilder Buffer = new StringBuilder(32);
    internal TokenType tokenType;
    public ushort line;
    public ushort col;
    public ushort length;
    public object value;

    public Token Clone()
    {
      Token token = (Token) Token.tokenPool.Acquire();
      token.tokenType = this.tokenType;
      token.value = this.value;
      token.line = this.line;
      token.col = this.col;
      token.length = this.length;
      return token;
    }

    public static void Release(Token token) => Token.tokenPool.Release((object) token);

    public static void ClearTokenPool() => Token.tokenPool.Clear();

    private void Reset()
    {
      this.tokenType = TokenType.Sof;
      this.value = (object) null;
      this.line = (ushort) 0;
      this.col = (ushort) 0;
      this.length = (ushort) 0;
    }

    public override string ToString() => string.Format("{0}:{1} <{2}>({3})", (object) this.line, (object) this.col, (object) this.tokenType, this.value);

    public bool Is(TokenType tokenType) => this.tokenType == tokenType;

    public bool IsNot(TokenType tokenType) => !this.Is(tokenType);

    public bool IsOneOf(params TokenType[] tokenTypes)
    {
      foreach (TokenType tokenType in tokenTypes)
      {
        if (this.tokenType == tokenType)
          return true;
      }
      return false;
    }

    public bool IsNotOneOf(params TokenType[] tokenTypes) => !this.IsOneOf(tokenTypes);

    public bool IsEof => this.tokenType == TokenType.Eof;

    public Token ScanNumber(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      bool flag1 = false;
      bool flag2 = false;
      char ch;
      if (scanner.CurrChar == '0')
      {
        ch = scanner.PeekChar;
        char lower = ch.ToLower();
        flag1 = lower == 'x';
        flag2 = lower == 'b';
        if (flag1 | flag2)
        {
          int num1 = (int) scanner.NextChar();
          int num2 = (int) scanner.NextChar();
        }
      }
      ch = scanner.CurrChar;
      for (char lower = ch.ToLower(); scanner.CurrChar.IsDigit() || flag1 && lower >= 'a' && lower <= 'f'; lower = ch.ToLower())
      {
        stringBuilder.Append(scanner.CurrChar);
        int num = (int) scanner.NextChar();
        ch = scanner.CurrChar;
      }
      bool flag3 = false;
      if (scanner.CurrChar == '.')
      {
        if (flag1)
          Errors.Raise("Invalid hex number", scanner);
        if (flag2)
          Errors.Raise("Invalid binary number", scanner);
        flag3 = true;
        stringBuilder.Append('.');
        int num1 = (int) scanner.NextChar();
        while (scanner.CurrChar.IsDigit())
        {
          stringBuilder.Append(scanner.CurrChar);
          int num2 = (int) scanner.NextChar();
        }
      }
      this.length = (ushort) (scanner.Pos - pos);
      if (flag3)
      {
        this.tokenType = TokenType.Float;
        try
        {
          this.value = (object) double.Parse(stringBuilder.ToString());
        }
        catch
        {
          Errors.Raise("Invalid number", this.line, this.col);
        }
      }
      else
      {
        this.tokenType = TokenType.Int;
        string s = stringBuilder.ToString();
        if (flag1)
        {
          try
          {
            this.value = (object) Convert.ToInt32(s, 16);
          }
          catch
          {
            Errors.Raise("Invalid hex number", this.line, this.col);
          }
        }
        else if (flag2)
        {
          int num = 0;
          for (int index = 0; index < s.Length; ++index)
          {
            if (s[index] != '1' && s[index] != '0')
              Errors.Raise("Invalid binary number", this.line, this.col);
            num = num * 2 + (s[index] == '1' ? 1 : 0);
          }
          this.value = (object) num;
        }
        else
        {
          try
          {
            this.value = (object) int.Parse(s);
          }
          catch
          {
            Errors.Raise("Invalid number", this.line, this.col);
          }
        }
      }
      return this;
    }

    public Token Indent(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = (ushort) 0;
      this.length = scanner.Col;
      this.tokenType = TokenType.Indent;
      return this;
    }

    public Token Outdent(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = scanner.Col;
      this.length = (ushort) 0;
      this.tokenType = TokenType.Outdent;
      return this;
    }

    public Token ScanSymbol(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = scanner.Col;
      int pos = scanner.Pos;
      char currChar = scanner.CurrChar;
      char peekChar = scanner.PeekChar;
      switch (currChar)
      {
        case '!':
          if (peekChar == '=')
          {
            this.tokenType = TokenType.Neq;
            this.value = (object) "!=";
            int num = (int) scanner.NextChar();
            break;
          }
          Errors.Raise("Expected {0}", scanner, (object) "!=");
          break;
        case '%':
          this.tokenType = TokenType.Mod;
          this.value = (object) "%";
          break;
        case '&':
          if (peekChar == '&')
          {
            this.tokenType = TokenType.And;
            this.value = (object) "&&";
            int num = (int) scanner.NextChar();
            break;
          }
          Errors.Raise("Expected {0}", scanner, (object) "&&");
          break;
        case '(':
          this.tokenType = TokenType.LParen;
          this.value = (object) "(";
          break;
        case ')':
          this.tokenType = TokenType.RParen;
          this.value = (object) ")";
          break;
        case '*':
          this.tokenType = TokenType.Times;
          this.value = (object) "*";
          break;
        case '+':
          this.tokenType = TokenType.Plus;
          this.value = (object) "+";
          break;
        case ',':
          this.tokenType = TokenType.Comma;
          this.value = (object) ",";
          break;
        case '-':
          this.tokenType = TokenType.Minus;
          this.value = (object) "-";
          break;
        case '/':
          this.tokenType = TokenType.Divide;
          this.value = (object) "/";
          break;
        case ':':
          this.tokenType = TokenType.Colon;
          this.value = (object) ":";
          break;
        case ';':
          this.tokenType = TokenType.SemiColon;
          this.value = (object) ";";
          break;
        case '<':
          switch (peekChar)
          {
            case '=':
              this.tokenType = TokenType.Leq;
              this.value = (object) "<=";
              int num1 = (int) scanner.NextChar();
              break;
            case '>':
              this.tokenType = TokenType.Neq;
              this.value = (object) "<>";
              int num2 = (int) scanner.NextChar();
              break;
            default:
              this.tokenType = TokenType.Lt;
              this.value = (object) "<";
              break;
          }
          break;
        case '=':
          if (peekChar == '=')
          {
            this.tokenType = TokenType.Eq;
            this.value = (object) "==";
            int num3 = (int) scanner.NextChar();
            break;
          }
          this.tokenType = TokenType.Assign;
          this.value = (object) "=";
          break;
        case '>':
          if (peekChar == '=')
          {
            this.tokenType = TokenType.Geq;
            this.value = (object) ">=";
            int num3 = (int) scanner.NextChar();
            break;
          }
          this.tokenType = TokenType.Gt;
          this.value = (object) ">";
          break;
        case '[':
          this.tokenType = TokenType.LBracket;
          this.value = (object) "[";
          break;
        case ']':
          this.tokenType = TokenType.RBracket;
          this.value = (object) "]";
          break;
        case '{':
          this.tokenType = TokenType.LBrace;
          this.value = (object) "{";
          break;
        case '|':
          if (peekChar == '|')
          {
            this.tokenType = TokenType.Or;
            this.value = (object) "||";
            int num3 = (int) scanner.NextChar();
            break;
          }
          Errors.Raise("Expected {0}", scanner, (object) "||");
          break;
        case '}':
          this.tokenType = TokenType.RBrace;
          this.value = (object) "}";
          break;
        default:
          throw new InterpreterException(string.Format("SymbolToken: Unexpected symbol '{0}'", new object[1]
          {
            (object) currChar
          }), this.line, this.col);
      }
      int num4 = (int) scanner.NextChar();
      this.length = (ushort) (scanner.Pos - pos);
      if (this.value is string key)
        this.value = (object) StringTable.Intern(key);
      return this;
    }

    public Token ScanWord(Scanner scanner)
    {
      this.tokenType = TokenType.Identifier;
      this.line = scanner.Line;
      this.col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      while (scanner.CurrChar.IsIdentifierChar())
      {
        stringBuilder.Append(scanner.CurrChar.ToLower());
        int num = (int) scanner.NextChar();
      }
      string key = stringBuilder.ToString();
      switch (key)
      {
        case "and":
          this.tokenType = TokenType.And;
          break;
        case "begin":
          this.tokenType = TokenType.Begin;
          break;
        case "break":
          this.tokenType = TokenType.Break;
          break;
        case "cls":
          this.tokenType = TokenType.Cls;
          break;
        case "const":
          this.tokenType = TokenType.ConstDecl;
          break;
        case "continue":
          this.tokenType = TokenType.Continue;
          break;
        case "else":
          this.tokenType = TokenType.Else;
          break;
        case "elseif":
          this.tokenType = TokenType.ElseIf;
          break;
        case "end":
          this.tokenType = TokenType.End;
          break;
        case "func":
          this.tokenType = TokenType.Func;
          break;
        case "if":
          this.tokenType = TokenType.If;
          break;
        case "locate":
          this.tokenType = TokenType.Locate;
          break;
        case "or":
          this.tokenType = TokenType.Or;
          break;
        case "print":
          this.tokenType = TokenType.Print;
          break;
        case "return":
          this.tokenType = TokenType.Return;
          break;
        case "var":
          this.tokenType = TokenType.VarDecl;
          break;
        case "while":
          this.tokenType = TokenType.While;
          break;
        default:
          this.value = (object) StringTable.Intern(key);
          break;
      }
      this.length = (ushort) (scanner.Pos - pos);
      return this;
    }

    public Token ScanString(Scanner scanner)
    {
      this.tokenType = TokenType.String;
      this.line = scanner.Line;
      this.col = scanner.Col;
      int pos = scanner.Pos;
      StringBuilder stringBuilder = Token.Buffer.Clear();
      char currChar = scanner.CurrChar;
      int num1 = (int) scanner.NextChar();
      while (scanner.CurrChar.IsPrintable() && ((int) scanner.CurrChar != (int) currChar || (int) scanner.PeekChar == (int) currChar))
      {
        stringBuilder.Append(scanner.CurrChar);
        int num2 = (int) scanner.NextChar();
      }
      if ((int) scanner.CurrChar != (int) currChar)
        Errors.Raise("Unexpected '{0}'", scanner, (object) scanner.CurrChar);
      int num3 = (int) scanner.NextChar();
      this.value = (object) StringTable.Intern(stringBuilder.ToString());
      this.length = (ushort) (scanner.Pos - pos);
      return this;
    }

    public Token Sof(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = scanner.Col;
      this.tokenType = TokenType.Sof;
      this.value = (object) "SOF";
      return this;
    }

    public Token Eof(Scanner scanner)
    {
      this.line = scanner.Line;
      this.col = (ushort) ((uint) scanner.Col + 1U);
      this.tokenType = TokenType.Eof;
      this.value = (object) "EOF";
      return this;
    }
  }
}
