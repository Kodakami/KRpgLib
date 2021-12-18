﻿using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace KRpgLibTests.Stats.Compound.AlgoBuilder
{
    [TestClass]
    public class ScannerTests
    {
        [DataRow(null, "NULL")]
        [DataRow("", "Empty String")]
        [DataRow(" ", "White Space")]
        [DataRow("\n", "Newline")]
        [DataRow("\r", "Carriage Return")]
        [DataRow("\t", "Tab")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        private static void Constructor_WithSimpleInvalidScript_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new Scanner("");

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow("(", TokenType.PAREN_OPEN, DisplayName = "Open Parenthesis: (")]
        [DataRow(")", TokenType.PAREN_CLOSED, DisplayName = "Closed Parenthesis: )")]
        [DataRow("0", TokenType.NUMBER, DisplayName = "Number: 0")]
        [DataRow("a", TokenType.IDENTIFIER, DisplayName = "Identifier: a")]
        [DataRow("$", TokenType.CASH, DisplayName = "Cash (dollar sign): $")]
        [DataRow("*", TokenType.ASTERISK, DisplayName = "Asterisk: *")]
        public void TryScanTokens_WithSingleCharScript_ReturnsCorrectToken(string singleCharScript, TokenType expectedType)
        {
            var mockScanner = new Scanner(singleCharScript);

            if (!mockScanner.TryScanTokens(out IReadOnlyList<Token> resultTokens))
            {
                Assert.Fail("Scanner reported a failure to parse the script.");
            }

            if (resultTokens.Count != 1)
            {
                Assert.Fail("Scanner returned zero or multiple tokens when one was expected.");
            }

            var singleToken = resultTokens[0];

            Assert.AreEqual(expectedType, singleToken.TokenType);
        }
        [TestMethod]
        public void TryScanTokens_WithValidScript_ReturnsCorrectTokens()
        {
            const string SIMPLE_SCRIPT = "(add 35 (mul *statName 3))";
            var expectedTokenTypes = new TokenType[]
            {
                TokenType.PAREN_OPEN,
                TokenType.IDENTIFIER,
                TokenType.NUMBER,
                TokenType.PAREN_OPEN,
                TokenType.IDENTIFIER,
                TokenType.ASTERISK,
                TokenType.IDENTIFIER,
                TokenType.NUMBER,
                TokenType.PAREN_CLOSED,
                TokenType.PAREN_CLOSED,
            };

            var mockScanner = new Scanner(SIMPLE_SCRIPT);

            if (!mockScanner.TryScanTokens(out IReadOnlyList<Token> resultTokens))
            {
                Assert.Fail("Scanner reported a failure to parse the script.");
            }

            var resultTypes = resultTokens.Select(t => t.TokenType).ToArray();

            CollectionAssert.AreEqual(expectedTokenTypes, resultTypes);
        }
    }
}
