// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Scanner
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using ExpressionCode.IO;
using System.Collections;

namespace ExpressionCode
{
  internal class Scanner
  {
    public const char EOF = '\0';
    private TextReaderEx rdr;
    private int currPos;
    private int readPos;
    private int lineStart;
    private Token[] tokens = new Token[4];
    private int tokenIndex;
    private bool indentedBlocks;
    private int indentDepth;
    private int indentBlockStart;
    private readonly Stack indentStack = new Stack();

    public char CurrChar { get; private set; }

    public char PeekChar { get; private set; }

    public ushort Line { get; private set; }

    public ushort Col => (ushort) (this.currPos - this.lineStart + 1);

    public int Pos => this.currPos;

    public Scanner(TextReaderEx rdr)
    {
      this.currPos = -1;
      this.lineStart = 0;
      this.readPos = -1;
      this.rdr = rdr;
      this.Line = (ushort) 1;
      this.CurrChar = char.MinValue;
      this.PeekChar = this.ScanChar();
      int num = (int) this.NextChar();
      this.tokens[0] = new Token().Sof(this);
      this.tokens[1] = new Token().Sof(this);
      this.tokens[2] = new Token().Sof(this);
      this.tokens[3] = new Token().Sof(this);
      this.tokenIndex = 0;
    }

    public char NextChar()
    {
      this.CurrChar = this.PeekChar;
      this.PeekChar = this.ScanChar();
      return this.CurrChar;
    }

    private char ScanChar()
    {
      if (this.rdr.Peek() < 0)
      {
        this.currPos = this.readPos;
        return char.MinValue;
      }
      this.currPos = this.readPos++;
      return (char) this.rdr.Read();
    }

    private Token CurrTok => this.tokens[this.tokenIndex];

    public Token GetToken()
    {
      if (this.CurrTok.IsEof)
        return this.CurrTok;
      this.tokenIndex = this.tokenIndex + 1 & 3;
      return this.ScanToken();
    }

    private Token ScanToken()
    {
      while (this.CurrChar != char.MinValue)
      {
        switch (this.CurrChar)
        {
          case '\n':
            int num1 = (int) this.NextChar();
            if (this.CurrChar == '\r')
            {
              int num2 = (int) this.NextChar();
            }
            this.lineStart = this.currPos;
            ++this.Line;
            this.indentDepth = 0;
            continue;
          case '\r':
            int num3 = (int) this.NextChar();
            if (this.CurrChar == '\n')
            {
              int num4 = (int) this.NextChar();
            }
            this.lineStart = this.currPos;
            ++this.Line;
            this.indentDepth = 0;
            continue;
          case '!':
          case '%':
          case '&':
          case '(':
          case ')':
          case '*':
          case '+':
          case ',':
          case '-':
          case ';':
          case '<':
          case '=':
          case '>':
          case '[':
          case ']':
          case '{':
          case '|':
          case '}':
            return this.CurrTok.ScanSymbol(this);
          case '"':
          case '\'':
            return this.CurrTok.ScanString(this);
          case '#':
            this.SkipLine();
            continue;
          case '/':
            if (this.PeekChar != '/')
              return this.CurrTok.ScanSymbol(this);
            this.SkipLine();
            continue;
          case ':':
            this.indentedBlocks = true;
            this.indentBlockStart = this.indentDepth;
            return this.CurrTok.ScanSymbol(this);
          default:
            if (this.CurrChar.IsWhiteSpace())
            {
              bool flag = this.Col == (ushort) 1;
              while (this.CurrChar.IsWhiteSpace())
              {
                int num5 = (int) this.NextChar();
                if (flag)
                  ++this.indentDepth;
              }
              continue;
            }
            if (this.CurrChar.IsDigit())
              return this.CurrTok.ScanNumber(this);
            if (this.CurrChar.IsLetter())
            {
              if (this.indentedBlocks && this.indentDepth > this.indentBlockStart)
              {
                this.indentBlockStart = -1;
                if (this.indentDepth > this.LastIndent())
                {
                  this.indentStack.Push((object) this.indentDepth);
                  return this.CurrTok.Indent(this);
                }
                if (this.indentDepth < this.LastIndent())
                {
                  this.indentStack.Pop();
                  if (this.indentStack.Count == 0)
                    this.indentedBlocks = false;
                  return this.CurrTok.Outdent(this);
                }
              }
              return this.CurrTok.ScanWord(this);
            }
            Errors.Raise("Unexpected '{0}'", this, (object) this.CurrChar);
            continue;
        }
      }
      if (this.indentStack.Count <= 0)
        return this.CurrTok.Eof(this);
      this.indentStack.Pop();
      return this.CurrTok.Outdent(this);
    }

    private int LastIndent() => this.indentStack.Count == 0 ? 0 : (int) this.indentStack.Peek();

    private void SkipLine()
    {
      while (this.PeekChar != '\n' && this.PeekChar != '\r' && this.PeekChar != char.MinValue)
      {
        int num1 = (int) this.NextChar();
      }
      int num2 = (int) this.NextChar();
    }
  }
}
