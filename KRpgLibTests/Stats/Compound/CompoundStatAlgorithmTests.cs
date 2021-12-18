﻿using KRpgLib.Stats.Compound;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KRpgLibTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatAlgorithmTests
    {
        [TestMethod]
        public void CalculateValue_WithValidAlgorithm_ReturnsCorrectValue()
        {
            var stubSet = new FakeStatSet().Snapshot;
            var stubExpression = new FakeValueExpression(0);
            var mockAlgorithm = new CompoundStatAlgorithm(stubExpression);

            var resultValue = mockAlgorithm.CalculateValue(stubSet);

            Assert.AreEqual(0, resultValue);
        }
        [TestMethod]
        public void Constructor_WithNullAlgorithm_ThrowsArgNullEx()
        {
            FakeValueExpression stubNullExpression = null;

            void exceptionalAction() => new CompoundStatAlgorithm(stubNullExpression);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void CalculateValue_WithNullStatSet_ThrowsArgNullEx()
        {
            var stubValueExpression = new FakeValueExpression(0);
            var mockAlgorithm = new CompoundStatAlgorithm(stubValueExpression);
            KRpgLib.Stats.StatSnapshot stubNullSet = null;

            void exceptionalAction() => mockAlgorithm.CalculateValue(stubNullSet);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
