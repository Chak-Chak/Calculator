using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using Calculator.Interfaces;
using Calculator.Models;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Memory = new Memory();
            History = new History();
        }
        public IMemory Memory { get; }
        public IHistory History { get; }

        public string leftValue { get; set; } = null;
        public long? rightValue { get; set; } = null;

        public string mark = "";
        public long? resultValue { get; set; } = null;
        string result = "";
        private int countCloseBrackets = 0;
        private int countOpenBrackets = 0;


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

                    if (TextBlock[TextBlock.Length - 1] == ')') return;
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

                    if (TextBlock[TextBlock.Length - 1] == '(') countOpenBrackets--;
                    if (TextBlock[TextBlock.Length - 1] == ')') countCloseBrackets--;

                    TextBlock = TextBlock.Remove(TextBlock.Count() - 1);
                }
                if ((result = Calc.Parse(TextBlock)) != "error") { TextExample = result; }
                else { TextExample = null; }
            }, x => true);
        }

        private ICommand _addOpenBracket;
        public ICommand AddOpenBracket
        {
            get => _addOpenBracket ?? new RelayCommand<string>(x =>
            {
                if (x == "(")
                {
                    if (TextBlock == "0")
                    {
                        TextBlock = "(";
                        countOpenBrackets++;
                    }
                    else
                    {
                        if ("(+-*/".IndexOf(TextBlock[TextBlock.Length - 1]) != -1)
                        {
                            TextBlock += "(";
                            countOpenBrackets++;
                        }
                    }
                }
            }, x => true);
        }

        private ICommand _addCloseBracket;
        public ICommand AddCloseBracket
        {
            get => _addCloseBracket ?? new RelayCommand<string>(x =>
            {
                if (x == ")")
                {
                    if (("0123456789".IndexOf(TextBlock[TextBlock.Length - 1]) != -1) || TextBlock[TextBlock.Length - 1] == ')')
                    {
                        TextBlock += ")";
                        countCloseBrackets++;
                    }
                }
            }, x => { if (countOpenBrackets > countCloseBrackets) return true; else return false; });
        }

        private ICommand _action;
        public ICommand Action
        {
            get => _action ?? new RelayCommand<string>(x =>
            {
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
                            if ((mark == "") && (TextBlock.Length - 1 >= 0) && 
                                (("0123456789".IndexOf(TextBlock[TextBlock.Length - 1]) != -1) || (TextBlock[TextBlock.Length - 1] == ')')))
                            {
                                //MessageBox.Show("Вставка знака!");
                                TextBlock += x;
                                mark = x;
                            }
                        }
                        break;
                    }
                    case "CE":
                    {
                        TextBlock = "0";
                        break;
                    }
                    case "=":
                    {
                        result = Calc.Parse(TextBlock);
                        History.Add(new Expression(TextBlock, result));
                        break;
                    }
                    default: break;
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
            }, () => true);
        }

        private ICommand _historyDelete;

        public ICommand HistoryDelete
        {
            get => _historyDelete ?? new RelayCommand<TextBlock>(x =>
                {
                    History.Delete((int)x.Tag);
                }, x => true);
        }

        private ICommand _memoryActionPlus;
        public ICommand MemoryActionPlus
        {
            get => _memoryActionPlus ?? new RelayCommand<TextBlock>(x =>
            {
                double temp = Convert.ToDouble(x.Text);
                temp += Convert.ToDouble(TextExample);
                x.Text = temp.ToString();

            }, x => { if (TextExample == "") return false; else return true; });
        }

        private ICommand _memoryActionMinus;
        public ICommand MemoryActionMinus
        {
            get => _memoryActionMinus ?? new RelayCommand<TextBlock>(x =>
            {
                double temp = Convert.ToDouble(x.Text);
                temp -= Convert.ToDouble(TextExample);
                x.Text = temp.ToString();

            }, x => { if (TextExample == "") return false; else return true; });
        }

        private ICommand _memoryActionClear;
        public ICommand MemoryActionClear
        {
            get => _memoryActionClear ?? new RelayCommand<TextBlock>(x =>
            {
                Memory.Delete((int)x.Tag);
            }, x => true);
        }

        private ICommand _memoryActionSave;
        public ICommand MemoryActionSave
        {
            get => _memoryActionSave ?? new RelayCommand<string>(x =>
            {
                Memory.Add(Convert.ToDouble(TextExample));
            }, x => { if (TextExample == "") return false; else return true; });
        }

        private ICommand _memoryActionReset;
        public ICommand MemoryActionReset
        {
            get => _memoryActionReset ?? new RelayCommand<string>(x =>
            {
                History.Clear();
            }, x => true);
        }


        public static class Calc
        {
            public static string Parse(string expression)
            {
                expression = expression.Replace(" ", "");
                string result = "";
                char operation = 'n';
                string leftValue = "", rightValue = "", action = "";
                int indexStartBrackets = -1, indexEndBrackets = -1;
                int countStartBrackets = 0, countEndBrackets = 0;
                int indexStartMultiply;
                string tempExpression = "";

                while (expression.Length != 0)
                {
                    for (int j = 0; j < expression.Length; j++)
                    {
                        if (expression[j] == '(')
                        {
                            indexStartBrackets = j;
                            countStartBrackets++;
                        }
                        if (expression[j] == ')')
                        {
                            if (countEndBrackets == 0)
                            {
                                indexEndBrackets = j;
                                countEndBrackets++;
                            }
                            else
                            {
                                countEndBrackets++;
                            }
                        }
                    }
                    if (countStartBrackets == countEndBrackets)
                    {
                        if (countStartBrackets != 0)
                        {
                            if (indexEndBrackets > 1)
                            {
                                for (int j = 0; j < expression.Length; j++)
                                {
                                    if (expression[j] == ')')
                                    {
                                        indexEndBrackets = j;
                                        break;
                                    }
                                }
                            }
                            tempExpression = expression.Substring(indexStartBrackets + 1, indexEndBrackets - indexStartBrackets - 1);
                            tempExpression = Parse(tempExpression);
                            if (Convert.ToDouble(tempExpression) >= 0)
                            {
                                expression = expression.Remove(indexEndBrackets, 1);
                                expression = expression.Remove(indexStartBrackets, 1);
                                indexEndBrackets = indexEndBrackets - 2;
                                expression = expression.Remove(indexStartBrackets, indexEndBrackets - indexStartBrackets + 1);
                                expression = expression.Insert(indexStartBrackets, tempExpression);
                            }
                            else
                            {
                                indexEndBrackets = indexEndBrackets - 1;
                                expression = expression.Remove(indexStartBrackets + 1, indexEndBrackets - indexStartBrackets);
                                expression = expression.Insert(indexStartBrackets + 1, tempExpression);
                            }
                            countStartBrackets = 0;
                            countEndBrackets = 0;
                            continue;
                        }
                    }
                    else { return expression = "error"; }

                    for (int i = 0; i < expression.Length; i++)
                    {
                        if (((indexStartMultiply = expression.IndexOf('*')) > -1) || (indexStartMultiply = expression.IndexOf('/')) > -1)
                        {
                            if (indexStartMultiply - 1 >= 0)
                            {
                                if (expression[indexStartMultiply - 1] == ')')
                                {
                                    while ((indexStartMultiply - 1 >= 0) && (expression[indexStartMultiply - 1] != '('))
                                    {
                                        indexStartMultiply--;
                                    }
                                    indexStartMultiply--;
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
                                    leftValue = leftValue.Replace("(", "");
                                    leftValue = leftValue.Replace(")", "");
                                    rightValue = rightValue.Replace("(", "");
                                    rightValue = rightValue.Replace(")", "");
                                    result = calculation(leftValue, operation, rightValue);
                                    expression = expression.Insert(indexStartMultiply, result.ToString());
                                    leftValue = "";
                                    rightValue = "";
                                    operation = 'n';
                                }
                                else
                                {
                                    while (true)
                                    {
                                        if (indexStartMultiply - 1 >= 0)
                                        {
                                            if ((expression[indexStartMultiply - 1] != '+') ||
                                                (expression[indexStartMultiply - 1] != '-') ||
                                                (expression[indexStartMultiply - 1] != '/'))
                                            {
                                                indexStartMultiply--;
                                                continue;
                                            }
                                            else break;
                                        }
                                        else break;
                                    }
                                    leftValue = FindValue(expression, indexStartMultiply);
                                    expression = expression.Remove(indexStartMultiply, leftValue.Length);
                                    operation = expression[indexStartMultiply];
                                    expression = expression.Remove(indexStartMultiply, 1);
                                    if (indexStartMultiply != expression.Length)
                                    {
                                        rightValue = FindValue(expression, indexStartMultiply);
                                        expression = expression.Remove(indexStartMultiply, rightValue.Length);
                                    }
                                    else
                                    {
                                        rightValue = "0";
                                    }
                                    leftValue = leftValue.Replace("(", "");
                                    leftValue = leftValue.Replace(")", "");
                                    rightValue = rightValue.Replace("(", "");
                                    rightValue = rightValue.Replace(")", "");
                                    result = calculation(leftValue, operation, rightValue);
                                    expression = expression.Insert(indexStartMultiply, result.ToString());
                                    leftValue = "";
                                    rightValue = "";
                                    operation = 'n';
                                }
                            }
                            else
                            {
                                return expression = "error";
                            }
                        }
                        else
                        {
                            if ((expression[0] == '*') || (expression[0] == '/'))
                            {
                                return expression = "error";
                            }
                            else
                            {
                                if (leftValue == "")
                                {
                                    if (expression[0] == '(')
                                    {
                                        leftValue = FindValue(expression);
                                        expression = expression.Remove(0, leftValue.Length);
                                    }
                                    else
                                    {
                                        leftValue = FindValue(expression);
                                        expression = expression.Remove(0, leftValue.Length);
                                    }
                                }
                                int Length = expression.Length; // debug
                                if ((expression.Length != 0) && ((expression[0] == '+') || (expression[0] == '-')))
                                {
                                    operation = expression[0];
                                    expression = expression.Substring(1);
                                }
                                if ((expression != "") && (expression[0] == '('))
                                {
                                    rightValue = FindValue(expression);
                                    expression = expression.Remove(0, rightValue.Length);
                                }
                                else
                                {
                                    rightValue = FindValue(expression);
                                    expression = expression.Remove(0, rightValue.Length);
                                }
                                if (leftValue == "")
                                    leftValue = "0";
                                if (rightValue == "")
                                    return leftValue;
                                result = calculation(leftValue, operation, rightValue);
                                expression = expression.Insert(0, result);
                                leftValue = "";
                                rightValue = "";
                                operation = 'n';
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

            public static string calculation(string leftValue, char operation, string rightValue)
            {
                string result = "";
                try
                {
                    switch (operation)
                    {
                        case '+':
                        {
                            result = (Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue)).ToString();
                            break;
                        }
                        case '-':
                        {
                            result = (Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue)).ToString();
                            break;
                        }
                        case '*':
                        {
                            result = (Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue)).ToString();
                            break;
                        }
                        case '/':
                        {
                            result = (Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue)).ToString();
                            break;
                        }
                    }
                    return result.ToString();
                }
                catch
                {
                    return result = "error";
                }
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
