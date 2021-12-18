using KRpgLib.Flags;

namespace KRpgLib.Affixes
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
