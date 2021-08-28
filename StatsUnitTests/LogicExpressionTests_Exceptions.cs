using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLib.Stats.Compound.LogicOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLib.UnitTests.StatsTests.Compound
{
    [TestClass]
    public class LogicExpressionTests_Exceptions
    {
        [TestMethod]
        public void LogicOperation_Unary_Constructor_WithNullLogicExpression_ThrowsArgNullEx()
        {
            void exceptionalAction() => new LogicOperation_Not<int>(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void LogicOperation_Binary_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubLogicExpression = new FakeLogicExpression(true);

            void exceptionalAction() => new LogicOperation_And<int>(null, stubLogicExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void LogicOperation_Binary_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubLogicExpression = new FakeLogicExpression(true);

            void exceptionalAction() => new LogicOperation_And<int>(stubLogicExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void LogicOperation_Multiary_Constructor_WithNullExpressionList_ThrowsArgNullEx()
        {
            List<LogicExpression<int>> stubNullList = null;

            void exceptionalAction() => new LogicOperation_All<int>(stubNullList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void LogicOperation_Multiary_Constructor_WithEmptyExpressionList_ThrowsArgEx()
        {
            var stubEmptyList = new List<LogicExpression<int>>();

            void exceptionalAction() => new LogicOperation_All<int>(stubEmptyList);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void LogicOperation_Multiary_Constructor_WithNullExpressionInList_ThrowsArgEx()
        {
            var stubListWithNullEntry = new List<LogicExpression<int>>() { null };

            void exceptionalAction() => new LogicOperation_All<int>(stubListWithNullEntry);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Comparison_Constructor_WithNullComparisonType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new KRpgLib.Stats.Compound.Comparison<int>(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Comparison_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubComparisonType = CommonInstances.Int.EqualTo;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new KRpgLib.Stats.Compound.Comparison<int>(stubComparisonType, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Comparison_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubComparisonType = CommonInstances.Int.EqualTo;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new KRpgLib.Stats.Compound.Comparison<int>(stubComparisonType, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
