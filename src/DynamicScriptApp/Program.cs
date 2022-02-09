using ExpressionCode;
using ExpressionCode.Debugger;
using ExpressionEvaluator;
using System;
using System.Diagnostics;
using System.Threading;

namespace DynamicScriptApp
{
    public class Program
    {
        static void Main(string[] args)
        {

            //we still have a problem because of system.reflection implementation is not complete in NanoFramework 
            //the problems are inside BuiltInDeclAst.cs
            var script = new ScriptEngine();
            script.DebugEvent += Script_DebugEvent;
            var console = new MyConsole();
            script.SetConsole(console);
            script.Run(@"
    const scale = 10000
    const arrinit = 2000
    const PI = 3.14159265359        
    var x = 5
    var y = 50
    cls
    print('Hello World!')   
    cls // will clear the previous prints
    print('Hello World!')   
    print   
    print 'TinyCLR'
    print ""with DUE""   
    print 'is the best!'
          
");

            var variableList = script.GetGlobalVariables();

            foreach (var v in variableList)
            {
                Debug.WriteLine($"{v.Name} = {v.Value} Is constant = {v.IsConstant}");
            }

            script.Run(@"
        func factorial(n)
            if n == 0 return 1 end
            return n * factorial(n - 1)
        end

        print(factorial(5))
");
            /*
            script.Run(@"
        func ToWords(i)
            if i < 10
                return i + ' units'
            elseif i < 100
                return i + ' tens'
            elseif i< 1000
                return i + ' hundreds'
            else
                        return i + ' thousands'
            end
        end

        print(ToWords(3))
        print(ToWords(55))
        print(ToWords(432))
        print(ToWords(2342))
",true);*/

            script.Run(@"
        var i = 1
        while i <= 10
            print(i)
            i = i + 1
        end
");
            
            script.Run(@"
        func pi_digits(digits)
            var carry = 0
            var arr = []
            var result = []

            // Adjust digits for groups of 4
            digits = (digits * 14) / 4

            // Initialize array
            var i  = 0
            while i < digits + 1
                append(arr, arrinit)
                i = i + 1
            end

            // Calculate digits
            i = digits
            while i > 0
                var sum = 0
                var j = i
                while j > 0
                    sum = sum * j + scale * arr[j]
                    arr[j] = sum % (j * 2 - 1)
                    sum = trunc(sum / (j * 2 - 1))
                    j = j - 1
                end
                append(result, carry + trunc(sum / scale))
                carry = sum % scale
                i = i - 14
            end
            return result
        end

        var result = pi_digits(3)
        print(result)
");
            TestExtension();
            Debug.WriteLine("--Test Evaluator Engine--");
            TestEvaluator();
            Thread.Sleep(Timeout.Infinite);
        }

        private static void Script_DebugEvent1(ScriptEngine sender, ExpressionCode.Debugger.DebugEventArgs e)
        {
            throw new NotImplementedException();
        }

        static void TestEvaluator()
        {
            EvaluatorEngine engine = new EvaluatorEngine();
            foreach (var line in Resources.GetString(Resources.StringResources.Logic).Split('\n'))
            {
                var hasil = engine.EvaluateLogic(line);
                Debug.WriteLine($"{line} => {hasil}");
            }
            foreach (var line in Resources.GetString(Resources.StringResources.Math).Split('\n'))
            {
                var hasil = engine.EvaluateFormula(line);
                Debug.WriteLine($"{line} => {hasil}");
            }
        }

        private static void Script_DebugEvent(ScriptEngine sender, ExpressionCode.Debugger.DebugEventArgs e)
        {
            Debug.WriteLine($"Executing: {e.SrcLine} : {e.SrcCol}..{e.SrcCol + e.SrcLength}");
            Debug.WriteLine("Locals");
            foreach (var v in e.Locals)
            {
                Debug.WriteLine($"{v.Name} = {v.Value}");
            }
            Debug.WriteLine("");
            e.Action = DebugAction.StepInto; // Step into next statement
        }

        static void TestExtension()
        {
            var script = new ScriptEngine(typeof(ExtensionLibrary));

            script.Run(@"ExtensionFuncSample(""Hello DUE!"")
var x = ExtensionAdder(44, 77)
ExtensionFuncSample(""x: "" + x)

func tambah(a,b){
    var res = ExtensionAdder(a, b);
    res = res + Additional;
    return res;
}
");
            var tambah = script.GetFunction("tambah");
            var hasil = script.Invoke(tambah, 10, 11);
            Debug.WriteLine($"result:{hasil}");
        }
    }

    class MyConsole : IConsole
    {
        private int row = 0;
        private const int charHeight = 9;

        public MyConsole()
        {

        }
        public void Cls()
        {
            this.row = 0;
        }
        public void Locate(int row, int col) { }// deprecated!
        public void Print(string s)
        {
            if (row >= (800 / charHeight))
            {
                Cls();
                row = 0;
            }
            //screen.DrawString("Hello world!", font, new SolidBrush(Color.Blue), 210, 255);
            Debug.WriteLine(s);
            row++;
        }


    }

    static class ExtensionLibrary
    {
        public static void ExtensionFuncSample(string arg)
        {
            Debug.WriteLine("Arg: " + arg);
        }

        public static int ExtensionAdder(int arg1, int arg2)
        {
            return arg1 + arg2;
        }

        public static readonly int Additional = 3;
    }
}

/*
var numbers = [1, 2, 3, 4]
var names = ["DUE", "is", "amazing"]
var mix = ["Hi", 55, 96.34]

const x = 5
const s = "Hello"

func greet(name)
  print "Hello " + name
end

greet("Gus")

if speed > 100 AND temp < 32
    print "Slow down!"
end

if (speed > 200 && temp < -40) {
    print("Goodbye!");
}

func add(x, y) 
  return x + y
end

var answer = add(32, 10)

[keyword]
print/cls	Special Output Console Statements
print	Prints the following expression to the console and moves to the next line
locate	Locates where the print will take place in rows, columns
if/elseif/else	Conditional execution of the code
while	Executes a block of code until the condition is false
break	Breaks out of the current while loop
continue	Continues the current while loop skipping the rest of the body
return	Returns a value from a function
end	Used in BASIC-style to indicate the end of if or while
var	Declares a variable
const	Declares a constant, a variable that can't be modified!
func	Declares a function block
AND/OR	Boolean operators

[general]
Delay(int ms)	Sleeps for specified duration in milliseconds

[array n string]
StrFmt(double d, string format)	Returns a formatted string representation of a variable ("N4" format for example)
Len(object o)	Returns the length of an array or string
Left(object o, int count)	Returns array or string based on count from left of the object
Right(object o, int count)	Returns an array or string based on count from right of the object
Mid(object o, int index, int count)	Returns an array or string based on index and count
IndexOf(object o, object value)	Returns the Index of an array or string based on object value passed
Val(string s)	Returns string as a Double
Append(ArrayList arr, object value)	Appends a value to an array
RemoveAt(ArrayList arr, int value)	Removes a value from an array at a specific array index
InsertAt(ArrayList arr, object value)	Inserts a value into an array at a specific array index

[math]
IsNan(double d)	Returns either 1 or 0 based on value passed
Abs(double d)	Returns the absolute value of a number
Sqrt(double d)	Returns the square root of a number
Sin(double rad)	Returns the sine of an number
Cos(double rad)	Returns the cosine of an number
Tan(double rad)	Returns the tangent of an number
Acos(double rad)	Returns the arc cosine value of a number
Asin(double rad)	Returns the arc sine value of a number
Atan(double rad)	Returns the arctangent value of a number
Atan2(double y, double x)	Returns the arctangent of x & y
Trunc(double d)	Returns the integer part of number by removing fractional digits
Round(double d)	Returns the value of a number rounded to the nearest integer
Rnd()	Returns the next pseudorandom double
 */ 