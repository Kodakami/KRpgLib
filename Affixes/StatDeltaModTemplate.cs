using KRpgLib.Stats;
using System;
using System.Linq;
using System.Collections.Generic;
using KRpgLib.Utility.Serialization;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A mod template for mods which provide stat deltas.
    /// </summary>
    public sealed class StatDeltaModTemplate: ModTemplate<IReadOnlyList<int>>
    {
        // List of info to create stat deltas within a range.
        // Example list might contain:
        //      (MinWeaponDmgPhysicalLocal, Additional, (min: 3, max: 7, prec: 1))
        //      (MaxWeaponDmgPhysicalLocal, Additional, (min: 12, max: 20, prec: 1))
        // In PoE would look like:
        //      [Explicit Prefix "Sharp"]
        //      Adds (3-7) to (12-20) Physical Damage;

        // This class uses an arg collection (instead of just one) specifically for mods that act as one like in the above example. It is not intended to be used for multiple mods that are part of one Affix. As a counter-example:
        //      (MaxLife, Multiply, (min: 3, max: 5, prec: 1))
        //      (MaxMana, SuperMultiply, (min: 10, max: 12, prec: 1))
        // In PoE would look like (one mod):
        //      [Explicit Prefix "Perfected"]
        //      (3-5)% increased Maximum Life
        //      (10-12)% more Maximum Mana
        // For this type of functionality, you should make multiple mods and put them all on one Affix.

        /// <summary>
        /// The bounds which govern the possible argument values of the mod. Basically the minimum, maximum, and precision values of the roll.
        /// </summary>
        public IReadOnlyList<StatDeltaValueBounds> ArgBounds { get; }

        public StatDeltaModTemplate(IReadOnlyList<StatDeltaValueBounds> argBounds)
            : base(new ModArgType<IReadOnlyList<int>>(new UniformListSerializer<int>(Int32Serializer.Singleton)))
        {
            ArgBounds = new List<StatDeltaValueBounds>(argBounds ?? throw new ArgumentNullException(nameof(argBounds)));
        }

        // This ctor casts and passes to the other.
        public StatDeltaModTemplate(params StatDeltaValueBounds[] argBounds)
            : this((IReadOnlyList<StatDeltaValueBounds>)argBounds) { }

        protected override IReadOnlyList<int> GetNewArgStrongValue(Random rng)
        {
            return ArgBounds.Select(ab => GenerateRandomValueWithinBounds(rng, ab)).ToArray();
        }
        private int GenerateRandomValueWithinBounds(Random rng, StatDeltaValueBounds bounds)
        {
            // System.Random uses exclusive upper bound.
            int raw = rng.Next(bounds.MinRollValue, bounds.MaxRollValue + 1);

            return StatUtilities.LegalizeIntValue(raw, bounds.MinRollValue, bounds.MaxRollValue, bounds.RollPrecision);
        }

        protected override IModEffect GetModEffect_Internal(Mod modInstance)
        {
            var modArgValues = (IReadOnlyList<int>)modInstance.ArgValue;

            var deltaList = new List<StatDelta>();
            for (int i = 0; i < ArgBounds.Count; i++)
            {
                var thisSubArg = ArgBounds[i];
                var thisStat = thisSubArg.Stat;
                var thisDeltaType = thisSubArg.DeltaType;
                var thisValue = modArgValues[i];

                deltaList.Add(new StatDelta(thisStat, new Delta(thisDeltaType, thisValue)));
            }

            return new StatDeltaModEffect(new DeltaCollection(deltaList));
        }
    }
}
