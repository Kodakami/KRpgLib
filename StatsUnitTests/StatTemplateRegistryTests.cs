using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLib.UnitTests.StatsTests.Compound.AlgoBuilder
{
    [TestClass]
    public class StatTemplateRegistryTests
    {
        [TestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty string")]
        [DataRow(" ", DisplayName = "White-space only")]
        [DataRow(".", DisplayName = "Non-letter")]
        public void Add_WithInvalidIdentifier_ThrowsArgEx(string identifier)
        {
            var stubStat = new FakeStat(null, null, null, 0);
            var mockReg = new StatTemplateRegistry<int>();

            void exceptionalAction() => mockReg.Add(identifier, stubStat);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow("a", "a", DisplayName = "Same case")]
        [DataRow("a", "A", DisplayName = "Different cases")]
        public void Add_WithTakenIdentifier_ThrowsArgEx(string identifier, string otherIdentifier)
        {
            var stubStat = new FakeStat(null, null, null, 0);
            var mockReg = new StatTemplateRegistry<int>();
            mockReg.Add(identifier, stubStat);

            void exceptionalAction() => mockReg.Add(otherIdentifier, stubStat);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithNullStatTemplate_ThrowsArgNullEx()
        {
            var mockReg = new StatTemplateRegistry<int>();
            const string ANY_VALID_IDENTIFIER = "a";

            void exceptionalAction() => mockReg.Add(ANY_VALID_IDENTIFIER, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty string")]
        [DataRow(" ", DisplayName = "White-space only")]
        [DataRow(".", DisplayName = "Non-letter")]
        public void TryGetStatTemplate_WithInvalidIdentifier_ThrowsArgEx(string identifier)
        {
            var stubStat = new FakeStat(null, null, null, 0);
            var mockReg = new StatTemplateRegistry<int>();

            void exceptionalAction() => mockReg.TryGetStatTemplate(identifier, out _);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
    }
}
