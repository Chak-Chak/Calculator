using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Calculator
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            _expression = new ObservableCollection<Expression>();
        }

        public string leftValue { get; set; } = null;
        public long? rightValue { get; set; } = null;

        public string mark = "";
        public long? resultValue { get; set; } = null;

        public bool isFinished = false;
        public bool isSetNumber = true;


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string textBlock = "0";

        public string TextBlock
        {
            get => textBlock;
            set
            {
                textBlock = value;
                OnPropertyChanged(nameof(TextBlock));
            }
        }

        private string textExample = "0";
        public string TextExample
        {
            get => textExample;
            set
            {
                textExample = value;
                OnPropertyChanged(nameof(TextExample));
            }
        }

        private ICommand _addNumber;
        public ICommand AddNumber
        {
            get => _addNumber ?? new RelayCommand<string>(x =>
            {
                /*if (isFinished)
                {
                    //TextBlock = "0";
                    isFinished = false;
                }*/
                if ("0123456789".IndexOf(x) != -1)
                {
                    if (TextBlock == "0" || TextBlock == null)
                    {
                        if (x == "00")
                        {
                            TextBlock = "0";
                            return;
                        }
                        else
                        {
                            TextBlock = x;
                            return;
                        }
                    }
                    isSetNumber = true;
                    TextBlock += x;
                    TextExample = mark;
                    mark = "";
                }
                if (x == "Back")
                {
                    if (TextBlock.Count() == 1)
                    {
                        TextBlock = "0";
                        return;
                    }
                    if ("+-*/".IndexOf(TextBlock[TextBlock.Length - 1]) != -1)
                    {
                        TextExample = mark;
                        mark = "";
                    }

                    TextBlock = TextBlock.Remove(TextBlock.Count() - 1);
                }
            }, x => true);
        }

        //leftValue = Int64.Parse(textBlock.Text);
        //textExample.Text = leftValue + buttonText;

        private ICommand _action;
        public ICommand Action
        {
            get => _action ?? new RelayCommand<string>(x =>
            {
                //isFinished = true;
                /*if (leftValue == "")
                {
                    //leftValue = Int64.Parse(TextBlock);
                    mark = x == "=" ? null : x;
                    //TextBlock = "0";
                    return;
                }*/
                /*if (mark == null)
                {
                    if (x == "=") return;
                    mark = x;
                    //leftValue = Int64.Parse(TextBlock);
                    return;
                }*/
                switch (x)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        {
                            if ((TextBlock.Length - 1 >= 0) && ("+-*/".IndexOf(TextBlock[TextBlock.Length - 1]) != -1))
                            {
                                //MessageBox.Show("Изменение знака!");
                                TextBlock = TextBlock.Remove(TextBlock.Length - 1);
                                TextBlock += x;
                                mark = x;
                            }
                            else
                            {
                                if ((mark == "") && (TextBlock.Length - 1 >= 0) && ("0123456789".IndexOf(TextBlock[TextBlock.Length - 1]) != -1))
                                {
                                    //MessageBox.Show("Вставка знака!");
                                    //leftValue = Int64.Parse(TextBlock);
                                    TextBlock += x;
                                    //TextBlock = "0";
                                    mark = x;
                                }
                                /*else
                                {
                                    //MessageBox.Show("Вычисление!");
                                    TextBlock += x;
                                    rightValue = Int64.Parse(TextBlock);
                                    calculate();
                                    TextExample = resultValue + x;
                                    leftValue = resultValue;
                                    rightValue = null;
                                    resultValue = null;
                                    TextBlock = "0";
                                }*/
                            }
                            break;
                        }

                    default: break;
                }
                if (x == "=")
                {
                    string result = Calc.Parse(TextBlock);
                    Expressions.Add(new Expression(TextBlock, result));
                    /*if ((leftValue != null) && (mark != null))
                    {
                        rightValue = Int64.Parse(TextBlock);
                        //calculate();
                        TextExample = resultValue.ToString();
                        //leftValue = resultValue;
                        rightValue = null;
                        mark = null;
                        resultValue = null;
                        TextBlock = "0";
                    }*/
                }
            }, x => true);
        }

        private ICommand _clear;
        public ICommand Clear
        {
            get => _clear ?? new RelayCommand(() =>
            {
                TextBlock = "0";
                TextExample = "0";
                leftValue = null;
                mark = null;
                //isFinished = false;
            }, () => true);
        }

        private ObservableCollection<Expression> _expression;
        public ObservableCollection<Expression> Expressions
        {
            get => _expression;
        }

        /*public void calculate()
        {
            switch (mark)
            {
                case "+":
                    {
                        resultValue = leftValue + rightValue;
                        break;
                    }
                case "-":
                    {
                        resultValue = leftValue - rightValue;
                        break;
                    }
                case "*":
                    {
                        resultValue = leftValue - rightValue;
                        break;
                    }
                case "/":
                    {
                        if (rightValue != 0)
                            resultValue = leftValue / rightValue;
                        break;
                    }
            }
        }*/

        public static class Calc
        {
            public static string Parse(string expression)
            {
                expression = expression.Replace(" ", "");
                double result = 0;
                char operation = 'n';
                string leftValue = "", rightValue = "", action = "";
                int indexStartBrackets = -1, indexEndBrackets = -1;
                int indexStartMultiply;
                string tempExpression = "";

                while (expression.Length != 0)
                {
                    if ((indexStartBrackets = expression.IndexOf('(')) > -1)
                    {
                        if ((indexEndBrackets = expression.IndexOf(')')) > -1)
                        {
                            tempExpression = expression.Substring(indexStartBrackets + 1, indexEndBrackets - indexStartBrackets - 1);
                            tempExpression = Parse(tempExpression);
                            expression = expression.Remove(indexEndBrackets, 1);
                            expression = expression.Remove(indexStartBrackets, 1);
                            indexEndBrackets = indexEndBrackets - 2;
                            expression = expression.Remove(indexStartBrackets, indexEndBrackets - indexStartBrackets + 1);
                            expression = expression.Insert(indexStartBrackets, tempExpression);
                        }
                        else
                        {
                            return expression = "error";
                        }
                    }
                    else
                    {
                        for (int i = 0; i < expression.Length; i++)
                        {
                            if (((indexStartMultiply = expression.IndexOf('*')) > -1) || (indexStartMultiply = expression.IndexOf('/')) > -1)
                            {
                                while ((indexStartMultiply - 1 >= 0) && (expression[indexStartMultiply - 1] != '+') && (expression[indexStartMultiply - 1] != '-'))
                                {
                                    indexStartMultiply--;
                                }
                                leftValue = FindValue(expression, indexStartMultiply);
                                expression = expression.Remove(indexStartMultiply, leftValue.Length);
                                operation = expression[indexStartMultiply];
                                expression = expression.Remove(indexStartMultiply, 1);
                                if (indexStartMultiply != expression.Length - 1)
                                {
                                    rightValue = FindValue(expression, indexStartMultiply);
                                    expression = expression.Remove(indexStartMultiply, rightValue.Length);
                                }
                                else
                                {
                                    rightValue = "0";
                                }
                                result = calculation(leftValue, operation, rightValue);
                                expression = expression.Insert(indexStartMultiply, result.ToString());
                                leftValue = "";
                                rightValue = "";
                                operation = 'n';
                            }
                            else
                            {
                                if ((expression[0] == '*') || (expression[0] == '/'))
                                {
                                    return expression = "error";
                                }
                                else
                                {
                                    if (leftValue.Length == 0)
                                    {
                                        leftValue = FindValue(expression);
                                        expression = expression.Substring(leftValue.Length);
                                    }
                                    int Length = expression.Length; // debug
                                    if ((expression.Length != 0) && ((expression[0] == '+') || (expression[0] == '-')))
                                    {
                                        operation = expression[0];
                                        expression = expression.Substring(1);
                                    }
                                    else
                                    {
                                        expression = leftValue;
                                        return expression;
                                    }
                                    rightValue = FindValue(expression);
                                    expression = expression.Substring(rightValue.Length);
                                    result = calculation(leftValue, operation, rightValue);
                                    leftValue = result.ToString();
                                    operation = 'n';
                                }
                            }
                        }
                    }
                }
                expression += result;
                return expression;
            }

            public static string FindValue(string expression, int indexStart = 0)
            {
                string value = "";
                for (int i = indexStart; i < expression.Length; i++)
                {
                    if ((expression[i] == '-') && (value == ""))
                    {
                        value += "-";
                    }
                    else
                    {
                        if ((expression[i] == '+') || (expression[i] == '-') || (expression[i] == '*') || (expression[i] == '/'))
                        {
                            return value;
                        }
                        else
                        {
                            value += expression[i];
                        }
                    }
                }
                return value;
            }

            public static string FindRightValue(string expression, int indexStart = 0)
            {
                string value = "";
                for (int i = indexStart; i < expression.Length; i++)
                {
                    if ((expression[i] == '+') || (expression[i] == '-') || (expression[i] == '*') || (expression[i] == '/'))
                    {
                        return value;
                    }
                    else
                    {
                        value += expression[i];
                    }
                }
                return value;
            }

            public static double calculation(string leftValue, char operation, string rightValue)
            {
                double result = 0;
                switch (operation)
                {
                    case '+':
                        {
                            result = Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue);
                            break;
                        }
                    case '-':
                        {
                            result = Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue);
                            break;
                        }
                    case '*':
                        {
                            result = Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue);
                            break;
                        }
                    case '/':
                        {
                            result = Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue);
                            break;
                        }
                }
                return result;
            }

        }
    }

    public class Expression
    {
        public Expression(string exp, string answer)
        {
            Exp = exp;
            Value = answer;
        }
        public string Exp { get; }
        public string Value { get; }
    }
}
