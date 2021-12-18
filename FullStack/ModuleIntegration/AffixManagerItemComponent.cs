using System;
using KRpgLib.Affixes;

namespace KRpgLib.Items
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
