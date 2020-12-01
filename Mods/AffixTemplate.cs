using KRpgLib.Flags;
using KRpgLib.Mods.ModTemplates;
using System;
using System.Collections.Generic;

namespace KRpgLib.Mods
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

        public Affix<TValue> GetNewRolledAffix(Random rng)
        {
            var flags = GetNewRolledFlags(rng);
            var statDeltas = GetNewRolledStatDeltas(rng);

            return new Affix<TValue>(this, flags, statDeltas);
        }
        protected List<ModFlag> GetNewRolledFlags(Random rng)
        {
            return _flagMods.ConvertAll(mt => new ModFlag(mt, rng));
        }
        protected List<ModStatDelta<TValue>> GetNewRolledStatDeltas(Random rng)
        {
            return _statDeltaMods.ConvertAll(mt => new ModStatDelta<TValue>(mt, rng));
        }
    }
}
