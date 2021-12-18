using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound;
using KRpgLib.Stats.Compound.LogicOperations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound
{
    [TestClass]
    public class LogicExpressionTests_Exceptions
    {
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void LogicOperation_Unary_Constructor_WithNullLogicExpression_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new LogicOperation_Not(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void LogicOperation_Binary_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubLogicExpression = new FakeLogicExpression(true);

            void exceptionalAction() => new LogicOperation_And(null, stubLogicExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void LogicOperation_Binary_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubLogicExpression = new FakeLogicExpression(true);

            void exceptionalAction() => new LogicOperation_And(stubLogicExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void LogicOperation_Multiary_Constructor_WithNullExpressionList_ThrowsArgNullEx()
        {
            List<LogicExpression> stubNullList = null;

            void exceptionalAction() => new LogicOperation_All(stubNullList);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void LogicOperation_Multiary_Constructor_WithNullExpressionInList_ThrowsArgEx()
        {
            var stubListWithNullEntry = new List<LogicExpression>() { null };

            void exceptionalAction() => new LogicOperation_All(stubListWithNullEntry);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Comparison_Constructor_WithNullComparisonType_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new Comparison(null, stubValueExpression, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Comparison_Constructor_WithNullLeftOperand_ThrowsArgNullEx()
        {
            var stubComparisonType = CommonInstances.EqualTo;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new Comparison(stubComparisonType, null, stubValueExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Comparison_Constructor_WithNullRightOperand_ThrowsArgNullEx()
        {
            var stubComparisonType = CommonInstances.EqualTo;
            var stubValueExpression = new FakeValueExpression(0);

            void exceptionalAction() => new Comparison(stubComparisonType, stubValueExpression, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
