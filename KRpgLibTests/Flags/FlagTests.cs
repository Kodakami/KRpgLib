using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Flags;
using System;

namespace KRpgLibTests.Flags
{
    [TestClass]
    public class FlagTests
    {
        [TestMethod]
        public void Create_WithNullTemplate_ThrowsArgNullEx()
        {
            static void exceptionalAction() => Flag.Create(null, 0);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void Create_WithInvalidTemplate_ThrowsArgEx()
        {
            var stubInvalidTemplate = new FakeFlagTemplate(variantCount: 0);

            void exceptionalAction() => Flag.Create(stubInvalidTemplate, 0);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        [DataRow(-1, DisplayName = "Negative")]
        [DataRow(1, DisplayName = "Greater than max variant index")]
        public void Create_WithOutOfRangeIndex_ThrowsArgEx(int outOfRangeIndex)
        {
            var stubTemplate = new FakeFlagTemplate(variantCount: 1);

            void exceptionalAction() => Flag.Create(stubTemplate, outOfRangeIndex);

            Assert.ThrowsException<ArgumentException>(exceptionalAction);
        }
        [TestMethod]
        public void Create_WithValidInputs_ReturnsCorrectResult()
        {
            var stubTemplate = new FakeFlagTemplate(variantCount: 1);
            const int VARIANT_INDEX = 0;

            var mockFlag = Flag.Create(stubTemplate, VARIANT_INDEX);

            Assert.IsTrue(mockFlag.Template == stubTemplate);
            Assert.IsTrue(mockFlag.VariantIndex == VARIANT_INDEX);
        }
        [TestMethod]
        public void SameTemplateAs_WithValidInputs_ReturnsCorrectResult()
        {
            var stubTemplate = new FakeFlagTemplate(variantCount: 1);
            const int VARIANT_INDEX = 0;
            var mockFlag = Flag.Create(stubTemplate, VARIANT_INDEX);

            bool result = mockFlag.SameTemplateAs(stubTemplate);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void SameAs_WithValidInputs_ReturnsCorrectResult()
        {
            var stubTemplate = new FakeFlagTemplate(variantCount: 1);
            const int VARIANT_INDEX = 0;
            var mockFlag1 = Flag.Create(stubTemplate, VARIANT_INDEX);
            var mockFlag2 = Flag.Create(stubTemplate, VARIANT_INDEX);

            bool result = mockFlag1.SameAs(mockFlag2);

            Assert.IsTrue(result);
        }
    }
}
