using KRpgLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using KRpgLib.Affixes.AffixTypes;
using KRpgLib.Stats;
using KRpgLib.Flags;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A collector and manipulator of Affixes considered "applied" to something moddable.
    /// </summary>
    public class AffixManager : IDynamicStatProvider, IDynamicFlagProvider
    {
        private readonly List<Affix> _appliedAffixes;

        private readonly AffixApplicationHelper _affixApplicationHelper;
        private readonly ModEffectCacheHelper _effectCacheHelper;

        // Invoked by SetDirty().
        public event Action OnStatsChanged;
        public event Action OnFlagsChanged;

        public AffixManager()
        {
            _appliedAffixes = new List<Affix>();
            _affixApplicationHelper = new AffixApplicationHelper(this);
            _effectCacheHelper = new ModEffectCacheHelper(this);
        }

        public ModEffectCollection GetAllModEffects() => _effectCacheHelper.GetCacheCopy();

        public bool RerollAllAffixValues(Random rng)
        {
            return RerollAffixValues_Internal(_appliedAffixes, rng);
        }
        public bool RerollAffixValues(Predicate<Affix> predicate, Random rng)
        {
            return RerollAffixValues_Internal(FindAffixes(predicate ?? throw new ArgumentNullException(nameof(predicate))), rng);
        }
        public bool RerollAffixValues(AffixType withAffixType, Random rng)
        {
            return RerollAffixValues(a => a.Template.AffixType == (withAffixType ?? throw new ArgumentNullException(nameof(withAffixType))), rng);
        }
        private bool RerollAffixValues_Internal(IEnumerable<Affix> affixes, Random rng)
        {
            bool changeOccurred = false;

            foreach (var affix in affixes)
            {
                bool changed = affix.RerollAllMods(rng);

                changeOccurred = changeOccurred || changed;
            }

            if (changeOccurred)
            {
                // Set this effect collection dirty.
                SetDirty();
            }

            return changeOccurred;
        }

        public void AddAffix_NoMessage(Affix preRolledAffix)
        {
            if (CheckAffixCanBeApplied_NoMessage(preRolledAffix ?? throw new ArgumentNullException(nameof(preRolledAffix))))
            {
                AddAffix_Internal(preRolledAffix);
            }
        }
        public AffixApplicationStatus AddAffix(Affix preRolledAffix)
        {
            var result = CheckAffixCanBeApplied(preRolledAffix ?? throw new ArgumentNullException(nameof(preRolledAffix)));
            if (result.Type == AffixApplicationStatusType.SUCCESS)
            {
                AddAffix_Internal(preRolledAffix);
            }
            return result;
        }
        public void AddAffix_NoMessage(AffixTemplate affixToRoll, Random rng)
        {
            if (affixToRoll == null)
            {
                throw new ArgumentNullException(nameof(affixToRoll));
            }

            AddAffix_NoMessage(affixToRoll.CreateNewAffixInstance(rng));
        }
        public AffixApplicationStatus AddAffix(AffixTemplate affixToRoll, Random rng)
        {
            if (affixToRoll == null)
            {
                throw new ArgumentNullException(nameof(affixToRoll));
            }

            return AddAffix(affixToRoll.CreateNewAffixInstance(rng));
        }

        public void AddAffixes_NoMessages(IEnumerable<Affix> preRolledAffixes)
        {
            if (preRolledAffixes == null)
            {
                throw new ArgumentNullException(nameof(preRolledAffixes));
            }

            // Add the ones that are allowed.
            AddAffixes_Internal(preRolledAffixes.Where(a => CheckAffixCanBeApplied_NoMessage(a)));
        }
        public IReadOnlyList<AffixApplicationStatus> AddAffixes(IEnumerable<Affix> preRolledAffixes)
        {
            if (preRolledAffixes == null)
            {
                throw new ArgumentNullException(nameof(preRolledAffixes));
            }

            // Collect the list of results.
            var results = new List<AffixApplicationStatus>();
            foreach (var affix in preRolledAffixes)
            {
                results.Add(AddAffix(affix));
            }
            return results;
        }

        public void RemoveAffix(AffixTemplate affixTemplate)
        {
            var foundAffix = FindAffix(affixTemplate);

            if (foundAffix != null)
            {
                RemoveAffix_Internal(foundAffix);
            }
        }
        public void RemoveAffixes(IEnumerable<AffixTemplate> affixTemplates)
        {
            var affixesWithGivenTemplates = FindAffixes(a => affixTemplates.Contains(a.Template));

            RemoveAffixes_Internal(new List<Affix>(affixesWithGivenTemplates));
        }
        public void RemoveAllAffixes()
        {
            ClearAffixes_Internal();
        }
        public bool HasAffixWhere(Predicate<Affix> predicate)
        {
            return _appliedAffixes.Exists(predicate ?? throw new ArgumentNullException(nameof(predicate)));
        }
        public bool HasAffix(AffixTemplate withTemplate)
        {
            return HasAffixWhere(a => a.Template == (withTemplate ?? throw new ArgumentNullException(nameof(withTemplate))));
        }
        public bool HasAffixes(Predicate<Affix> predicate, int count)
        {
            if (count <= 0 || count > _appliedAffixes.Count)
            {
                return false;
            }

            return Count(predicate) >= count;
        }
        public int Count(Predicate<Affix> predicate)
        {
            return _appliedAffixes.Count(a => predicate(a));
        }
        public int Count(AffixType affixType) => Count(a => a.Template.AffixType == affixType);
        public int Count() => _appliedAffixes.Count;

        protected Affix FindAffix(AffixTemplate withTemplate) => _appliedAffixes.Find(a => a.Template == withTemplate);
        protected IEnumerable<Affix> FindAffixes(Predicate<Affix> predicate)
        {
            return _appliedAffixes.FindAll(predicate);
        }
        protected IEnumerable<Affix> FindAffixes(AffixType withAffixType)
        {
            return FindAffixes(a => a.Template.AffixType == withAffixType);
        }
        // TODO: Replace with helper class that manages return codes (reasons why it's not allowed).
        protected AffixApplicationStatus CheckAffixCanBeApplied(Affix affix)
        {
            return _affixApplicationHelper.CheckAffixCanBeApplied(affix);
        }
        protected bool CheckAffixCanBeApplied_NoMessage(Affix affix)
        {
            return _affixApplicationHelper.CheckAffixCanBeApplied_NoMessage(affix);
        }
        protected virtual (bool CanBeApplied, string FailureMessage) AffixCanBeApplied(Affix affix) => (true, null);

        protected void AddAffix_NoQuestionsAsked(Affix affix)
        {
            AddAffix_Internal(affix);
        }
        protected void AddAffixes_NoQuestionsAsked(IEnumerable<Affix> affixes)
        {
            AddAffixes_Internal(affixes);
        }

        private void AddAffix_Internal(Affix affix)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Add(affix);

            // Set this effect collection dirty.
            SetDirty();
        }
        private void AddAffixes_Internal(IEnumerable<Affix> affixes)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.AddRange(affixes);

            // Set this effect collection dirty.
            SetDirty();
        }
        private void RemoveAffix_Internal(Affix affix)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Remove(affix);

            // Set this effect collection dirty.
            SetDirty();
        }
        private void RemoveAffixes_Internal(IEnumerable<Affix> affixes)
        {
            // It may be useful to raise an event here.

            foreach (var affix in new List<Affix>(affixes))
            {
                _appliedAffixes.Remove(affix);
            }

            // Set this effect collection dirty.
            SetDirty();
        }
        private void ClearAffixes_Internal()
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Clear();

            // Set this effect collection dirty.
            SetDirty();
        }
        // Sets the ModEffectCacheHelper dirty and invokes the OnStatsChanged and OnFlagsChanged events.
        private void SetDirty()
        {
            _effectCacheHelper.SetDirty_FromExternal();
            OnStatsChanged.Invoke();
            OnFlagsChanged.Invoke();
        }

        public virtual DeltaCollection GetDeltaCollection()
        {
            // Implicit conversion on StatDeltaModEffect.
            return new DeltaCollection(_effectCacheHelper.GetCacheCopy().GetModEffects<StatDeltaModEffect>().Cast<DeltaCollection>());
        }
        public IReadOnlyFlagCollection GetFlagCollection()
        {
            // Implicit conversion on FlagModEffect.
            return new FlagCollection(_effectCacheHelper.GetCacheCopy().GetModEffects<FlagModEffect>().Cast<FlagCollection>());
        }

        //Flags

        private class AffixApplicationHelper
        {
            private readonly AffixManager _parent;
            public AffixApplicationHelper(AffixManager parent)
            {
                // No null checks in private code. Just don't be wrong.
                _parent = parent;
            }
            public AffixApplicationStatus CheckAffixCanBeApplied(Affix affix)
            {
                // Check if affix template already has an instance here.
                if (_parent.HasAffix(affix.Template))
                {
                    return new AffixApplicationStatus(AffixApplicationStatusType.TEMPLATE_EXISTS, "Affix manager already has an affix with the same template.");
                }

                // Check for restrictions on affix type (usually max quantity).
                var affixType = affix.Template.AffixType;
                if (!affixType.AffixCanBeApplied(_parent))
                {
                    return new AffixApplicationStatus(AffixApplicationStatusType.AFFIX_TYPE_RESTRICTION, "The Affix can't be applied due to a restriction on the Affix type.");
                }

                // Custom limitations.
                var (CanBeApplied, FailureMessage) = _parent.AffixCanBeApplied(affix);
                if (!CanBeApplied)
                {
                    return new AffixApplicationStatus(AffixApplicationStatusType.OTHER, FailureMessage ?? "Unspecified limitation.");
                }

                return new AffixApplicationStatus(AffixApplicationStatusType.SUCCESS, "Affix can be applied.");
            }
            public bool CheckAffixCanBeApplied_NoMessage(Affix affix)
            {
                // Check if affix template already has an instance here.
                if (_parent.HasAffix(affix.Template))
                {
                    return false;
                }

                // Check for restrictions on affix type (usually max quantity).
                var affixType = affix.Template.AffixType;
                if (!affixType.AffixCanBeApplied(_parent))
                {
                    return false;
                }

                // Custom limitations.
                var (CanBeApplied, _) = _parent.AffixCanBeApplied(affix);
                return CanBeApplied;
            }
        }
        private class ModEffectCacheHelper : CachedValueController<ModEffectCollection, AffixManager>
        {
            public ModEffectCacheHelper(AffixManager context) : base(context) { }

            protected override ModEffectCollection CalculateNewCache()
            {
                // Get all mod effect collections from all affixes. Will use cached values for affixes which haven't changed.
                return new ModEffectCollection(Context._appliedAffixes.Select(a => a.GetAllModEffects()));
            }

            protected override ModEffectCollection CreateCacheCopy(ModEffectCollection cache)
            {
                // ModEffectCollection is sealed and not modifiable. Safe to pass by reference.
                return cache;
            }
        }
    }
    public struct AffixApplicationStatus
    {
        public readonly AffixApplicationStatusType Type;
        public readonly string Message;

        public AffixApplicationStatus(AffixApplicationStatusType type, string message = null)
        {
            Type = type;
            Message = message;
        }
    }
    public enum AffixApplicationStatusType
    {
        SUCCESS = 0,
        OTHER = 1,
        TEMPLATE_EXISTS = 2,
        AFFIX_TYPE_RESTRICTION = 3,
    }
}
