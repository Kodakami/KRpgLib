using KRpgLib.Utility.KomponentObject;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    public sealed class ModEffectCollection : KomponentObject<IModEffect>
    {
        public ModEffectCollection(IEnumerable<IModEffect> modEffects) : base(modEffects) { }
        public ModEffectCollection(ModEffectCollection other) : base(other) { }
        public ModEffectCollection(IEnumerable<ModEffectCollection> others) : base(others) { }
    }
}
