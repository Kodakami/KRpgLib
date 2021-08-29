using KRpgLib.Affixes;
using KRpgLib.Flags;

namespace KRpgLib.AffixesFlagsIntegration
{
    public sealed class FlagModEffect : IModEffect
    {
        public Flag Flag { get; }

        public FlagModEffect(Flag flag)
        {
            Flag = flag;
        }

        public static implicit operator Flag(FlagModEffect modEffect) => modEffect.Flag;
    }
}
