using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Stats.Compound;

namespace StatsUnitTests.Compound.AlgoBuilder
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
        public void Constructor_WithNullTokenList_ThrowsArgNullEx()
        {
            var stubExpReg = new ExpressionRegistry<int>();
            var stubStatReg = new StatTemplateRegistry<int>();

            void exceptionalAction() => new Parser_Int(null, stubExpReg, stubStatReg);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithNullExpressionRegistry_ThrowsArgNullEx()
        {
            var stubTokens = GetStubTokenList();
            var stubStatReg = new StatTemplateRegistry<int>();

            void exceptionalAction() => new Parser_Int(stubTokens, null, stubStatReg);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Constructor_WithNullStatTemplateRegistry_ThrowsArgNullEx()
        {
            var stubTokens = GetStubTokenList();
            var stubExpReg = new ExpressionRegistry<int>();

            void exceptionalAction() => new Parser_Int(stubTokens, stubExpReg, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void TryParseTokens_WithValidTokenList_ReturnsCorrectResult()
        {
            var stubTokens = GetStubTokenList();
            var stubExpReg = new ExpressionRegistry<int>();
            stubExpReg.Add(new List<string>() { "add" },
                ParserUtilities<int>.PopMultiaryValueParams,
                q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(
                    q, list => new ValueOperation_Multiary<int>(CommonInstances.Int.Add, list))
                );
            var stubStatReg = new StatTemplateRegistry<int>();
            var stubStatSet = new FakeStatSet();
            var mockParser = new Parser_Int(stubTokens, stubExpReg, stubStatReg);

            if (!mockParser.TryParseTokens(out ValueExpression<int> resultAlgo))
            {
                Assert.Fail("Did not parse. Parser returned message: {0}", mockParser.StatusMessage);
            }

            var calculatedValue = resultAlgo.Evaluate(stubStatSet);

            const int EXPECTED_CALCULATION = 3;

            Assert.AreEqual(EXPECTED_CALCULATION, calculatedValue);
        }
    }
}
