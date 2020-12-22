using KRpgLib.Flags;
using KRpgLib.Affixes.ModTemplates;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    public class AffixTemplate<TValue> where TValue : struct
    {
        private readonly List<AbstractMTFlag> _flagMods;
        private readonly List<MTStatDelta<TValue>> _statDeltaMods;

        public string ExternalName { get; }
        public AffixType AffixType { get; }

        public AffixTemplate(string externalName, AffixType affixType, List<AbstractMTFlag> flagMods, List<MTStatDelta<TValue>> statDeltaMods)
        {
            ExternalName = externalName ?? throw new ArgumentNullException(nameof(externalName));
            AffixType = affixType ?? throw new ArgumentNullException(nameof(affixType));
            _flagMods = flagMods ?? throw new ArgumentNullException(nameof(flagMods));
            _statDeltaMods = statDeltaMods ?? throw new ArgumentNullException(nameof(statDeltaMods));
        }

        public Affix<TValue> GetNewRolledAffix()
        {
            // Null check in later code.

            var flags = GetNewRolledFlags();
            var statDeltas = GetNewRolledStatDeltas();

            return Affix<TValue>.Create(this, flags, statDeltas);
        }
        protected List<ModFlag> GetNewRolledFlags()
        {
            return _flagMods.ConvertAll(mt => new ModFlag(mt));
        }
        protected List<ModStatDelta<TValue>> GetNewRolledStatDeltas()
        {
            return _statDeltaMods.ConvertAll(mt => new ModStatDelta<TValue>(mt));
        }
    }
}
