using FFEC.ExpressionModule.Expression;
using Avalonia.Controls;
namespace FFEC
{
    internal static class InputController
    {
        public static event ChangeHandler Update;
        public delegate void ChangeHandler();

        public static void Append(List<Composite> expression, Term number)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count == 0)
            {
                actualExpression.Add(number);
            }
            else
            {
                switch (actualExpression.Last())
                {
                    case Term lnumber:
                        {
                            if (lnumber.Record == "0")
                            {
                                actualExpression[actualExpression.Count - 1] = number;
                            }
                            else
                            {
                                actualExpression[actualExpression.Count - 1].Set(lnumber.Record + number.Record);
                            }

                            break;
                        }
                    case Operator: { actualExpression.Add(number); break; }
                }
            }
            Update.Invoke();
        }

        public static void Append(List<Composite> expression, Comma comma)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count != 0 && actualExpression.Last() is Term term && !term.HasDouble)
            {
                term.ToDouble();
                Update.Invoke();
            }
        }

        public static void Append(List<Composite> expression, Constanta constanta)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count == 0)
            {
                actualExpression.Add(constanta);
            }
            else
            {
                switch (actualExpression.Last())
                {
                    case Term: { actualExpression[actualExpression.Count - 1] = constanta; break; }
                    case Operator: { actualExpression.Add(constanta); break; }
                }
            }
            Update.Invoke();
        }

        private static void AppendConstantaLike(List<Composite> expression, Composite composite)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count == 0)
            {
                actualExpression.Add(composite);
            }
            else
            {
                switch (actualExpression.Last())
                {
                    case Term: { actualExpression[actualExpression.Count - 1] = composite; break; }
                    case Operator: { actualExpression.Add(composite); break; }
                }
            }
            Update.Invoke();
        }

        public static void Append(List<Composite> expression, Operator operation)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count != 0)
            {
                switch (actualExpression.Last())
                {
                    case Operator: { actualExpression[actualExpression.Count - 1] = operation; break; }
                    case Variable or Term or Function or Staples: { actualExpression.Add(operation); break; }
                }
                Update.Invoke();
            }
        }

        public static void Append(List<Composite> expression, IExpressionStoreable storable)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count == 0 || actualExpression.Last() is Operator) { actualExpression.Add((Composite)storable); Update.Invoke(); }
        }

        public static void Append(List<Composite> expression, Variable variable)
        {
            AppendConstantaLike(expression, variable);
        }

        public static void Append(List<Composite> expression, CustomFunction function)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count == 0 || actualExpression.Last() is Operator)
            {
                actualExpression.AddRange(function.GetExpression);
            }
            Update.Invoke();
            ValidateVariableInExpression(expression);
        }

        public static void ChangeSign(List<Composite> expression)
        {
            List<Composite> actualExpression = GetActualExpression(expression);

            if (actualExpression.Count != 0 && actualExpression.Last() is Term term) { term.ChangeSign(); Update.Invoke(); }
        }

        private static List<Composite> GetActualExpression(List<Composite> expression)
        {
            return expression.Count != 0 && expression.Last() is IExpressionStoreable expressionStoreable ?
                expressionStoreable.GetActualExpression(expression) :
                expression;
        }

        private static List<Composite> GetNotEmptyExpression(List<Composite> expression)
        {
            return expression.Last() is IExpressionStoreable expressionStoreable
                ? expressionStoreable.GetActualNotEmptyExpression(expression)
                : expression;
        }

        public static void CloseExpressionWrite(List<Composite> expression)
        {
            if (expression.Count == 0)
            {
                return;
            }

            if (expression.Last() is IExpressionStoreable compositeStoreable)
            {
                IExpressionStoreable actualComposite = compositeStoreable.GetActualComposite();
                List<Composite>? currentExpression = actualComposite.GetCurrentExpression();
                if (currentExpression is not null && currentExpression.Count != 0 && currentExpression.Last() is not Operator)
                {
                    actualComposite.CloseWrite();
                }
            }
        }

        public static void Equally(ref List<Composite> expression)
        {
            if ((expression.Count > 1 && expression[expression.Count - 2] is Operator) | (expression.Count == 1 && expression.Last() is IExpressionStoreable))
            {
                expression =
                [
                    Calculate.SolutionEquation(expression)
                ];
            }
            Update.Invoke();
        }

        public static void DeleteLast(List<Composite> expression)
        {
            if (expression.Count == 0)
            {
                return;
            }

            List<Composite> actualExpression = GetNotEmptyExpression(expression);

            switch (actualExpression.Last())
            {
                case Term number:
                    {
                        if (actualExpression[actualExpression.Count - 1].Record.Length == 1)
                        {
                            actualExpression.RemoveAt(actualExpression.Count - 1);
                        }
                        else
                        {
                            actualExpression[actualExpression.Count - 1].Set(number.Record.Remove(number.Record.Length - 1));
                        }

                        break;
                    }
                default:
                    {
                        actualExpression.RemoveAt(actualExpression.Count - 1);
                        break;
                    }
            }
            Update.Invoke();
        }

        public static void ClearAll(List<Composite> expression)
        {
            if (expression.Count != 0) { expression.Clear(); Update.Invoke(); }
        }
        public static void ClearOne(List<Composite> expression)
        {
            if (expression.Count == 0)
            {
                return;
            }

            List<Composite> actualExpression = GetNotEmptyExpression(expression);

            actualExpression.RemoveAt(actualExpression.Count - 1);

            Update.Invoke();
        }

        public static void UpdateVariableInExpression(List<Composite> expression)
        {
            if (expression.Count == 0)
            {
                return;
            }

            List<string> variableNames = Config.GetVariableNames();
            //List<string> missing

            foreach (Composite composite in expression)
            {
                if (composite is Variable variable && !variableNames.Contains(variable.Record))
                {
                    expression.Clear();
                    Update.Invoke();
                    break;
                }

            }
        }

        public static void ValidateVariableInExpression(List<Composite> expression)
        {
            if (expression.Count == 0)
            {
                return;
            }

            List<string> actualVariableNames = Config.GetVariableNames();

            List<string> expressionVariableNames = new List<string>();

            List<string> missingVariableNames = new List<string>();

            foreach (Composite composite in expression)
            {
                if (composite is Variable variable) expressionVariableNames.Add(variable.Record);
                if (composite is IExpressionStoreable storable) expressionVariableNames.AddRange(storable.ContainsVariable());
            }

            foreach (string variableName in expressionVariableNames.Distinct())
            {
                if (!actualVariableNames.Contains(variableName)) missingVariableNames.Add(variableName);
            }

            if (missingVariableNames.Count != 0)
            {
                // Messages.RaiseInformationMessage($"""
                //     Отсутствуют необходимые переменные:
                //     {string.Join(",\n", missingVariableNames)}.
                //     Пожалуйста, добавьте переменные и повторите попытку.
                //     """);
                expression.Clear();
                Update.Invoke();
            }
        }


        public static void UpdateDisplay()
        {
            Update.Invoke();
        }

        public static Func<List<Composite>, bool, string> GetRuleByData(JObject data)
        {
            return data["Name"].Value<string>() switch
            {
                "Base" => Output.ExpressionToRecord,
                "Manual" => Output.ExpressionToManual,
                _ => (List<Composite> expression, bool debug) => ""
            };
        }

        public static EventHandler GetActionByButtonData(JObject data)
        {
            if (!data.ContainsKey("Sector")) return (object sender, EventArgs e) => { };
            string sector = data["Sector"].Value<string>(), name = data["Name"].Value<string>();
            EventHandler handler = sector switch
            {
                "Numbers" => name switch
                {
                    "Zero" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("0")); }
                    ,
                    "One" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("1")); }
                    ,
                    "Two" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("2")); }
                    ,
                    "Three" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("3")); }
                    ,
                    "Four" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("4")); }
                    ,
                    "Five" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("5")); }
                    ,
                    "Six" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("6")); }
                    ,
                    "Seven" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("7")); }
                    ,
                    "Eight" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("8")); }
                    ,
                    "Nine" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Term("9")); }
                    ,
                    "Pi" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Constanta(Math.PI.ToString())); }
                    ,
                    "E" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Constanta(Math.E.ToString())); }
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Actions" => name switch
                {
                    "ToDouble" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Comma()); }
                    ,
                    "Backspace" => (object sender, EventArgs e) => { InputController.DeleteLast(Global.expression); }
                    ,
                    "Equally" => (object sender, EventArgs e) =>
                    {
                        try
                        {
                            InputController.Equally(ref Global.expression);
                        }
                        catch (DivideByZeroException)
                        {
                            Global.expression.Clear();
                            InputController.UpdateDisplay();
                        }
                    }
                    ,
                    "Clear" => (object sender, EventArgs e) => { InputController.ClearAll(Global.expression); }
                    ,
                    "ClearElement" => (object sender, EventArgs e) => { InputController.ClearOne(Global.expression); }
                    ,
                    "ChangeSign" => (object sender, EventArgs e) => { InputController.ChangeSign(Global.expression); }
                    ,
                    "Parenthesis" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Staples()); }
                    ,
                    "CloseFunctionWrite" => (object sender, EventArgs e) => { InputController.CloseExpressionWrite(Global.expression); },
                    "DeleteElement" => (object sender, EventArgs e) => {}
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Operators" => name switch
                {
                    "Append" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Operator(OutputView.GetViewByMark(OperatorMark.Add), OperatorMark.Add)); }
                    ,
                    "Subtract" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Operator(OutputView.GetViewByMark(OperatorMark.Subtract), OperatorMark.Subtract)); }
                    ,
                    "Multiply" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Operator(OutputView.GetViewByMark(OperatorMark.Multiply), OperatorMark.Multiply)); }
                    ,
                    "Division" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Operator(OutputView.GetViewByMark(OperatorMark.Division), OperatorMark.Division)); }
                    ,
                    "Modular" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Operator(OutputView.GetViewByMark(OperatorMark.Modular), OperatorMark.Modular)); }
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Function" => name switch
                {
                    "NaturalLogarithm" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.NaturalLogarithm), FunctionMark.NaturalLogarithm)); }
                    ,
                    "EPowerOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.EPowerOfX), FunctionMark.EPowerOfX)); }
                    ,
                    "DecimalLogarithm" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.DecimalLogarithm), FunctionMark.DecimalLogarithm)); }
                    ,
                    "LogarithmOfXBasedOnY" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new BinaryFunction(OutputView.GetViewByMark(FunctionMark.LogarithmOfXBasedOnY), FunctionMark.LogarithmOfXBasedOnY)); }
                    ,
                    "TenPowerOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.TenPowerOfX), FunctionMark.TenPowerOfX)); }
                    ,
                    "TwoPowerOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.TwoPowerOfX), FunctionMark.TwoPowerOfX)); }
                    ,
                    "XPowerOfY" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new BinaryFunction(OutputView.GetViewByMark(FunctionMark.XPowerOfY), FunctionMark.XPowerOfY)); }
                    ,
                    "YRootOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new BinaryFunction(OutputView.GetViewByMark(FunctionMark.YRootOfX), FunctionMark.YRootOfX)); }
                    ,
                    "XPowerOfTwo" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.XPowerOfTwo), FunctionMark.XPowerOfTwo)); }
                    ,
                    "XPowerOfThree" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.XPowerOfThree), FunctionMark.XPowerOfThree)); }
                    ,
                    "SquareRootOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.SquareRootOfX), FunctionMark.SquareRootOfX)); }
                    ,
                    "CubicRootOfX" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.CubicRootOfX), FunctionMark.CubicRootOfX)); }
                    ,
                    "XReverse" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.XReverse), FunctionMark.XReverse)); }
                    ,
                    "XAbsolute" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.XAbsolute), FunctionMark.XAbsolute)); }
                    ,
                    "Exponential" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.Exponential), FunctionMark.Exponential)); }
                    ,
                    "NFactorial" => (object sender, EventArgs e) => { InputController.Append(Global.expression, new SingularFunction(OutputView.GetViewByMark(FunctionMark.NFactorial), FunctionMark.NFactorial)); }
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Trigonometry" => name switch
                {
                    "Sine" => (object sender, EventArgs e) => { }
                    ,
                    "Cosine" => (object sender, EventArgs e) => { }
                    ,
                    "Tangent" => (object sender, EventArgs e) => { }
                    ,
                    "Cosecant" => (object sender, EventArgs e) => { }
                    ,
                    "Secant" => (object sender, EventArgs e) => { }
                    ,
                    "Cotangent" => (object sender, EventArgs e) => { }
                    ,
                    "Arcsine" => (object sender, EventArgs e) => { }
                    ,
                    "Arccosine" => (object sender, EventArgs e) => { }
                    ,
                    "Arctangent" => (object sender, EventArgs e) => { }
                    ,
                    "Arccosecant" => (object sender, EventArgs e) => { }
                    ,
                    "Arcsecant" => (object sender, EventArgs e) => { }
                    ,
                    "Arccotangent" => (object sender, EventArgs e) => { }
                    ,
                    "DegreesTypeChange" => (object sender, EventArgs e) => { }
                    ,
                    "ToDegreesMinutesSeconds" => (object sender, EventArgs e) => { }
                    ,
                    "ToDegrees" => (object sender, EventArgs e) => { }
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Memory" => name switch
                {
                    "MemoryClear" => (object sender, EventArgs e) => { }
                    ,
                    "MemoryRead" => (object sender, EventArgs e) => { }
                    ,
                    "MemorySave" => (object sender, EventArgs e) => { }
                    ,
                    "MemoryView" => (object sender, EventArgs e) => { }
                    ,
                    "ChangeView" => (object sender, EventArgs e) => { }
                    ,
                    "MemoryNumberAddNumber" => (object sender, EventArgs e) => { }
                    ,
                    "MemoryNumberSubtractNumber" => (object sender, EventArgs e) => { }
                    ,
                    _ => (object sender, EventArgs e) => { }
                },
                "Variables" => name switch
                {
                    _ => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Variable(name)); }
                },
                "Custom functions" => name switch
                {
                    _ => (object sender, EventArgs e) => { InputController.Append(Global.expression, new CustomFunction(name)); }
                    //_ => (object sender, EventArgs e) => { InputController.Append(Global.expression, new Variable(name)); }
                },
                _ => (object sender, EventArgs e) => { }
            };
            return handler;
        }
    }
}