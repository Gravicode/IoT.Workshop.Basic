// Decompiled with JetBrains decompiler
// Type: ExpressionCode.Parser
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

using System;
using System.Collections;

namespace ExpressionCode
{
  internal class Parser
  {
    private static readonly ExpressionAst[] emptyExpressionArray = new ExpressionAst[0];
    private static readonly string[] emptyStringArray = new string[0];
    private readonly Pool arrayListPool = new Pool((Pool.FactoryFunc) (() => (object) new ArrayList()), (Pool.ResetFunc) (o => ((ArrayList) o).Clear()));
    private Scanner scanner;
    private Token prevTok;
    private Token currTok;
    private Token nextTok;

    public Parser(Scanner scanner)
    {
      this.scanner = scanner;
      this.nextTok = this.scanner.GetToken();
      this.NextToken();
    }

    private bool NextToken()
    {
      if (this.currTok != null && this.currTok.IsEof)
        return false;
      this.prevTok = this.currTok;
      this.currTok = this.nextTok;
      this.nextTok = this.scanner.GetToken();
      return !this.currTok.IsEof;
    }

    private Token Expect(TokenType tokenType, string errorMessage, params object[] args)
    {
      if (this.currTok.tokenType != tokenType)
        Errors.Raise(errorMessage, this.prevTok, args);
      Token currTok = this.currTok;
      this.NextToken();
      return currTok;
    }

    public AstNode Parse() => this.ParseScript();

    private AstNode ParseScript()
    {
      try
      {
        return (AstNode) new ScriptAst(this.ParseConstants(), this.ParseStatementList(TokenType.Eof));
      }
      finally
      {
        StringTable.Clear();
        this.arrayListPool.Clear();
        Token.ClearTokenPool();
        //GC.Collect();
        GC.WaitForPendingFinalizers();
      }
    }

    private StatementAst ParseStatement()
    {
      switch (this.currTok.tokenType)
      {
        case TokenType.Identifier:
          return this.ParseIdentifierStatement();
        case TokenType.VarDecl:
          return this.ParseVarOrConstDecl();
        case TokenType.ConstDecl:
          Errors.Raise("Constants must be declared before any other statements", this.currTok);
          break;
        case TokenType.Func:
          return this.ParseFuncDecl();
        case TokenType.Return:
          return this.ParseReturn();
        case TokenType.If:
          return this.ParseIf();
        case TokenType.While:
          return this.ParseWhile();
        case TokenType.Break:
          return this.ParseBreak();
        case TokenType.Continue:
          return this.ParseContinue();
        case TokenType.Locate:
          return this.ParseLocate();
        case TokenType.Print:
          return this.ParsePrint();
        case TokenType.Cls:
          return this.ParseCls();
        default:
          Errors.Raise("Unexpected '{0}'", this.currTok, this.currTok.value);
          break;
      }
      return (StatementAst) null;
    }

    private StatementBlockAst ParseConstants()
    {
      ArrayList arrayList = (ArrayList) this.arrayListPool.Acquire();
      try
      {
        while (this.currTok.Is(TokenType.ConstDecl))
          arrayList.Add((object) this.ParseVarOrConstDecl());
        return new StatementBlockAst((StatementAst[]) arrayList.ToArray(typeof (StatementAst)), (NopAst) null);
      }
      finally
      {
        this.arrayListPool.Release((object) arrayList);
      }
    }

    private StatementBlockAst ParseStatementList(
      TokenType terminator,
      params TokenType[] terminators)
    {
      ArrayList arrayList = (ArrayList) this.arrayListPool.Acquire();
      try
      {
        while (!this.currTok.IsEof && this.currTok.IsNot(TokenType.Error) && (this.currTok.IsNot(terminator) && this.currTok.IsNotOneOf(terminators)))
          arrayList.Add((object) this.ParseStatement());
        if (this.currTok.IsEof)
          arrayList.Add((object) new EndOfSourceAst(this.currTok));
        return new StatementBlockAst((StatementAst[]) arrayList.ToArray(typeof (StatementAst)), new NopAst(this.currTok));
      }
      finally
      {
        this.arrayListPool.Release((object) arrayList);
      }
    }

    private StatementAst ParseVarOrConstDecl()
    {
      bool flag = this.currTok.Is(TokenType.ConstDecl);
      this.NextToken();
      Token token = this.Expect(TokenType.Identifier, "Expected {0}", (object) "variable name").Clone();
      try
      {
        this.Expect(TokenType.Assign, "Expected {0}", (object) "'='");
        ExpressionAst expression = this.ParseExpression();
        while (this.currTok.Is(TokenType.SemiColon))
          this.NextToken();
        return flag ? (StatementAst) new ConstDeclAst(token, expression) : (StatementAst) new VarDeclAst(token, expression);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParseFuncDecl()
    {
      this.NextToken();
      Token token = this.Expect(TokenType.Identifier, "Expected {0}", (object) "Function Name").Clone();
      try
      {
        this.Expect(TokenType.LParen, "Expected {0}", (object) "'('");
        string[] identifierList = this.ParseIdentifierList(TokenType.RParen);
        this.Expect(TokenType.RParen, "Expected {0}", (object) "')'");
        StatementBlockAst codeBlock = this.ParseCodeBlock();
        return (StatementAst) new FuncDeclAst(token, identifierList, codeBlock);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementBlockAst ParseCodeBlock()
    {
      TokenType tokenType = TokenType.End;
      string str = "END";
      if (this.currTok.Is(TokenType.Colon))
      {
        this.NextToken();
        this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code block");
        tokenType = TokenType.Outdent;
        str = "Outdent";
      }
      else if (this.currTok.Is(TokenType.LBrace))
      {
        this.NextToken();
        tokenType = TokenType.RBrace;
        str = "}";
      }
      StatementBlockAst statementList = this.ParseStatementList(tokenType);
      this.Expect(tokenType, "Expected {0}", (object) str);
      return statementList;
    }

    private StatementAst ParseReturn()
    {
      ReturnAst returnAst = new ReturnAst(this.currTok);
      int num = (int) this.nextTok.line == (int) this.currTok.line ? 1 : 0;
      this.NextToken();
      if (num != 0)
        returnAst.SetReturnExpression(this.ParseExpression());
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      return (StatementAst) returnAst;
    }

    private StatementAst ParseBreak()
    {
      BreakAst breakAst = new BreakAst(this.currTok);
      this.NextToken();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      return (StatementAst) breakAst;
    }

    private StatementAst ParseContinue()
    {
      ContinueAst continueAst = new ContinueAst(this.currTok);
      this.NextToken();
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      return (StatementAst) continueAst;
    }

    private StatementAst ParseIdentifierStatement()
    {
      Token token = this.Expect(TokenType.Identifier, "Expected {0}", (object) "identifier").Clone();
      try
      {
        if (this.currTok.Is(TokenType.Assign))
        {
          this.NextToken();
          ExpressionAst expression = this.ParseExpression();
          while (this.currTok.Is(TokenType.SemiColon))
            this.NextToken();
          return (StatementAst) new AssignmentAst(token, expression);
        }
        ExpressionAst expressionAst = (ExpressionAst) new VariableAccessAst(token);
        if (this.currTok.Is(TokenType.LBracket))
          expressionAst = (ExpressionAst) this.ParseIndexAccessor(expressionAst);
        if (this.currTok.Is(TokenType.Assign))
        {
          this.NextToken();
          ExpressionAst expression = this.ParseExpression();
          while (this.currTok.Is(TokenType.SemiColon))
            this.NextToken();
          return (StatementAst) new AssignmentAst(token, (IndexAccessorAst) expressionAst, expression);
        }
        if (this.currTok.Is(TokenType.LParen))
          return this.ParseFunctionCall(expressionAst).AsStatement();
        Errors.Raise("Unexpected '{0}'", token, (object) (string) token.value);
        return (StatementAst) null;
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParseIf()
    {
      Token token = this.currTok.Clone();
      try
      {
        this.NextToken();
        ExpressionAst expression1 = this.ParseExpression();
        bool flag1 = this.currTok.Is(TokenType.LBrace);
        bool flag2 = this.currTok.Is(TokenType.Colon);
        StatementBlockAst statementList1;
        if (flag1)
        {
          this.NextToken();
          statementList1 = this.ParseStatementList(TokenType.RBrace);
          this.Expect(TokenType.RBrace, "Expected {0}", (object) "'}'");
        }
        else if (flag2)
        {
          this.NextToken();
          this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code");
          statementList1 = this.ParseStatementList(TokenType.Outdent);
          this.Expect(TokenType.Outdent, "Expected {0}", (object) "Outdent");
        }
        else
          statementList1 = this.ParseStatementList(TokenType.ElseIf, TokenType.Else, TokenType.End);
        IfAst[] elseIfList = (IfAst[]) null;
        if (this.currTok.tokenType == TokenType.ElseIf)
        {
          ArrayList arrayList = (ArrayList) this.arrayListPool.Acquire();
          try
          {
            while (this.currTok.tokenType == TokenType.ElseIf)
            {
              Token currTok = this.currTok;
              this.NextToken();
              ExpressionAst expression2 = this.ParseExpression();
              StatementBlockAst statementList2;
              if (flag1)
              {
                this.Expect(TokenType.LBrace, "Expected {0}", (object) "'{'");
                statementList2 = this.ParseStatementList(TokenType.RBrace);
                this.Expect(TokenType.RBrace, "Expected {0}", (object) "'}'");
              }
              else if (flag2)
              {
                this.Expect(TokenType.Colon, "Expected {0}", (object) "':'");
                this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code");
                statementList2 = this.ParseStatementList(TokenType.Outdent);
                this.Expect(TokenType.RBrace, "Expected {0}", (object) "Outdent");
              }
              else
                statementList2 = this.ParseStatementList(TokenType.ElseIf, TokenType.Else, TokenType.End);
              arrayList.Add((object) new IfAst(currTok, expression2, statementList2));
            }
            elseIfList = (IfAst[]) arrayList.ToArray(typeof (IfAst));
          }
          finally
          {
            this.arrayListPool.Release((object) arrayList);
          }
        }
        StatementBlockAst elseBody = (StatementBlockAst) null;
        if (this.currTok.tokenType == TokenType.Else)
        {
          this.NextToken();
          if (flag1)
          {
            this.Expect(TokenType.LBrace, "Expected {0}", (object) "'{'");
            elseBody = this.ParseStatementList(TokenType.RBrace);
            this.Expect(TokenType.RBrace, "Expected {0}", (object) "'}'");
          }
          else if (flag2)
          {
            this.Expect(TokenType.Colon, "Expected {0}", (object) "':'");
            this.Expect(TokenType.Indent, "Expected {0}", (object) "Indented code");
            elseBody = this.ParseStatementList(TokenType.Outdent);
            this.Expect(TokenType.Outdent, "Expected {0}", (object) "Outdent");
          }
          else
            elseBody = this.ParseStatementList(TokenType.End);
        }
        if (!flag1 && !flag2)
          this.Expect(TokenType.End, "Expected {0}", (object) "END");
        if (elseIfList != null && elseIfList.Length != 0)
          return (StatementAst) new IfElseIfAst(token, expression1, statementList1, elseIfList, elseBody);
        return elseBody != null && elseBody.statements.Length != 0 ? (StatementAst) new IfElseAst(token, expression1, statementList1, elseBody) : (StatementAst) new IfAst(token, expression1, statementList1);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParseWhile()
    {
      Token token = this.currTok.Clone();
      try
      {
        this.NextToken();
        ExpressionAst expression = this.ParseExpression();
        StatementBlockAst codeBlock = this.ParseCodeBlock();
        return (StatementAst) new WhileAst(token, expression, codeBlock);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParseLocate()
    {
      Token token = this.currTok.Clone();
      try
      {
        this.NextToken();
        ExpressionAst expression1 = this.ParseExpression();
        this.Expect(TokenType.Comma, "Expected {0}", (object) ",");
        ExpressionAst expression2 = this.ParseExpression();
        while (this.currTok.Is(TokenType.SemiColon))
          this.NextToken();
        return (StatementAst) new LocateAst(token, expression1, expression2);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParsePrint()
    {
      Token token = this.currTok.Clone();
      try
      {
        this.NextToken();
        ExpressionAst expressionAst = (ExpressionAst) null;
        if ((int) this.currTok.line == (int) token.line)
          expressionAst = this.ParseExpression();
        while (this.currTok.Is(TokenType.SemiColon))
          this.NextToken();
        return (StatementAst) new PrintAst(token, expressionAst);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private StatementAst ParseCls()
    {
      ClsAst clsAst = new ClsAst(this.currTok);
      this.NextToken();
      return (StatementAst) clsAst;
    }

    private ExpressionAst ParseExpression() => (ExpressionAst) this.ParseOrExpression().Optimize();

    private ExpressionAst ParseOrExpression()
    {
      ExpressionAst left = (ExpressionAst) this.ParseAndExpression().Optimize();
      while (this.currTok.Is(TokenType.Or))
      {
        TokenType tokenType = this.currTok.tokenType;
        this.NextToken();
        left = (ExpressionAst) new BinOpAst(left, tokenType, this.ParseAndExpression()).Optimize();
      }
      return left;
    }

    private ExpressionAst ParseAndExpression()
    {
      ExpressionAst left = (ExpressionAst) this.ParseRelExpression().Optimize();
      while (this.currTok.Is(TokenType.And))
      {
        TokenType tokenType = this.currTok.tokenType;
        this.NextToken();
        left = (ExpressionAst) new BinOpAst(left, tokenType, this.ParseRelExpression()).Optimize();
      }
      return left;
    }

    private ExpressionAst ParseRelExpression()
    {
      ExpressionAst left = (ExpressionAst) this.ParseAddExpression().Optimize();
      while (this.currTok.Is(TokenType.Lt) || this.currTok.Is(TokenType.Leq) || (this.currTok.Is(TokenType.Gt) || this.currTok.Is(TokenType.Geq)) || (this.currTok.Is(TokenType.Eq) || this.currTok.Is(TokenType.Neq)))
      {
        TokenType tokenType = this.currTok.tokenType;
        this.NextToken();
        left = (ExpressionAst) new BinOpAst(left, tokenType, this.ParseAddExpression());
      }
      return left;
    }

    private ExpressionAst ParseAddExpression()
    {
      ExpressionAst left = (ExpressionAst) this.ParseMultExpression().Optimize();
      while (this.currTok.Is(TokenType.Plus) || this.currTok.Is(TokenType.Minus))
      {
        TokenType tokenType = this.currTok.tokenType;
        this.NextToken();
        left = (ExpressionAst) new BinOpAst(left, tokenType, this.ParseMultExpression()).Optimize();
      }
      return left;
    }

    private ExpressionAst ParseMultExpression()
    {
      ExpressionAst left = (ExpressionAst) this.ParseFactor().Optimize();
      while (this.currTok.Is(TokenType.Times) || this.currTok.Is(TokenType.Divide) || this.currTok.Is(TokenType.Mod))
      {
        TokenType tokenType = this.currTok.tokenType;
        this.NextToken();
        left = (ExpressionAst) new BinOpAst(left, tokenType, this.ParseFactor()).Optimize();
      }
      return left;
    }

    private ExpressionAst ParseFactor()
    {
      Token token = this.currTok.Clone();
      try
      {
        ExpressionAst expressionAst = (ExpressionAst) null;
        bool flag = false;
        while (this.currTok.Is(TokenType.Minus) || this.currTok.Is(TokenType.Plus))
        {
          if (this.currTok.Is(TokenType.Minus))
            flag = !flag;
          this.NextToken();
        }
        switch (this.currTok.tokenType)
        {
          case TokenType.Int:
            expressionAst = (ExpressionAst) new IntAst(this.currTok);
            this.NextToken();
            break;
          case TokenType.Float:
            expressionAst = (ExpressionAst) new FloatAst(this.currTok);
            this.NextToken();
            break;
          case TokenType.String:
            expressionAst = (ExpressionAst) new StringAst(this.currTok);
            this.NextToken();
            if (this.currTok.tokenType == TokenType.LBracket)
            {
              expressionAst = (ExpressionAst) this.ParseIndexAccessor(expressionAst);
              break;
            }
            break;
          case TokenType.Identifier:
            expressionAst = this.ParseIdentifierExpression();
            break;
          case TokenType.LParen:
            this.NextToken();
            expressionAst = this.ParseOrExpression();
            this.Expect(TokenType.RParen, "Expected {0}", (object) "')'");
            break;
          case TokenType.LBracket:
            expressionAst = (ExpressionAst) this.ParseArrayLiteral();
            if (this.currTok.tokenType == TokenType.LBracket)
            {
              expressionAst = (ExpressionAst) this.ParseIndexAccessor(expressionAst);
              break;
            }
            break;
          default:
            Errors.Raise("Syntax error", this.currTok);
            break;
        }
        if (flag)
          expressionAst = (ExpressionAst) new NegateAst(token, expressionAst);
        return expressionAst;
      }
      finally
      {
        Token.Release(token);
      }
    }

    private ExpressionAst ParseIdentifierExpression()
    {
      Token token = this.currTok.Clone();
      ExpressionAst expressionAst;
      try
      {
        string str = (string) this.currTok.value;
        expressionAst = (ExpressionAst) new VariableAccessAst(token);
      }
      finally
      {
        Token.Release(token);
      }
      this.NextToken();
      if (this.currTok.tokenType == TokenType.LBracket)
        expressionAst = (ExpressionAst) this.ParseIndexAccessor(expressionAst);
      if (this.currTok.Is(TokenType.LParen))
        expressionAst = this.ParseFunctionCall(expressionAst);
      return expressionAst;
    }

    private ArrayLiteralAst ParseArrayLiteral()
    {
      Token token = this.currTok.Clone();
      try
      {
        this.NextToken();
        ExpressionAst[] expressionList = this.ParseExpressionList(TokenType.RBracket);
        this.Expect(TokenType.RBracket, "Expected {0}", (object) ']');
        return new ArrayLiteralAst(token, expressionList);
      }
      finally
      {
        Token.Release(token);
      }
    }

    private IndexAccessorAst ParseIndexAccessor(ExpressionAst expression)
    {
      while (this.currTok.tokenType == TokenType.LBracket)
      {
        this.NextToken();
        ExpressionAst expression1 = this.ParseExpression();
        this.Expect(TokenType.RBracket, "Expected {0}", (object) ']');
        expression = (ExpressionAst) new IndexAccessorAst(this.currTok, expression, expression1);
      }
      return (IndexAccessorAst) expression;
    }

    private ExpressionAst ParseFunctionCall(ExpressionAst functionExpr)
    {
      this.NextToken();
      ExpressionAst[] expressionList = this.ParseExpressionList(TokenType.RParen);
      this.Expect(TokenType.RParen, "Expected {0}", (object) "')'");
      while (this.currTok.Is(TokenType.SemiColon))
        this.NextToken();
      return (ExpressionAst) new FuncCallAst(functionExpr, expressionList);
    }

    private ExpressionAst[] ParseExpressionList(TokenType terminator)
    {
      if (this.currTok.IsEof || !this.currTok.IsNot(TokenType.Error) || !this.currTok.IsNot(terminator))
        return Parser.emptyExpressionArray;
      ArrayList arrayList = (ArrayList) this.arrayListPool.Acquire();
      try
      {
        arrayList.Add((object) this.ParseExpression());
        while (this.currTok.Is(TokenType.Comma))
        {
          this.NextToken();
          arrayList.Add((object) this.ParseExpression());
        }
        return (ExpressionAst[]) arrayList.ToArray(typeof (ExpressionAst));
      }
      finally
      {
        this.arrayListPool.Release((object) arrayList);
      }
    }

    private string[] ParseIdentifierList(TokenType terminator)
    {
      if (this.currTok.IsEof || !this.currTok.IsNot(TokenType.Error) || !this.currTok.IsNot(terminator))
        return Parser.emptyStringArray;
      ArrayList arrayList = (ArrayList) this.arrayListPool.Acquire();
      try
      {
        Token token1 = this.Expect(TokenType.Identifier, "Expected {0}", (object) "identifier");
        arrayList.Add((object) StringTable.Intern((string) token1.value));
        while (this.currTok.Is(TokenType.Comma))
        {
          this.NextToken();
          Token token2 = this.Expect(TokenType.Identifier, "Expected {0}", (object) "identifier");
          arrayList.Add((object) StringTable.Intern((string) token2.value));
        }
        return (string[]) arrayList.ToArray(typeof (string));
      }
      finally
      {
        this.arrayListPool.Release((object) arrayList);
      }
    }
  }
}
