using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KRpgLibTests.Stats.Compound
{
    [TestClass]
    public class CompoundStatTests
    {
        [TestMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1806:Do not ignore method results", Justification = "Unit Test")]
        public void Ctor_WithNullAlgorithm_ThrowsArgNullEx()
        {
            static void exceptionalAction() => new FakeCompoundStat(0, 0, 0, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
