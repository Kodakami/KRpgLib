using KRpgLib.Utility;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes.AffixTypes
{
    /// <summary>
    /// A type (or slot) for an affix. Helps determine which affixes are valid in certain cases. Think "Prefix mod" and "Suffix mod" in a standard ARPG.
    /// </summary>
    public abstract class AffixType
    {
        public abstract bool AffixCanBeApplied(AffixManager toManager);
    }
    public abstract class AffixType<TManager> : AffixType
        where TManager : AffixManager
    {
        public override bool AffixCanBeApplied(AffixManager toManager)
        {
            var argType = toManager.GetType();
            var targetType = typeof(TManager);
            if (argType != targetType && !argType.IsSubclassOf(targetType))
            {
                throw new ArgumentException($"Argument must be an instance of type {targetType} or a subclass.", nameof(toManager));
            }

            var castManager = (TManager)toManager;
            return AffixCanBeApplied(castManager);
        }
        public abstract bool AffixCanBeApplied(TManager toManager);
    }
    public sealed class AffixType_AdHoc : AffixType
    {
        // Delegate.
        public delegate bool CanBeAppliedPredicate(AffixManager manager);

        // Static predicate instance for when an Affix has no specific restrictions.
        private static readonly CanBeAppliedPredicate _noRestrictions = new CanBeAppliedPredicate(_ => true);

        // Instance member.
        private readonly CanBeAppliedPredicate _predicate;

        public AffixType_AdHoc(CanBeAppliedPredicate predicateOrNull = null)
        {
            _predicate = predicateOrNull ?? _noRestrictions;
        }
        public override bool AffixCanBeApplied(AffixManager toManager) => _predicate.Invoke(toManager ?? throw new ArgumentNullException(nameof(toManager)));
    }
    public sealed class AffixType_AdHoc<TManager> : AffixType<TManager> where TManager : AffixManager
    {
        // Delegate.
        public delegate bool CanBeAppliedPredicate(TManager manager);

        // Static predicate instance for when an Affix has no specific restrictions.
        private static readonly CanBeAppliedPredicate _noRestrictions = new CanBeAppliedPredicate(_ => true);

        // Instance member.
        private readonly CanBeAppliedPredicate _predicate;

        public AffixType_AdHoc(CanBeAppliedPredicate predicateOrNull = null)
        {
            _predicate = predicateOrNull ?? _noRestrictions;
        }
        public override bool AffixCanBeApplied(TManager toManager) => _predicate.Invoke(toManager ?? throw new ArgumentNullException(nameof(toManager)));
    }
}
