using System;
using System.Collections.Generic;
using KRpgLib.Flags;

namespace KRpgLib.Affixes
{
    public sealed class FlagModTemplate : ModTemplate<Flag>
    {
        // We don't use a FlagCollection because the functionality of implied flags is of no use here.
        public IReadOnlyList<Flag> Flags { get; }

        public FlagModTemplate(IReadOnlyList<Flag> flags)
        {
            Flags = flags ?? throw new ArgumentNullException(nameof(flags));
        }

        public override IModEffect GetModEffect(Mod<Flag> modInstance)
        {
            return new FlagModEffect(modInstance.Arg);
        }

        public override Flag GetNewArg(Random rng)
        {
            return Flags[rng.Next(0, Flags.Count)];
        }
    }
}
