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
using Calculator.Models.History;
using Calculator.Models.Memory;

namespace Calculator
{
    class ViewModel : INotifyPropertyChanged
    {
        public ViewModel()
        {
            Calc = new Calc();
            Memory = new Memory();
            History = new History();
            /*Memory = new MemoryJSON();
            History = new HistoryJSON();*/
            /*Memory = new MemoryDB();
            History = new HistoryDB();*/
        }
        public IMemory Memory { get; }
        public IHistory History { get; }
        public ICalculator Calc;

        public string leftValue { get; set; } = null;
        public long? rightValue { get; set; } = null;

        public string mark = "";
        public long? resultValue { get; set; } = null;
        string result = "";
        private int countCloseBrackets = 0;
        private int countOpenBrackets = 0;
        public bool canBePoint = false;

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

        private ICommand _clearHistory;
        public ICommand ClearHistory
        {
            get => _clearHistory ?? new RelayCommand(() =>
            {
                History.Clear();
            }, () => { if (History.HistoryColl.Count > 0) return true; else return false; });
        }

        private ICommand _action;
        public ICommand Action
        {
            get => _action ?? new RelayCommand<string>(x =>
            {
                switch (x)
                {
                    /*case ".":
                    {
                        if ()
                        break;
                    }*/
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

        private ICommand _clearMemory;
        public ICommand ClearMemory
        {
            get => _clearHistory ?? new RelayCommand(() =>
            {
                Memory.Clear();
            }, () => { if (Memory.MemoryColl.Count > 0) return true; else return false; });
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
