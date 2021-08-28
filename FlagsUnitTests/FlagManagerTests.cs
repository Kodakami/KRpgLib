using System;
using KRpgLib.Flags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLib.UnitTests.FlagsTests
{
    [TestClass]
    public class FlagManagerTests
    {
        [TestMethod]
        public void AddFlagProvider_WithNullProvider_ThrowsArgNullEx()
        {
            var mockManager = new FlagManager();

            void exceptionalAction() => mockManager.AddFlagProvider(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void RemoveFlagProvider_WithNullProvider_ThrowsArgNullEx()
        {
            var mockManager = new FlagManager();

            void exceptionalAction() => mockManager.RemoveFlagProvider(null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
        [TestMethod]
        public void HasFlagTemplate_AfterProviderRemoved_ReturnsCorrectResult()
        {
            var stubTemplate = FakeFlagProvider.FakeFlagTemplate_VariantCount1;
            var stubProvider = new FakeFlagProvider(Flag.Create(stubTemplate));
            var mockManager = new FlagManager();
            mockManager.AddFlagProvider(stubProvider);

            Assert.IsTrue(mockManager.GetFlagCollection().HasFlagTemplate(stubTemplate));

            mockManager.RemoveFlagProvider(stubProvider);

            Assert.IsFalse(mockManager.GetFlagCollection().HasFlagTemplate(stubTemplate));
        }
        [TestMethod]
        public void HasFlagTemplate_AfterDynamicProviderUpdate_ReturnsCorrectResult()
        {
            var stubDynamicProvider = new FakeFlagProvider_Dynamic();
            var mockManager = new FlagManager();
            mockManager.AddFlagProvider(stubDynamicProvider);

            IFlagTemplate expectedTemplateBeforeUpdate = FakeFlagProvider_Dynamic.FlagTemplateProvided_InFalseState;
            IFlagTemplate expectedTemplateAfterUpdate = FakeFlagProvider_Dynamic.FlagTemplateProvided_InTrueState;

            Assert.IsTrue(mockManager.GetFlagCollection().HasFlagTemplate(expectedTemplateBeforeUpdate));

            stubDynamicProvider.ToggleState();

            Assert.IsTrue(mockManager.GetFlagCollection().HasFlagTemplate(expectedTemplateAfterUpdate));
        }
    }
}
