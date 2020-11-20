using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLib.Stats.Compound.LogicOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class ValueExpressionTests_Exceptions
    {
        [TestMethod]
        public void Expression_Evaluate_WithNullStatSet_ThrowsArgNullEx()
        {
            var mockExpression = new FakeValueExpression(0);

            void exceptionalAction() => mockExpression.Evaluate(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void StatLiteral_Constructor_WithNullTemplate_ThrowsArgNullEx()
        {
            void exceptionalAction() => new StatLiteral<int>(null, false);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void UnaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            void exceptionalAction() => new UnaryOperationType<int>(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void BinaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            void exceptionalAction() => new BinaryOperationType<int>(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void MultiaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            void exceptionalAction() => new MultiaryOperationType<int>(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ComparisonType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            void exceptionalAction() => new ComparisonType<int>(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Unary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Unary<int>(null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Unary_Constructor_WithNullValueExpression_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Int.Negative;

            void exceptionalAction() => new ValueOperation_Unary<int>(stubOperationType, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Binary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary<int>(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Binary_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Int.PowerOf;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary<int>(stubOperationType, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Binary_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Int.PowerOf;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary<int>(stubOperationType, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Multiary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpressionList = new List<ValueExpression<int>>() { new FakeValueExpression(0) };

            void exceptionalAction() => new ValueOperation_Multiary<int>(null, stubValueExpressionList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Multiary_Constructor_WithNullExpressionList_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Int.Add;
            List<ValueExpression<int>> stubNullList = null;

            void exceptionalAction() => new ValueOperation_Multiary<int>(stubOperationType, stubNullList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Multiary_Constructor_WithEmptyExpressionList_ThrowsArgEx()
        {
            var stubOperationType = CommonInstances.Int.Add;
            var stubEmptyList = new List<ValueExpression<int>>();

            void exceptionalAction() => new ValueOperation_Multiary<int>(stubOperationType, stubEmptyList);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void ValueOperation_Multiary_Constructor_WithNullExpressionInList_ThrowsArgEx()
        {
            var stubOperationType = CommonInstances.Int.Add;
            var stubListWithNullEntry = new List<ValueExpression<int>>() { null };

            void exceptionalAction() => new ValueOperation_Multiary<int>(stubOperationType, stubListWithNullEntry);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void ConditionalExpression_Constructor_WithNullCondition_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression<int>(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ConditionalExpression_Constructor_WithNullConsequent_ThrowsArgNullEx()
        {
            var stubCondition = new FakeLogicExpression(true);
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression<int>(stubCondition, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void ConditionalExpression_Constructor_WithNullAlternative_ThrowsArgNullEx()
        {
            var stubCondition = new FakeLogicExpression(true);
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression<int>(stubCondition, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
