// Decompiled with JetBrains decompiler
// Type: ExpressionCode.BinOpAst
// Assembly: ExpressionCode, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 342C2B39-3E06-4908-8333-0B3A4BE39DBA
// Assembly location: C:\Users\gravi\AppData\Local\Temp\Cezygiw\ba9bcd6ccb\lib\net452\ExpressionCode.dll

namespace ExpressionCode
{
  internal class BinOpAst : ExpressionAst
  {
    public ExpressionAst left;
    public TokenType op;
    public ExpressionAst right;

    public BinOpAst(ExpressionAst left, TokenType op, ExpressionAst right)
      : base((AstNode) left)
    {
      this.NodeType = AstNodeType.BinOpPre;
      this.left = left;
      this.op = op;
      this.right = right;
    }

    public override AstNode Optimize()
    {
      this.left = (ExpressionAst) this.left.Optimize();
      this.right = (ExpressionAst) this.right.Optimize();
      if (this.left is StringAst left4 && this.right is StringAst right4)
      {
        string strA = left4.value;
        string strB = right4.value;
        switch (this.op)
        {
          case TokenType.Plus:
            left4.Update(strA + strB, (AstNode) left4, (AstNode) right4);
            return (AstNode) left4;
          case TokenType.Eq:
            return (AstNode) new FloatAst(strA == strB ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          case TokenType.Neq:
            return (AstNode) new FloatAst(strA != strB ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          case TokenType.Lt:
            return (AstNode) new FloatAst(string.Compare(strA, strB) < 0 ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          case TokenType.Leq:
            return (AstNode) new FloatAst(string.Compare(strA, strB) <= 0 ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          case TokenType.Gt:
            return (AstNode) new FloatAst(string.Compare(strA, strB) > 0 ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          case TokenType.Geq:
            return (AstNode) new FloatAst(string.Compare(strA, strB) >= 0 ? 1.0 : 0.0, (AstNode) left4, (AstNode) right4);
          default:
            Errors.Raise("Invalid operator", (AstNode) this);
            return (AstNode) null;
        }
      }
      else if (this.left is IntAst left5 && this.right is IntAst right5)
      {
        int num1 = left5.value;
        int num2 = right5.value;
        switch (this.op)
        {
          case TokenType.Plus:
            return (AstNode) left5.Update(num1 + num2, (AstNode) left5, (AstNode) right5);
          case TokenType.Minus:
            return (AstNode) left5.Update(num1 - num2, (AstNode) left5, (AstNode) right5);
          case TokenType.Times:
            return (AstNode) left5.Update(num1 * num2, (AstNode) left5, (AstNode) right5);
          case TokenType.Divide:
            return (AstNode) left5.Update(num1 / num2, (AstNode) left5, (AstNode) right5);
          case TokenType.Mod:
            return (AstNode) left5.Update(num1 % num2, (AstNode) left5, (AstNode) right5);
          case TokenType.Eq:
            return (AstNode) left5.Update(num1 == num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.Neq:
            return (AstNode) left5.Update(num1 != num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.Lt:
            return (AstNode) left5.Update(num1 < num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.Leq:
            return (AstNode) left5.Update(num1 <= num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.Gt:
            return (AstNode) left5.Update(num1 > num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.Geq:
            return (AstNode) left5.Update(num1 >= num2 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          case TokenType.And:
            return (AstNode) left5.Update(num1 == 0 || num2 == 0 ? 0 : 1, (AstNode) left5, (AstNode) right5);
          case TokenType.Or:
            return (AstNode) left5.Update(num1 != 0 || num2 != 0 ? 1 : 0, (AstNode) left5, (AstNode) right5);
          default:
            Errors.Raise("Invalid operator", (AstNode) this);
            return (AstNode) null;
        }
      }
      else if (this.left is FloatAst left6 && this.right is FloatAst right6)
      {
        double num1 = left6.value;
        double num2 = right6.value;
        switch (this.op)
        {
          case TokenType.Plus:
            return (AstNode) left6.Update(num1 + num2, (AstNode) left6, (AstNode) right6);
          case TokenType.Minus:
            return (AstNode) left6.Update(num1 - num2, (AstNode) left6, (AstNode) right6);
          case TokenType.Times:
            return (AstNode) left6.Update(num1 * num2, (AstNode) left6, (AstNode) right6);
          case TokenType.Divide:
            return (AstNode) left6.Update(num1 / num2, (AstNode) left6, (AstNode) right6);
          case TokenType.Mod:
            return (AstNode) left6.Update((double) ((int) num1 % (int) num2), (AstNode) left6, (AstNode) right6);
          case TokenType.Eq:
            return (AstNode) left6.Update(num1 == num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.Neq:
            return (AstNode) left6.Update(num1 != num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.Lt:
            return (AstNode) left6.Update(num1 < num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.Leq:
            return (AstNode) left6.Update(num1 <= num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.Gt:
            return (AstNode) left6.Update(num1 > num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.Geq:
            return (AstNode) left6.Update(num1 >= num2 ? 1.0 : 0.0, (AstNode) left6, (AstNode) right6);
          case TokenType.And:
            return (AstNode) new IntAst(num1 == 0.0 || num2 == 0.0 ? 0 : 1, (AstNode) left6, (AstNode) right6);
          case TokenType.Or:
            return (AstNode) new IntAst(num1 != 0.0 || num2 != 0.0 ? 1 : 0, (AstNode) left6, (AstNode) right6);
          default:
            Errors.Raise("Invalid operator", (AstNode) this);
            return (AstNode) null;
        }
      }
      else if (this.left is FloatAst && this.right is IntAst || this.left is FloatAst && this.right is IntAst)
      {
        FloatAst floatAst = this.left is FloatAst ? (FloatAst) this.left : (FloatAst) this.right;
        IntAst intAst = this.left is IntAst ? (IntAst) this.left : (IntAst) this.right;
        double num1 = this.left is IntAst ? (double) ((IntAst) this.left).value : ((FloatAst) this.left).value;
        double num2 = this.right is IntAst ? (double) ((IntAst) this.right).value : ((FloatAst) this.right).value;
        switch (this.op)
        {
          case TokenType.Plus:
            return (AstNode) floatAst.Update(num1 + num2, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Minus:
            return (AstNode) floatAst.Update(num1 - num2, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Times:
            return (AstNode) floatAst.Update(num1 * num2, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Divide:
            return (AstNode) floatAst.Update(num1 / num2, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Mod:
            return (AstNode) floatAst.Update((double) ((int) num1 % (int) num2), (AstNode) this.left, (AstNode) this.right);
          case TokenType.Eq:
            return (AstNode) floatAst.Update(num1 == num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Neq:
            return (AstNode) floatAst.Update(num1 != num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Lt:
            return (AstNode) floatAst.Update(num1 < num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Leq:
            return (AstNode) floatAst.Update(num1 <= num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Gt:
            return (AstNode) floatAst.Update(num1 > num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Geq:
            return (AstNode) floatAst.Update(num1 >= num2 ? 1.0 : 0.0, (AstNode) this.left, (AstNode) this.right);
          case TokenType.And:
            return (AstNode) intAst.Update(num1 == 0.0 || num2 == 0.0 ? 0 : 1, (AstNode) this.left, (AstNode) this.right);
          case TokenType.Or:
            return (AstNode) intAst.Update(num1 != 0.0 || num2 != 0.0 ? 1 : 0, (AstNode) this.left, (AstNode) this.right);
          default:
            Errors.Raise("Invalid operator", (AstNode) this);
            return (AstNode) null;
        }
      }
      else
      {
        this.WithSpan((AstNode) this.left, (AstNode) this.right);
        return (AstNode) this;
      }
    }
  }
}
