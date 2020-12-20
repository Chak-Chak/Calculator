using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculator.Interfaces;

namespace Calculator.Models
{
    public class Calc : ICalculator
    {
        public Calc()
        {

        }
        public string Parse(string expression)
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

        public string calculation(string leftValue, char operation, string rightValue)
        {
            string result = "";
            int leftValueLength = leftValue.Length;
            int rightValueLength = rightValue.Length;
            int countLeftValueCharAfterPoint = 0;
            int countRightValueCharAfterPoint = 0;
            int leftValuePointPosition = -1;
            int rightValuePointPosition = -1;

            try
            {
                for (int i = 0; i < leftValue.Length; i++)
                {
                    if ((leftValue[i] == '.') || (leftValue[i] == ','))
                    {
                        leftValuePointPosition = i;
                        for (int j = leftValuePointPosition + 1; j < leftValue.Length; j++)
                        {
                            countLeftValueCharAfterPoint++;
                        }
                        break;
                    }
                }

                for (int i = 0; i < rightValue.Length; i++)
                {
                    if ((rightValue[i] == '.') || (rightValue[i] == ','))
                    {
                        rightValuePointPosition = i;
                        for (int j = rightValuePointPosition + 1; j < rightValue.Length; j++)
                        {
                            countRightValueCharAfterPoint++;
                        }
                        break;
                    }
                }

                leftValue = leftValue.Replace(".", ",");
                rightValue = rightValue.Replace(".", ",");
                double tempLeftValue = Convert.ToDouble(leftValue);
                double tempRightValue = Convert.ToDouble(rightValue);
                if (countLeftValueCharAfterPoint >= countRightValueCharAfterPoint)
                {
                    for (int i = 0; i < countLeftValueCharAfterPoint; i++)
                    {
                        tempLeftValue = tempLeftValue * 10;
                        tempRightValue = tempRightValue * 10;
                    }
                }
                else
                {
                    if (countLeftValueCharAfterPoint < countRightValueCharAfterPoint)
                    {
                        for (int i = 0; i < countRightValueCharAfterPoint; i++)
                        {
                            tempLeftValue = tempLeftValue * 10;
                            tempRightValue = tempRightValue * 10;
                        }
                    }
                }

                leftValue = tempLeftValue.ToString();
                rightValue = tempRightValue.ToString();
                switch (operation)
                {
                    case '+':
                        {
                            result = (Convert.ToDouble(leftValue) + Convert.ToDouble(rightValue)).ToString();
                            if ((leftValuePointPosition > 0) && (rightValuePointPosition > 0) && (leftValuePointPosition > rightValuePointPosition))
                                result = result.Insert(leftValuePointPosition, ",");
                            else if ((leftValuePointPosition > 0) && (rightValuePointPosition > 0) && (leftValuePointPosition < rightValuePointPosition)) result = result.Insert(rightValuePointPosition, ",");
                            if ((leftValuePointPosition <= 0) && (rightValuePointPosition > 0)) result = result.Insert(rightValuePointPosition, ",");
                            if ((leftValuePointPosition > 0) && (rightValuePointPosition <= 0)) result = result.Insert(leftValuePointPosition, ",");
                            break;
                        }
                    case '-':
                        {
                            result = (Convert.ToDouble(leftValue) - Convert.ToDouble(rightValue)).ToString();
                            if (Convert.ToDouble(leftValue) < Convert.ToDouble(rightValue))
                                if ((leftValuePointPosition > 0) && (rightValuePointPosition > 0) && (leftValuePointPosition > rightValuePointPosition))
                                    result = result.Insert(leftValuePointPosition, ",");
                                else if ((leftValuePointPosition > 0) && (rightValuePointPosition > 0) && (leftValuePointPosition < rightValuePointPosition)) result = result.Insert(rightValuePointPosition, ",");
                            if ((leftValuePointPosition <= 0) && (rightValuePointPosition > 0)) result = result.Insert(rightValuePointPosition, ",");
                            if ((leftValuePointPosition > 0) && (rightValuePointPosition <= 0)) result = result.Insert(leftValuePointPosition, ",");
                            break;
                        }
                    case '*':
                        {
                            result = (Convert.ToDouble(leftValue) * Convert.ToDouble(rightValue)).ToString();
                            result = result.Insert(result.Length - countLeftValueCharAfterPoint - countRightValueCharAfterPoint, ",");
                            break;
                        }
                    case '/':
                        {
                            result = (Convert.ToDouble(leftValue) / Convert.ToDouble(rightValue)).ToString();
                            break;
                        }
                }
                return result;
            }
            catch
            {
                return result = "error";
            }
        }

    }
    
}
