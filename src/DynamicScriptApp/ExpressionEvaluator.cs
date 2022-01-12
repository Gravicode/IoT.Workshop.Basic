using ExpressionCode;
using System;

namespace ExpressionEvaluator
{
    public class EvaluatorEngine
    {
        public ScriptEngine script { get; set; }
        public EvaluatorEngine()
        {
            if (script == null)
                script = new ScriptEngine();

        }

        public bool EvaluateLogic(string ExpressionStr)
        {
            script.ResetEnvironment();
            script.Run(@"
        func evalbool(){
            if (" + ExpressionStr + @"){
             return 1;
            }
            else{
             return 0;
            }
        }       
");
            var hasil = script.Invoke("evalbool");
            if (hasil is int res)
            {
                return res == 1;
            }
            throw new Exception("expression is not correct");
        }

        public double EvaluateFormula(string ExpressionStr)
        {
            script.ResetEnvironment();
            script.Run($@"
        func evalformula()
            var x = {ExpressionStr}
            return x
        end       
");
            var hasil = script.Invoke("evalformula");
            if (hasil is double res)
            {
                return res;
            }
            else if (hasil is int res2)
            {
                return res2;
            }
            throw new Exception("expression is not correct");
        }
    }
}
