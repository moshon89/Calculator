﻿using System;
using Calculator.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculator.ViewModels;

namespace UnitTestCalculator
{
    [TestClass]
    public class UnitTest1
    {
        CalculatorModel calcModel = new CalculatorModel();

        [TestMethod]
        public void Multiple()
        {
            calcModel.Calculate("5*5");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("25"));
        }

        [TestMethod]
        public void Divide()
        {
            calcModel.Calculate("5/5");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("1"));
        }

        [TestMethod]
        public void DivideFloat()
        {
            calcModel.Calculate("5/10");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("0.5"));
        }

        [TestMethod]
        public void Subtruct()
        {
            calcModel.Calculate("5-5");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("0"));
        }

        [TestMethod]
        public void BracketsWithMinus()
        {
            calcModel.Calculate("5*[-5]");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("-25"));
        }

        [TestMethod]
        public void Priority()
        {
            calcModel.Calculate("2+2*2");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("6"));
        }

        [TestMethod]
        public void FloatingPoint()
        {
            calcModel.Calculate("2+2.2");
            string result = calcModel.Result;
            Console.WriteLine(result);
            Assert.IsTrue(result.Equals("4.2"));
        }

        [TestMethod]
        public void MoreThanOneFloatingPoint()
        {
            calcModel.Calculate("2+2.4.2");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("number contains more than one floating point"));
        }

        [TestMethod]
        public void BracketAfterNumber()
        {
            calcModel.Calculate("70[2]");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("an operator should be before brackets"));
        }

        [TestMethod]
        public void BracketsWithoutOperatorBetween()
        {
            calcModel.Calculate("[2][1]");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("an operator should be between 2 brackets expression"));
        }

        [TestMethod]
        public void BracketBeforeNumber()
        {
            calcModel.Calculate("[2]70");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("an operator should be between brackets and number"));
        }

        [TestMethod]
        public void PointAfterBracket()
        {
            calcModel.Calculate("[2].70");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("floating point couldn't be after brackets"));
        }

        [TestMethod]
        public void PointBeforeBracket()
        {
            calcModel.Calculate("2.[70]");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("floating point couldn't be before brackets"));
        }

        [TestMethod]
        public void StartWithOperator()
        {
            calcModel.Calculate("*8+3");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("expression cannot start with operator"));
        }

        [TestMethod]
        public void BracketsStartWithOperator()
        {
            calcModel.Calculate("8+3*[/4-2]");
            string result = calcModel.ErrorMessage;
            Console.WriteLine(result);
            Assert.IsTrue(result.Contains("brackets cannot start with operator"));
        }
    }
}
