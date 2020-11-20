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
            void exceptionalAction() => new FakeCompoundStatTemplate(0, 0, 0, null);

            Assert.ThrowsException<ArgumentNullException>(exceptionalAction);
        }
    }
}
