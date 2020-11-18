using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatTemplateTests
    {
        [TestMethod]
        public void CompoundStatTemplate_WhenConstructingWithNullAlgorithm_ThrowsArgNullRef()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestCompoundStatTemplate("Error Compound Stat", 0, 100, 1, null));
        }
    }
}
