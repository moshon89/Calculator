using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Calculator.Models
{
    public class CalculatorModel
    {
        public readonly string Default = "0";
        public readonly string Error = "Syntax ERROR:\n";
        private string _errorMessage;
        private string _result;

        public string ErrorMessage
        {
            get { return _errorMessage; }
        }
        public string Result
        {
            get { return _result; }
        }

        /// <summary>
        /// gets an arithmetic expression as a string and check if it can be evaluated,
        /// if evaluation succeeded calculate the result, else return error message
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string Calculate(string expression)
        {
            if (ValidateExpression(expression)) return _result;
            return Error + _errorMessage;
        }

        /// <summary>
        /// manage the validations that expression should pass before being calculated
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private bool ValidateExpression(string expression)
        {
            try
            {
                char[] exp = expression.ToCharArray();
                CheckBracketsAndDoubleOverflow(exp);
                CheckBracketsSyntax(expression);
                string withSpaces = AddSpaces(exp);
                CalculateResult(withSpaces.ToCharArray());
            }
            catch(Exception e)
            {
                _errorMessage = e.Message;
                return false;
            }

            return true;
        }

        /// <summary>
        /// checks if the brackets and float points in expression are in reasonable places
        /// </summary>
        /// <param name="expression"></param>
        private void CheckBracketsSyntax(string expression)
        {
            Dictionary<Regex,string> regexes = new Dictionary<Regex, string>();
            regexes.Add(new Regex(@"[0-9][[]"), "an operator should be before brackets");// find 2[3]
            regexes.Add(new Regex(@"][[]"), "an operator should be between 2 brackets expression");// find [2][3]
            regexes.Add(new Regex(@"][0-9]"), "an operator should be between brackets and number");// find [2]3
            regexes.Add(new Regex(@"[.0-9][[]"), "floating point couldn't be before brackets");// find 2.[3]
            regexes.Add(new Regex(@"][.0-9]"), "floating point couldn't be after brackets");// find [2].3
            regexes.Add(new Regex(@"[[][*/+]"), "brackets cannot start with operator");// find [/2-3]
            regexes.Add(new Regex(@"^[+*/]"), "expression cannot start with operator");// find /2-3
            Match match;
            foreach (KeyValuePair<Regex, string> regex in regexes)
            {
                match = regex.Key.Match(expression);
                if (match.Success)
                {
                    _errorMessage = regex.Value;
                    throw new Exception(_errorMessage);
                }
            }
        }

        /// <summary>
        /// for make life easy to our calculating algorithm we help him to know where a number start
        /// and finish by adding spaces between numbers to operators and brackets
        /// </summary>
        /// <param name="exression"></param>
        /// <returns></returns>
        private string AddSpaces(char[] exression)
        {
            string withSpaces = "";

            for (int i = 0; i < exression.Length; i++)
            {
                char c = exression[i];
                if (i == 0 && c == '-') //case exression start with minus
                {
                    withSpaces += "0 " + c + " ";
                }
                else if (c == '-' && exression[i-1] == '[') ////case brackets start with minus
                {
                    withSpaces += "0 " + c + " ";
                }
                else if (IsOperator(c.ToString()) || IsBrackets(c.ToString()))
                {
                    withSpaces += " " + c + " ";
                }
                else
                {
                    withSpaces += c;
                }
                    
            }
            return withSpaces;
        }

        /// <summary>
        /// return whether string is operator or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsOperator(string str)
        {
            return str.Equals("+") || str.Equals("/") || str.Equals("*") || str.Equals("-");
        }

        /// <summary>
        /// return whether string is bracket or not
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool IsBrackets(string str)
        {
            return str.Equals("[") || str.Equals("]");
        }

        /// <summary>
        /// ***Full Disclosure***
        /// i knew this algorithm before from "Software structure" course.
        /// i have read this algorithm pseudo code but the implementation is mine..
        /// 
        /// main calculating algorithm - works by evaluate Infix arithmetic expression
        /// and push the operators to stack of operators and the numbers to stack of numbers
        /// gets them out in right order and calculate the result
        /// </summary>
        /// <param name="expression"></param>
        private void CalculateResult(char[] expression)
        {
            // holds all numbers
            Stack<double> numbers = new Stack<double>();

            // holds all operators and brackets
            Stack<char> operators = new Stack<char>();

            for (int i = 0; i < expression.Length; i++)
            {
                // when i is space we know that it's end of subExpression 
                if (expression[i] == ' ')
                    continue;

                // if its a number, find the end of it and push it to operators stack as a double 
                if ((expression[i] >= '0' && expression[i] <= '9') || expression[i] == '.')
                {
                    string number = "";
                    while (i < expression.Length &&
                           ((expression[i] >= '0' && expression[i] <= '9') || expression[i] == '.'))
                    {
                        //check if the number contain more than 1 floating point
                        if (expression[i] == '.' && number.Contains("."))
                        {
                            _errorMessage = "number contains more than one floating point";
                            throw new Exception(_errorMessage);
                        }
                        number += expression[i++];
                    }
                    numbers.Push(Convert.ToDouble(number));
                }

                // if it's an opening bracket, push to operators
                else if (expression[i] == '[')
                    operators.Push(expression[i]);

                // if it's an opening bracket 
                else if (expression[i] == ']')
                {
                    // calculate brackets subExpression
                    while (operators.Peek() != '[')
                    {
                        numbers.Push(CalculateOperator(operators.Pop(), numbers.Pop(), numbers.Pop()));
                    }
                    operators.Pop();
                }

                // if it's an operator
                else if (IsOperator(expression[i].ToString()))
                {
                    // While top of operators has same or greater priority from current 
                    // operator, calculate operator on top of operators 
                    // with top two elements in numbers stack
                    while (operators.Any() && HasPriority(expression[i], operators.Peek()))
                    {
                        numbers.Push(CalculateOperator(operators.Pop(), numbers.Pop(), numbers.Pop()));
                    }
                    
                    //push the operator
                    operators.Push(expression[i]);
                }
            }

            // after we pushed the whole expression to the stacks,
            // calculate the rest of the elements
            while (operators.Any())
            {
                numbers.Push(CalculateOperator(operators.Pop(), numbers.Pop(), numbers.Pop()));
            }

            //the last element remain in stack is the result
            _result = numbers.Pop().ToString();
        }

        /// <summary>
        /// check witch operator should be calculated first (  /,* before +,-  )
        /// </summary>
        /// <param name="firstOperator"></param>
        /// <param name="secondOperator"></param>
        /// <returns></returns>
        private bool HasPriority(char firstOperator, char secondOperator)
        {
            if (secondOperator == '[' || secondOperator == ']')
            {
                return false;
            }

            if ((firstOperator == '*' || firstOperator == '/') && (secondOperator == '+' || secondOperator == '-'))
            {
                return false;
            }
                return true;
        }

        /// <summary>
        /// get an operator and two variables and return the result of applying
        /// this operator on the variables on right order.
        /// </summary>
        /// <param name="op"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        private double CalculateOperator(char oper, double second, double first)
        {
            switch (oper)
            {
                case '+':
                    return first + second;
                case '-':
                    return first - second;
                case '*':
                    return first * second;
                case '/':
                    if (second == 0)
                        throw new Exception("Divide by 0 is not allowed");
                           
                    return first / second;
            }
            return 0;
        }

        /// <summary>
        /// check that the expression has balanced brackets,
        /// And if the number is longer than double object
        /// </summary>
        /// <param name="exp"></param>
        private void CheckBracketsAndDoubleOverflow(char[] exp)
        {
            int counter = 0;
            for (int i = 0; i < exp.Length; i++)
            {
                char c = exp[i];
                switch (c)
                {
                    case 'E':
                        throw new Exception("this number is too long :(");
                    case '[':
                    {
                        counter++;
                        break;
                    }
                    case ']':
                        counter--;
                        break;
                }

                if (counter < 0)
                {
                    throw new Exception("Unbalanced brackets");
                }
            }

            if (counter != 0)
            {
                throw new Exception("Unbalanced brackets");
            }
        }
    }
}
