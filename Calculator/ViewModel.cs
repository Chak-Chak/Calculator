using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;


namespace Calculator
{

    class ViewModel : INotifyPropertyChanged
    {
        public long? leftValue { get; set; } = null;
        public long? rightValue { get; set; } = null;

        public string mark = null;
        public long? resultValue { get; set; } = null;

        public bool isFinished = false;


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
                if (isFinished)
                {
                    TextBlock = "0";
                    isFinished = false;
                }
                if (Regex.IsMatch(x, "\\d+"))
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
                    TextBlock += x;
                }
                if (x == "Back")
                {
                    if (TextBlock.Count() == 1)
                    {
                        TextBlock = "0";
                        return;
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
                isFinished = true;
                if (leftValue == null)
                {
                    leftValue = Int64.Parse(TextBlock);
                    mark = x == "=" ? null : x;
                    TextBlock = "0";
                    return;
                }
                if (mark == null)
                {
                    if (x == "=") return;
                    mark = x;
                    leftValue = Int64.Parse(TextBlock);
                    return;
                }
                switch (mark)
                {
                    case "+":
                    case "-":
                    case "*":
                    case "/":
                        {
                            if ((mark != null) && (leftValue != null) && (rightValue == null) && (TextBlock == "0"))
                            {
                                //MessageBox.Show("Изменение знака!");
                                TextExample = TextExample.Remove(TextExample.Length - 1);
                                TextExample += x;
                                mark = x;
                            }
                            else
                            {
                                if ((mark == null) && (rightValue == null))
                                {
                                    //MessageBox.Show("Вставка знака!");
                                    leftValue = Int64.Parse(TextBlock);
                                    TextExample = leftValue + x;
                                    TextBlock = "0";
                                    mark = x;
                                }
                                else
                                {
                                    //MessageBox.Show("Вычисление!");
                                    rightValue = Int64.Parse(TextBlock);
                                    calculate();
                                    TextExample = resultValue + x;
                                    leftValue = resultValue;
                                    rightValue = null;
                                    resultValue = null;
                                    TextBlock = "0";
                                }
                            }
                            break;
                        }
                        
                    default: break;
                }
                if (x == "=")
                {
                    if ((leftValue != null) && (mark != null))
                    {
                        rightValue = Int64.Parse(TextBlock);
                        calculate();
                        TextExample = resultValue.ToString();
                        leftValue = resultValue;
                        rightValue = null;
                        mark = null;
                        resultValue = null;
                        TextBlock = "0";
                    }
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
                isFinished = false;
            }, () => true);
        }

        /*private void Button_Click(object sender, RoutedEventArgs e)
        {
            string buttonText = (sender as Button).Content.ToString();
            switch (buttonText)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    {
                        if (textBlock.Text.Length < 19)
                        {
                            if (textBlock.Text == "0")
                            {
                                textBlock.Text = "";
                            }
                            textBlock.Text += buttonText;
                        }
                        break;
                    }
                case "00":
                    {
                        if (textBlock.Text.Length + 2 <= 19)
                        {
                            if (textBlock.Text == "0")
                            {
                                textBlock.Text = "0";
                            }
                            else textBlock.Text += buttonText;
                        }
                        break;
                    }
                case "+":
                case "-":
                case "*":
                case "/":
                    {
                        if ((mark != null) && (leftValue != null) && (rightValue == null) && (textBlock.Text == "0"))
                        {
                            //MessageBox.Show("Изменение знака!");
                            textExample.Text = textExample.Text.Remove(textExample.Text.Length - 1);
                            textExample.Text += buttonText;
                            mark = buttonText;
                        }
                        else
                        {
                            if ((mark == null) && (rightValue == null))
                            {
                                //MessageBox.Show("Вставка знака!");
                                leftValue = Int64.Parse(textBlock.Text);
                                textExample.Text = leftValue + buttonText;
                                textBlock.Text = "0";
                                mark = buttonText;
                            }
                            else
                            {
                                //MessageBox.Show("Вычисление!");
                                rightValue = Int64.Parse(textBlock.Text);
                                calculate();
                                textExample.Text = resultValue + buttonText;
                                leftValue = resultValue;
                                rightValue = null;
                                resultValue = null;
                                textBlock.Text = "0";
                            }
                        }

                        break;
                    }

                case "=":
                    {
                        if ((leftValue != null) && (mark != null))
                        {
                            rightValue = Int64.Parse(textBlock.Text);
                            calculate();
                            textExample.Text = resultValue.ToString();
                            leftValue = resultValue;
                            rightValue = null;
                            mark = null;
                            resultValue = null;
                            textBlock.Text = "0";
                        }
                        break;
                    }

                case "C":
                    {
                        textBlock.Text = "0";
                        textExample.Text = "";
                        leftValue = null;
                        rightValue = null;
                        mark = null;
                        resultValue = null;
                        break;
                    }
                case "CE":
                    {
                        textBlock.Text = "0";
                        break;
                    }
                case "Back":
                    {
                        if ((textBlock.Text.Length - 1 > 0) && (textBlock.Text != null))
                        {
                            textBlock.Text = textBlock.Text.Remove(textBlock.Text.Length - 1);
                        }
                        else
                        {
                            if ((textBlock.Text.Length - 1 == 0) || (textBlock.Text.Length - 1 < 0))
                            {
                                textBlock.Text = "0";
                            }
                        }
                        break;
                    }

            }
        }*/

        public void calculate()
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
        }
    }
}
