using FluentAssertions;
using NUnit.Framework;
using System;
using System.ComponentModel.Design;

namespace Calc
{
    [TestFixture]
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var calc = new Calc();
            var result = calc.Parse("(1+2)+(2+3)");
            //var result = Convert.ToInt32(calc.findLeftValue("2"));
            
            Convert.ToInt32(result).Should().Be(8).And.BePositive("Upsssss");
        }
    }

    public class Calc
    {
        public string Parse(string expression)
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
                        tempExpression = this.Parse(tempExpression);
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
                            leftValue = FindLeftValue(expression, indexStartMultiply);
                            expression = expression.Remove(indexStartMultiply, leftValue.Length);
                            operation = expression[indexStartMultiply];
                            expression = expression.Remove(indexStartMultiply, 1);
                            if (indexStartMultiply != expression.Length - 1)
                            {
                                rightValue = FindRightValue(expression, indexStartMultiply);
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
                                    leftValue = FindLeftValue(expression);
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
                                rightValue = FindRightValue(expression);
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

        public string FindLeftValue(string expression, int indexStart = 0)
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

        public string FindRightValue(string expression, int indexStart = 0)
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

        public double calculation(string leftValue, char operation, string rightValue)
        {
            double result = 0;
            switch(operation)
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