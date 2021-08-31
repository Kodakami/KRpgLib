using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLib.Stats.Compound.LogicOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound
{
    [TestClass]
    public class LogicExpressionTests_Evaluate
    {
        // Unary Operations.
        [TestMethod]
        public void Not_Evaluate_ReturnsCorrectValue()
        {
            const bool LITERAL_INPUT = true;
            const bool EXPECTED_OUTPUT = false;

            var stubStatSet = new FakeStatSet().Snapshot;
            var stubLiteral = new FakeLogicExpression(LITERAL_INPUT);
            var mockOperation = new LogicOperation_Not<int>(stubLiteral);

            var resultValue = mockOperation.Evaluate(stubStatSet);

            Assert.AreEqual(resultValue, EXPECTED_OUTPUT);
        }

        // Binary Operations.
        private void BinaryLogicTest(
            Func<LogicExpression<int>, LogicExpression<int>, LogicOperation_Binary<int>> mockOperationGetter,
            bool left, bool right, bool expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubLiteralLeft = new FakeLogicExpression(left);
            var stubLiteralRight = new FakeLogicExpression(right);
            var mockOperation = mockOperationGetter(stubLiteralLeft, stubLiteralRight);

            var resultValue = mockOperation.Evaluate(stubStatSet);

            Assert.AreEqual(resultValue, expected);
        }
        [TestMethod]
        [DataRow(true, true, true, DisplayName = "T && T == T")]
        [DataRow(true, false, false, DisplayName = "T && F == F")]
        [DataRow(false, false, false, DisplayName = "F && F == F")]
        public void And_Evaluate_ReturnsCorrectValue(bool left, bool right, bool expected)
        {
            LogicOperation_Binary<int> mockOperationGetter(LogicExpression<int> l, LogicExpression<int> r) => new LogicOperation_And<int>(l, r);

            BinaryLogicTest(mockOperationGetter, left, right, expected);
        }
        [TestMethod]
        [DataRow(true, true, true, DisplayName = "T || T == T")]
        [DataRow(true, false, true, DisplayName = "T || F == T")]
        [DataRow(false, false, false, DisplayName = "F || F == F")]
        public void Or_Evaluate_ReturnsCorrectValue(bool left, bool right, bool expected)
        {
            LogicOperation_Binary<int> mockOperationGetter(LogicExpression<int> l, LogicExpression<int> r) => new LogicOperation_Or<int>(l, r);

            BinaryLogicTest(mockOperationGetter, left, right, expected);
        }
        [TestMethod]
        [DataRow(true, true, false, DisplayName = "T ^ T == F")]
        [DataRow(true, false, true, DisplayName = "T ^ F == T")]
        [DataRow(false, false, false, DisplayName = "F ^ F == F")]
        public void Xor_Evaluate_ReturnsCorrectValue(bool left, bool right, bool expected)
        {
            LogicOperation_Binary<int> mockOperationGetter(LogicExpression<int> l, LogicExpression<int> r) => new LogicOperation_Xor<int>(l, r);

            BinaryLogicTest(mockOperationGetter, left, right, expected);
        }

        // Multiary Operations.
        private void MultiaryLogicTest(
            Func<List<LogicExpression<int>>, LogicOperation_Multiary<int>> mockOperationGetter,
            bool item1, bool item2, bool item3, bool expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubLiteral1 = new FakeLogicExpression(item1);
            var stubLiteral2 = new FakeLogicExpression(item2);
            var stubLiteral3 = new FakeLogicExpression(item3);
            var stubExpressionList = new List<LogicExpression<int>>() { stubLiteral1, stubLiteral2, stubLiteral3 };
            var mockOperation = mockOperationGetter(stubExpressionList);

            var resultValue = mockOperation.Evaluate(stubStatSet);

            Assert.AreEqual(resultValue, expected);
        }
        [TestMethod]
        [DataRow(true, true, true, true, DisplayName = "All(TTT) == T")]
        [DataRow(false, true, true, false, DisplayName = "All(FTT) == F")]
        public void All_Evaluate_ReturnsCorrectValue(bool item1, bool item2, bool item3, bool expected)
        {
            LogicOperation_Multiary<int> mockOperationGetter(List<LogicExpression<int>> list) => new LogicOperation_All<int>(list);

            MultiaryLogicTest(mockOperationGetter, item1, item2, item3, expected);
        }
        [TestMethod]
        [DataRow(true, false, false, true, DisplayName = "Any(TFF) == T")]
        [DataRow(false, false, false, false, DisplayName = "Any(FFF) == F")]
        public void Any_Evaluate_ReturnsCorrectValue(bool item1, bool item2, bool item3, bool expected)
        {
            LogicOperation_Multiary<int> mockOperationGetter(List<LogicExpression<int>> list) => new LogicOperation_Any<int>(list);

            MultiaryLogicTest(mockOperationGetter, item1, item2, item3, expected);
        }
        [TestMethod]
        [DataRow(true, true, false, false, DisplayName = "One(TTF) == F")]
        [DataRow(true, false, false, true, DisplayName = "One(TFF) == T")]
        [DataRow(false, false, false, false, DisplayName = "One(FFF) == F")]
        public void One_Evaluate_ReturnsCorrectValue(bool item1, bool item2, bool item3, bool expected)
        {
            LogicOperation_Multiary<int> mockOperationGetter(List<LogicExpression<int>> list) => new LogicOperation_One<int>(list);

            MultiaryLogicTest(mockOperationGetter, item1, item2, item3, expected);
        }

        private void ComparisonTest(ComparisonType<int> comparisonType, int left, int right, bool expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubLiteralLeft = new Literal<int>(left);
            var stubLiteralRight = new Literal<int>(right);
            var mockOperation = new KRpgLib.Stats.Compound.Comparison<int>(comparisonType, stubLiteralLeft, stubLiteralRight);

            var resultBool = mockOperation.Evaluate(stubStatSet);

            Assert.AreEqual(expected, resultBool);
        }
        [TestMethod]
        [DataRow(0, 0, true, DisplayName = "0 == 0 [T]")]
        [DataRow(0, 1, false, DisplayName = "0 == 1 [F]")]
        public void EqualTo_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.EqualTo;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
        [TestMethod]
        [DataRow(0, 1, true, DisplayName = "0 != 1 [T]")]
        [DataRow(0, 0, false, DisplayName = "0 != 0 [F]")]
        public void NotEqualTo_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.NotEqualTo;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
        [TestMethod]
        [DataRow(1, 0, true, DisplayName = "1 > 0 [T]")]
        [DataRow(0, 0, false, DisplayName = "0 > 0 [F]")]
        [DataRow(0, 1, false, DisplayName = "0 > 1 [F]")]
        public void GreaterThan_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.GreaterThan;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
        [TestMethod]
        [DataRow(1, 0, true, DisplayName = "1 >= 0 [T]")]
        [DataRow(0, 0, true, DisplayName = "0 >= 0 [T]")]
        [DataRow(0, 1, false, DisplayName = "0 >= 1 [F]")]
        public void GreaterThanOrEqualTo_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.GreaterThanOrEqualTo;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
        [TestMethod]
        [DataRow(0, 1, true, DisplayName = "0 < 1 [T]")]
        [DataRow(0, 0, false, DisplayName = "0 < 0 [F]")]
        [DataRow(1, 0, false, DisplayName = "1 < 0 [F]")]
        public void LessThan_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.LessThan;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
        [TestMethod]
        [DataRow(0, 1, true, DisplayName = "0 <= 1 [T]")]
        [DataRow(0, 0, true, DisplayName = "0 <= 0 [T]")]
        [DataRow(1, 0, false, DisplayName = "1 <= 0 [F]")]
        public void LessThanOrEqualTo_Evaluate_ReturnsCorrectValue(int left, int right, bool expected)
        {
            var mockComparisonType = CommonInstances.Int.LessThanOrEqualTo;

            ComparisonTest(mockComparisonType, left, right, expected);
        }
    }
}
