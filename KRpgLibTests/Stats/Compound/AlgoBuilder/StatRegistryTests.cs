using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound.AlgoBuilder
{
    [TestClass]
    public class StatRegistryTests
    {
        [TestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty string")]
        [DataRow(" ", DisplayName = "White-space only")]
        [DataRow("\a", DisplayName = "Control Character (\\a)")]
        [DataRow(".", DisplayName = "Non-letter, non-underscore")]
        public void Add_WithInvalidIdentifier_ThrowsArgEx(string identifier)
        {
            var stubStat = new FakeStat(0, null, null, null);
            var mockReg = new StatRegistry();

            void exceptionalAction() => mockReg.Add(identifier, stubStat);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow("a", "a", DisplayName = "Same case")]
        [DataRow("a", "A", DisplayName = "Different cases")]
        public void Add_WithTakenIdentifier_ThrowsArgEx(string identifier, string otherIdentifier)
        {
            var stubStat = new FakeStat(0, null, null, null);
            var mockReg = new StatRegistry();
            mockReg.Add(identifier, stubStat);

            void exceptionalAction() => mockReg.Add(otherIdentifier, stubStat);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithNullStatTemplate_ThrowsArgNullEx()
        {
            var mockReg = new StatRegistry();
            const string ANY_VALID_IDENTIFIER = "a";

            void exceptionalAction() => mockReg.Add(ANY_VALID_IDENTIFIER, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Empty string")]
        [DataRow(" ", DisplayName = "White-space only")]
        [DataRow("\a", DisplayName = "Control Character (\\a)")]
        [DataRow(".", DisplayName = "Non-letter, non-underscore")]
        public void TryGetStat_WithInvalidIdentifier_ThrowsArgEx(string identifier)
        {
            var stubStat = new FakeStat(0, null, null, null);
            var mockReg = new StatRegistry();

            void exceptionalAction() => mockReg.TryGetStat(identifier, out _);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
    }
}
