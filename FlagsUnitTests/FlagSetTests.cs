using System;
using System.Collections.Generic;
using System.Text;
using KRpgLib.Flags;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KRpgLib.UnitTests.FlagsTests
{
    [TestClass]
    public class FlagSetTests
    {
        [TestMethod]
        public void HasFlag_WithTopLevelFlag_ReturnsTrue()
        {
            var stubFlag = Flag.Create(FakeFlagProvider.FakeFlagTemplate_VariantCount1, 0);
            var stubProvider = new FakeFlagProvider(stubFlag);
            var mockmanager = new FlagManager();
            mockmanager.AddFlagProvider(stubProvider);

            bool result = mockmanager.GetFlagCollection().HasFlag(stubFlag);

            Assert.IsTrue(result);
        }
        [TestMethod]
        public void HasFlag_WithImpliedFlag_ReturnsTrue()
        {
            var stubTemplate = new FakeFlagTemplate_WithImplied();
            var stubFlag = Flag.Create(stubTemplate, 0);
            var stubProvider = new FakeFlagProvider(stubFlag);
            var mockSet = new FlagManager();
            mockSet.AddFlagProvider(stubProvider);

            bool result = mockSet.GetFlagCollection().HasFlag(FakeFlagTemplate_WithImplied.ImpliedFlagTemplate, 0);

            Assert.IsTrue(result);
        }
    }
}
