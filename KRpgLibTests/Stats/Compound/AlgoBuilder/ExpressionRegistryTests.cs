using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Stats.Compound.AlgoBuilder;
using KRpgLib.Stats.Compound;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLibTests.Stats.Compound.AlgoBuilder
{
    [TestClass]
    public class ExpressionRegistryTests
    {
        private List<string> GetValidKeywordList()
        {
            return new List<string>() { "add" };
        }
        private PopParamsDelegate<int> GetValidPopParamsDelegate()
        {
            return ParserUtilities<int>.PopMultiaryValueParams;
        }
        private ExpressionCtorDelegate<int> GetValidExpressionCtorDelegate()
        {
            return q => ParserUtilities<int>.ConstructMultiaryOperation<ValueExpression<int>>(q,
                list => new ValueOperation_Multiary<int>(CommonInstances.Int.Add, list));
        }
        [TestMethod]
        public void Add_WithNullKeywordList_ThrowsArgEx()
        {
            var stubPopDelegate = GetValidPopParamsDelegate();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(null, stubPopDelegate, stubCtorDelegate);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithNullPopDelegate_ThrowsArgNullEx()
        {
            var stubKeywordList = GetValidKeywordList();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(stubKeywordList, null, stubCtorDelegate);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithNullCtorDelegate_ThrowsArgNullEx()
        {
            var stubKeywordList = GetValidKeywordList();
            var stubPopDelegate = GetValidPopParamsDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(stubKeywordList, stubPopDelegate, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithEmptyKeywordList_ThrowsArgEx()
        {
            var keywordList = new List<string>();
            var stubPopDelegate = GetValidPopParamsDelegate();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(keywordList, stubPopDelegate, stubCtorDelegate);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithNullKeywordInList_ThrowsArgEx()
        {
            var keywordList = new List<string>() { null };
            var stubPopDelegate = GetValidPopParamsDelegate();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(keywordList, stubPopDelegate, stubCtorDelegate);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(" ", DisplayName = "White-space")]
        [DataRow("1", DisplayName = "Number")]
        [DataRow("\n", DisplayName = "Control character")]
        public void Add_WithInvalidKeywordInList_ThrowsArgEx(string invalidKeyword)
        {
            var keywordList = new List<string>() { invalidKeyword };
            var stubPopDelegate = GetValidPopParamsDelegate();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();

            void exceptionalAction() => mockRegistry.Add(keywordList, stubPopDelegate, stubCtorDelegate);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Add_WithTakenKeywordInList_ThrowsArgEx()
        {
            var stubKeywordList = GetValidKeywordList();
            var stubPopDelegate = GetValidPopParamsDelegate();
            var stubCtorDelegate = GetValidExpressionCtorDelegate();
            var mockRegistry = new ExpressionRegistry<int>();
            void addToRegistry() => mockRegistry.Add(stubKeywordList, stubPopDelegate, stubCtorDelegate);
            addToRegistry();

            void exceptionalAction() => addToRegistry();

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
    }
}
