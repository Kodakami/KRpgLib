using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLib.Stats.Compound.LogicOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void StatLiteral_Constructor_WithNullTemplate_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new StatLiteral(null, false);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void UnaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new UnaryOperationType(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void BinaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new BinaryOperationType(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void MultiaryOperationType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new MultiaryOperationType(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ComparisonType_Constructor_WithNullFunc_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new ComparisonType(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Unary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Unary(null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Unary_Constructor_WithNullValueExpression_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Negative;

            void exceptionalAction() => new ValueOperation_Unary(stubOperationType, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Binary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Binary_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.PowerOf;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary(stubOperationType, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Binary_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.PowerOf;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ValueOperation_Binary(stubOperationType, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Multiary_Constructor_WithNullOperationType_ThrowsArgNullEx()
        {
            var stubValueExpressionList = new List<ValueExpression>() { new FakeValueExpression(0) };

            void exceptionalAction() => new ValueOperation_Multiary(null, stubValueExpressionList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Multiary_Constructor_WithNullExpressionList_ThrowsArgNullEx()
        {
            var stubOperationType = CommonInstances.Add;
            List<ValueExpression> stubNullList = null;

            void exceptionalAction() => new ValueOperation_Multiary(stubOperationType, stubNullList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ValueOperation_Multiary_Constructor_WithNullExpressionInList_ThrowsArgEx()
        {
            var stubOperationType = CommonInstances.Add;
            var stubListWithNullEntry = new List<ValueExpression>() { null };

            void exceptionalAction() => new ValueOperation_Multiary(stubOperationType, stubListWithNullEntry);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ConditionalExpression_Constructor_WithNullCondition_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ConditionalExpression_Constructor_WithNullConsequent_ThrowsArgNullEx()
        {
            var stubCondition = new FakeLogicExpression(true);
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression(stubCondition, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void ConditionalExpression_Constructor_WithNullAlternative_ThrowsArgNullEx()
        {
            var stubCondition = new FakeLogicExpression(true);
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new ConditionalExpression(stubCondition, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
