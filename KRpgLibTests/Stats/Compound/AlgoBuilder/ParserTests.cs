using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Stats.Compound;

namespace KRpgLibTests.Stats.Compound.AlgoBuilder
{
    [TestClass]
    public class ParserTests
    {
        private static List<Token> GetStubTokenList() => new List<Token>()
        {
            // Script: "(add 1 2)"
            // Index:  "012345678"
            // Should return: new ValueOperation_Multiary<int>(CommonInstances.Int.Add, new Literal(1), new Literal(2)) as ValueExpression<int>

            new Token("(", TokenType.PAREN_OPEN, null, 0),
            new Token("add", TokenType.IDENTIFIER, "add", 1),
            new Token("1", TokenType.NUMBER, "1", 5),
            new Token("2", TokenType.NUMBER, "2", 7),
            new Token(")", TokenType.PAREN_CLOSED, null, 8),
        };

        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Constructor_WithNullTokenList_ThrowsArgNullEx()
        {
            var stubExpReg = new ExpressionRegistry();
            var stubStatReg = new StatRegistry();

            void exceptionalAction() => new Parser(null, stubExpReg, stubStatReg);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Constructor_WithNullExpressionRegistry_ThrowsArgNullEx()
        {
            var stubTokens = GetStubTokenList();
            var stubStatReg = new StatRegistry();

            void exceptionalAction() => new Parser(stubTokens, null, stubStatReg);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Constructor_WithNullStatTemplateRegistry_ThrowsArgNullEx()
        {
            var stubTokens = GetStubTokenList();
            var stubExpReg = new ExpressionRegistry();

            void exceptionalAction() => new Parser(stubTokens, stubExpReg, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void TryParseTokens_WithValidTokenList_ReturnsCorrectResult()
        {
            var stubTokens = GetStubTokenList();
            var stubExpReg = new ExpressionRegistry();
            stubExpReg.Add(new List<string>() { "add" },
                ParserUtilities.PopMultiaryValueParams,
                q => ParserUtilities.ConstructMultiaryOperation<ValueExpression>(
                    q, list => new ValueOperation_Multiary(CommonInstances.Add, list))
                );
            var stubStatReg = new StatRegistry();
            var stubStatSet = new FakeStatSet().Snapshot;
            var mockParser = new Parser(stubTokens, stubExpReg, stubStatReg);

            if (!mockParser.TryParseTokens(out ValueExpression resultAlgo))
            {
                Assert.Fail("Did not parse. Parser returned message: {0}", mockParser.StatusMessage);
            }

            var calculatedValue = resultAlgo.Evaluate(stubStatSet);

            const int EXPECTED_CALCULATION = 3;

            Assert.AreEqual(EXPECTED_CALCULATION, calculatedValue);
        }
    }
}
