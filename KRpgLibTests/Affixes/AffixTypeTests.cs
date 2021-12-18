using Microsoft.VisualStudio.TestTools.UnitTesting;
using KRpgLib.Affixes;
using KRpgLib.Affixes.AffixTypes;
using System;

namespace KRpgLibTests.Affixes
{
    [TestClass]
    public static class AffixTypeTests
    {
        public sealed class FakeAffixManager : AffixManager { }

        public static readonly AffixManager StubBaseManager = new AffixManager();
        public static readonly AffixManager StubWeakTypedManager = new FakeAffixManager();
        public static readonly FakeAffixManager StubStrongTypedManager = new FakeAffixManager();

        [TestClass]
        public class AdHoc_BaseType
        {
            [TestMethod]
            public void Ctor_WithNullArg_DoesNotThrowEx()
            {
                AffixType_AdHoc.CanBeAppliedPredicate stubNullPredicate = null;

                var mockType = new AffixType_AdHoc(stubNullPredicate);

                Assert.IsNotNull(mockType);
            }
        }
        [TestClass]
        public class AdHoc_ConstrainedType
        {
            [TestMethod]
            public void Ctor_WithNullArg_DoesNotThrowEx()
            {
                AffixType_AdHoc.CanBeAppliedPredicate stubNullPredicate = null;

                var mockType = new AffixType_AdHoc(stubNullPredicate);

                Assert.IsNotNull(mockType);
            }
            [TestMethod]
            public void AffixCanBeApplied_WithBaseManager_ThrowsArgEx()
            {
                var mockType = new AffixType_AdHoc<FakeAffixManager>();

                void exceptionalAction() => mockType.AffixCanBeApplied(StubBaseManager);

                Assert.ThrowsException<ArgumentException>(exceptionalAction);
            }
            [TestMethod]
            public void AffixCanBeApplied_WithWeakTypedManager_DoesNotThrowArgEx()
            {
                var mockType = new AffixType_AdHoc<FakeAffixManager>();

                bool arbitraryValue = mockType.AffixCanBeApplied(StubWeakTypedManager);

                Assert.IsTrue(arbitraryValue);
            }
            [TestMethod]
            public void AffixCanBeApplied_WithStrongTypedManager_DoesNotThrowArgEx()
            {
                var mockType = new AffixType_AdHoc<FakeAffixManager>();

                bool arbitraryValue = mockType.AffixCanBeApplied(StubStrongTypedManager);

                Assert.IsTrue(arbitraryValue);
            }
        }
    }
}
