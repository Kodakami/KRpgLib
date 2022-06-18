using System;
using System.Collections.Generic;
using KRpgLib.Flags;
using KRpgLib.Utility.Serialization;

namespace KRpgLib.Affixes
{
    public sealed class FlagModTemplate : ModTemplate<Flag>
    {
        // We don't use a FlagCollection because the functionality of implied flags is of no use here.
        public IReadOnlyList<Flag> Flags { get; }

        public FlagModTemplate(IReadOnlyList<Flag> flags)
            : base(new ModArgType<Flag>(FlagSerializer.Singleton))
        {
            Flags = flags ?? throw new ArgumentNullException(nameof(flags));
        }

        // This ctor casts and passes to the other.
        public FlagModTemplate(params Flag[] flags)
            : this((IReadOnlyList<Flag>)flags) { }

        protected override Flag GetNewArgStrongValue(Random rng)
        {
            return Flags[rng.Next(0, Flags.Count)];
        }

        protected override IModEffect GetModEffect_Internal(Mod safeModInstance)
        {
            return new FlagModEffect((Flag)safeModInstance.ArgValue);
        }
    }
}
