using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace KRpgLibUnitTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatTemplateTests
    {
        [TestMethod]
        public void CompoundStatTemplate_WhenConstructingWithNullAlgorithm_ThrowsArgNullRef()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new TestCompoundStat("Error Compound Stat", 0, 100, 1, null));
        }
    }
}
