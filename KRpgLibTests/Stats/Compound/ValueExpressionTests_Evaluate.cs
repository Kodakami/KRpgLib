using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLibTests.Stats;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound
{
    [TestClass]
    public class ValueExpressionTests_Evaluate
    {
        private static void ValueExpressionTest(ValueExpression testedExpression, int expectedValue)
        {
            var stubSet = new FakeStatSet().Snapshot;

            var resultValue = testedExpression.Evaluate(stubSet);

            Assert.AreEqual(expectedValue, resultValue);
        }
        [TestMethod]
        public void Literal_Evaluate_ReturnsCorrectValue()
        {
            var mockLiteral = new Literal(0);

            ValueExpressionTest(mockLiteral, 0);
        }
        [TestMethod]
        public void StatLiteral_NonLegalized_Evaluate_ReturnsCorrectValue()
        {
            var mockLiteral = new StatLiteral(FakeStatSet.TestStat_Raw3_Legal2_Provided, false);
            const int RAW_VALUE = 3;

            ValueExpressionTest(mockLiteral, RAW_VALUE);
        }
        [TestMethod]
        public void StatLiteral_Legalized_Evaluate_ReturnsCorrectValue()
        {
            var mockLiteral = new StatLiteral(FakeStatSet.TestStat_Raw3_Legal2_Provided, true);
            const int LEGAL_VALUE = 2;

            ValueExpressionTest(mockLiteral, LEGAL_VALUE);
        }
        private static void UnaryOpTest(UnaryOperationType operationType, int input, int expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubValueExpression = new Literal(input);
            var mockExpression = new ValueOperation_Unary(operationType, stubValueExpression);

            var resultValue = mockExpression.Evaluate(stubStatSet);

            Assert.AreEqual(expected, resultValue);
        }
        [TestMethod]
        public void Negative_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Negative;
            const int INPUT_VALUE = 1;
            const int EXPECTED_RESULT = -1;

            UnaryOpTest(operationType, INPUT_VALUE, EXPECTED_RESULT);
        }
        private static void BinaryOpTest(BinaryOperationType operationType, int left, int right, int expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubValueExpressionLeft = new Literal(left);
            var stubValueExpressionRight = new Literal(right);
            var mockExpression = new ValueOperation_Binary(operationType, stubValueExpressionLeft, stubValueExpressionRight);

            var resultValue = mockExpression.Evaluate(stubStatSet);

            Assert.AreEqual(expected, resultValue);
        }
        [TestMethod]
        [DataRow(4, 2, 2, DisplayName = "Typical Division: (4 / 2) == 2")]
        [DataRow(1, 0, 0, DisplayName = "Safe Division by Zero: (1 / 0) == 0")]
        public void Divide_Evaluate_ReturnsCorrectResult(int left, int right, int expected)
        {
            var operationType = CommonInstances.Divide;

            BinaryOpTest(operationType, left, right, expected);
        }
        [TestMethod]
        [DataRow(3, 2, 1, DisplayName = "Typical Modulo: (3 % 2) == 1")]
        [DataRow(1, 0, 0, DisplayName = "Safe Modulo by Zero: (1 % 0) == 0")]
        public void Modulo_Evaluate_ReturnsCorrectResult(int left, int right, int expected)
        {
            var operationType = CommonInstances.Modulo;

            BinaryOpTest(operationType, left, right, expected);
        }
        [TestMethod]
        public void PowerOf_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.PowerOf;
            const int BASE = 2;
            const int EXPONENT = 2;
            const int EXPECTED_RESULT = 4;

            BinaryOpTest(operationType, BASE, EXPONENT, EXPECTED_RESULT);
        }
        private static void MultiaryOpTest(MultiaryOperationType operationType, int item1, int item2, int item3, int expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubValueExpressionList = new List<ValueExpression>() { new Literal(item1), new Literal(item2), new Literal(item3) };
            var mockExpression = new ValueOperation_Multiary(operationType, stubValueExpressionList);

            var resultValue = mockExpression.Evaluate(stubStatSet);

            Assert.AreEqual(expected, resultValue);
        }
        [TestMethod]
        public void Add_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Add;
            const int INPUT_VALUE = 1;
            const int EXPECTED_RESULT = 3;

            MultiaryOpTest(operationType, INPUT_VALUE, INPUT_VALUE, INPUT_VALUE, EXPECTED_RESULT);
        }
        [TestMethod]
        public void Subtract_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Subtract;
            const int INPUT_VALUE = 1;
            const int EXPECTED_RESULT = -1;

            MultiaryOpTest(operationType, INPUT_VALUE, INPUT_VALUE, INPUT_VALUE, EXPECTED_RESULT);
        }
        [TestMethod]
        public void Multiply_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Multiply;
            const int INPUT_VALUE = 2;
            const int EXPECTED_RESULT = 8;

            MultiaryOpTest(operationType, INPUT_VALUE, INPUT_VALUE, INPUT_VALUE, EXPECTED_RESULT);
        }
        [TestMethod]
        public void Min_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Smallest;
            const int INPUT_VALUE_1 = 1;
            const int INPUT_VALUE_2 = 2;
            const int INPUT_VALUE_3 = 3;
            const int EXPECTED_RESULT = 1;

            MultiaryOpTest(operationType, INPUT_VALUE_1, INPUT_VALUE_2, INPUT_VALUE_3, EXPECTED_RESULT);
        }
        [TestMethod]
        public void Max_Evaluate_ReturnsCorrectResult()
        {
            var operationType = CommonInstances.Biggest;
            const int INPUT_VALUE_1 = 1;
            const int INPUT_VALUE_2 = 2;
            const int INPUT_VALUE_3 = 3;
            const int EXPECTED_RESULT = 3;

            MultiaryOpTest(operationType, INPUT_VALUE_1, INPUT_VALUE_2, INPUT_VALUE_3, EXPECTED_RESULT);
        }

        [TestMethod]
        [DataRow(true, 0, DisplayName = "With True Condition")]
        [DataRow(false, 1, DisplayName = "With False Condition")]
        public void ConditionalExpression_Evaluate_ReturnsCorrectResult(bool conditionResult, int expected)
        {
            var stubStatSet = new FakeStatSet().Snapshot;
            var stubCondition = new FakeLogicExpression(conditionResult);
            const int ANY_VALUE = 0;
            var stubLiteralValueConsequent = new Literal(ANY_VALUE);
            const int ANY_OTHER_VALUE = 1;
            var stubLiteralValueAlternative = new Literal(ANY_OTHER_VALUE);
            var mockConditionalExpression = new ConditionalExpression(stubCondition, stubLiteralValueConsequent, stubLiteralValueAlternative);

            var resultValue = mockConditionalExpression.Evaluate(stubStatSet);

            Assert.AreEqual(expected, resultValue);
        }
    }
}
