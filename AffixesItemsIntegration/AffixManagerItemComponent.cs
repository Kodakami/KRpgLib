using System;
using KRpgLib.Items;
using KRpgLib.Affixes;

namespace KRpgLib.AffixesItemsIntegration
{
    public sealed class AffixManagerItemComponent : IItemComponent
    {
        public AffixManager AffixManager { get; }

        public AffixManagerItemComponent(AffixManager affixManager)
        {
            AffixManager = affixManager ?? throw new ArgumentNullException(nameof(affixManager));
        }
    }
}
