using System;
using System.Collections.Generic;
using KRpgLib.Utility.KomponentObject;
using KRpgLib.Affixes.ModTemplates;
using System.Linq;
using System.Collections;

namespace KRpgLib.Affixes
{
    public abstract class ModCollection : IKomponent { }
    public sealed class ModCollection<TModTemplate, TRolledResult> : ModCollection, IEnumerable<Mod<TModTemplate, TRolledResult>>
        where TModTemplate : class, IModTemplate<TRolledResult>
    {
        private readonly IEnumerable<Mod<TModTemplate, TRolledResult>> _mods;
        public TRolledResult CombinedResult { get; }

        public ModCollection(IEnumerable<Mod<TModTemplate, TRolledResult>> mods, TRolledResult combinedResult)
        {
            var validMods = mods?.Where(m => m != null) ?? throw new ArgumentNullException(nameof(mods));
            CombinedResult = combinedResult;
        }

        public IEnumerator<Mod<TModTemplate, TRolledResult>> GetEnumerator()
        {
            return _mods.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_mods).GetEnumerator();
    }
}
