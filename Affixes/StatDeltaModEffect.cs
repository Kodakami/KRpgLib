using KRpgLib.Stats;

namespace KRpgLib.Affixes
{
    public sealed class StatDeltaModEffect : IModEffect
    {
        public DeltaCollection StatDeltaCollection { get; }

        public StatDeltaModEffect(DeltaCollection statDeltaCollection)
        {
            StatDeltaCollection = statDeltaCollection;
        }

        public static implicit operator DeltaCollection(StatDeltaModEffect modEffect) => modEffect.StatDeltaCollection;
    }
}
