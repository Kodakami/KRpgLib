using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Affixes
{
    public class AffixManager<TValue> : IFlagProvider, IStatProvider<TValue> where TValue : struct
    {
        // TODO: Could make a subsystem to govern this list. Would help with maintaining restricted access to the values, as well as quick and easy serialization.
        private readonly List<Affix<TValue>> _appliedAffixes;

        private readonly StatDeltaCacheHelper _statDeltaCache;
        private readonly FlagCacheHelper _flagCache;

        public AffixManager()
        {
            _appliedAffixes = new List<Affix<TValue>>();
            _statDeltaCache = new StatDeltaCacheHelper(this);
            _flagCache = new FlagCacheHelper(this);
        }
        public void RerollAllAffixValues()
        {
            // Collect each template.
            var templates = _appliedAffixes.ConvertAll(a => a.Template);

            // Remove all affixes.
            ClearAffixes_Internal();

            // Reapply each affix from template with new rolls.
            foreach (var affix in templates.ConvertAll(t => t.GetNewRolledAffix()))
            {
                AddAffix_Internal(affix);
            }

            // Set dirty.
            SetDirty(_appliedAffixes);
        }
        public void RerollAffixValuesWhere(Predicate<Affix<TValue>> predicate)
        {
            if (predicate == null)
            {
                return;
            }

            var affixesWhere = FindAffixesWhere(predicate);
            if (affixesWhere.Count > 0)
            {
                var newAffixes = new List<Affix<TValue>>();
                foreach (var oldAffix in affixesWhere)
                {
                    var newAffix = oldAffix.Template.GetNewRolledAffix();
                    newAffixes.Add(newAffix);   //For setting dirty.

                    RemoveAffix_Internal(oldAffix);

                    AddAffix_Internal(newAffix);
                }

                SetDirty(newAffixes);
            }
        }
        public void RerollAllAffixValues(AffixType withAffixType) => RerollAffixValuesWhere(a => a.Template.AffixType == withAffixType);

        public void AddAffix(Affix<TValue> preRolledAffix)
        {
            if (CheckAffixIsAllowed(preRolledAffix))
            {
                AddAffix_Internal(preRolledAffix);

                SetDirty(preRolledAffix);
            }
        }
        public void AddAffix(AffixTemplate<TValue> affixToRoll)
        {
            if (affixToRoll == null)
            {
                return;
            }

            AddAffix(affixToRoll.GetNewRolledAffix());
        }

        public void AddAffixes(IEnumerable<Affix<TValue>> preRolledAffixes)
        {
            if (preRolledAffixes == null)
            {
                return;
            }

            var appliedAffixes = new List<Affix<TValue>>();
            foreach (var affix in preRolledAffixes)
            {
                if (CheckAffixIsAllowed(affix))
                {
                    AddAffix_Internal(affix);
                    appliedAffixes.Add(affix);
                }
            }

            // If this is empty, nothing will be set dirty.
            SetDirty(appliedAffixes);
        }

        public enum RemovalMethod
        {
            JUST_ONE = 0,
            ALL_INSTANCES = 1,
        }
        public void RemoveAffix(AffixTemplate<TValue> affixTemplate, RemovalMethod removalMethod = RemovalMethod.ALL_INSTANCES)
        {
            if (removalMethod == RemovalMethod.JUST_ONE)
            {
                if (_appliedAffixes.Exists(a => a.Template == affixTemplate))
                {
                    var found = _appliedAffixes.Find(a => a.Template == affixTemplate);
                    RemoveAffix_Internal(found);
                    SetDirty(found);
                }
            }
            // ALL_INSTANCES.
            else
            {
                var found = FindAffixesWhere(a => a.Template == affixTemplate);
                if (found.Count > 0)
                {
                    foreach (var foundAffix in found)
                    {
                        RemoveAffix_Internal(foundAffix);
                    }

                    SetDirty(found);
                }
            }
        }
        public void RemoveAffixes(IEnumerable<AffixTemplate<TValue>> affixTemplates, RemovalMethod removalMethod = RemovalMethod.ALL_INSTANCES)
        {
            foreach (var t in affixTemplates)
            {
                RemoveAffix(t, removalMethod);
            }
        }
        public void RemoveAllAffixes()
        {
            var allAffixes = new List<Affix<TValue>>(_appliedAffixes);
            foreach (var removedAffix in allAffixes)
            {
                // In case events are raised on each affix removed.
                RemoveAffix(removedAffix.Template, RemovalMethod.ALL_INSTANCES);
            }

            SetDirty(allAffixes);
        }
        public bool HasAffixWhere(Predicate<Affix<TValue>> predicate)
        {
            return _appliedAffixes.Exists(predicate);
        }
        public bool HasAffix(AffixTemplate<TValue> withTemplate)
        {
            if (withTemplate == null)
            {
                return false;
            }
            return HasAffixWhere(a => a.Template == withTemplate);
        }
        public bool HasAffixesWhere(int count, Predicate<Affix<TValue>> predicate)
        {
            if (count <= 0 || count > _appliedAffixes.Count)
            {
                return false;
            }
            int found = 0;
            foreach (var affix in _appliedAffixes)
            {
                if (predicate(affix))
                {
                    found++;
                }
                // Short-circuit.
                if (found == count)
                {
                    return true;
                }
            }
            return false;
        }
        public int CountAffixesWhere(Predicate<Affix<TValue>> predicate)
        {
            return FindAffixesWhere(predicate).Count;
        }
        public virtual string GetModifiedObjectName(string objectName)
        {
            foreach (var type in AffixType.GetAllByPriority())
            {
                foreach (var affixOfType in FindAffixes(type))
                {
                    objectName = type.ApplyName(affixOfType.ExternalName, objectName);
                }
            }
            return objectName;
        }

        protected List<Affix<TValue>> FindAffixesWhere(Predicate<Affix<TValue>> predicate)
        {
            return _appliedAffixes.FindAll(predicate);
        }
        protected List<Affix<TValue>> FindAffixes(AffixType withAffixType)
        {
            return FindAffixesWhere(a => a.Template.AffixType == withAffixType);
        }
        // TODO: Replace with helper class that manages return codes (reasons why it's not allowed).
        protected bool CheckAffixIsAllowed(Affix<TValue> affix)
        {
            // Check affixType limitations.

            // Check for max affixes of type.
            var affixType = affix.Template.AffixType;
            if (_appliedAffixes.FindAll(a => a.Template.AffixType == affixType).Count >= affixType.MaxAffixesOfType)
            {
                return false;
            }

            // Other limitations here.

            return true;
        }
        protected virtual void LateUpdateCache() { }
        protected void AddAffix_NoQuestionsAsked(Affix<TValue> affix)
        {
            AddAffix_Internal(affix);

            SetDirty(affix);
        }
        protected void AddAffixes_NoQuestionsAsked(IEnumerable<Affix<TValue>> affixes)
        {
            foreach (var affix in affixes)
            {
                AddAffix_Internal(affix);
            }

            SetDirty(affixes);
        }

        private void AddAffix_Internal(Affix<TValue> affix)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Add(affix);
        }
        private void RemoveAffix_Internal(Affix<TValue> affix)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Remove(affix);
        }
        private void ClearAffixes_Internal()
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Clear();
        }

        protected void SetDirty(bool flags, bool statDeltas)
        {
            if (flags)
            {
                _flagCache.SetDirty_FromExternal();
            }
            if (statDeltas)
            {
                _statDeltaCache.SetDirty_FromExternal();
            }
        }
        private void SetDirty(Affix<TValue> triggeringAffix)
        {
            SetDirty(triggeringAffix.HasFlagMods, triggeringAffix.HasStatDeltaMods);
        }
        private void SetDirty(IEnumerable<Affix<TValue>> triggeringAffixes)
        {
            // Define whether to set each dirty.
            bool flags = false;
            bool statDeltas = false;

            // For each triggering affix.
            foreach (var affix in triggeringAffixes)
            {
                // If the flags are already being set dirty OR the affix has flag mods, set the flags dirty.
                flags = flags || affix.HasFlagMods;
                // If the stat deltas are already being set dirty OR the affix has stat delta mods, set the stat deltas dirty.
                statDeltas = statDeltas || affix.HasStatDeltaMods;

                // If both caches are being set dirty, then short-circuit (nothing else can change, stop iterating the collection).
                if (flags && statDeltas)
                {
                    break;
                }
            }

            SetDirty(flags, statDeltas);
        }

        public List<Flag> GetAllFlags() => _flagCache.GetCacheCopy();
        public StatDeltaCollection<TValue> GetStatDeltaCollection() => _statDeltaCache.GetCacheCopy();

        private sealed class StatDeltaCacheHelper : ParentedCachedValueController<StatDeltaCollection<TValue>, AffixManager<TValue>>
        {
            public StatDeltaCacheHelper(AffixManager<TValue> parent) : base(parent) { }
            protected override StatDeltaCollection<TValue> CalculateNewCache()
            {
                // Get the stat delta collections from all affixes that have stat delta mods and make them into a new delta collection.
                return new StatDeltaCollection<TValue>(Parent._appliedAffixes.FindAll(a => a.HasStatDeltaMods).ConvertAll(a => a.GetStatDeltaCollection()));
            }
            protected override StatDeltaCollection<TValue> CreateCacheCopy(StatDeltaCollection<TValue> cache)
            {
                // Read-only collection is safe to redistribute.
                return cache;
            }
        }
        private sealed class FlagCacheHelper : ParentedCachedValueController<List<Flag>, AffixManager<TValue>>
        {
            public FlagCacheHelper(AffixManager<TValue> parent) : base(parent) { }
            protected override List<Flag> CalculateNewCache()
            {
                // Paring down duplicate or conflicting flags is not the job of the affix manager.
                return Parent._appliedAffixes.Where(a => a.HasFlagMods).SelectMany(a => a.GetAllFlags()).ToList();
            }

            protected override List<Flag> CreateCacheCopy(List<Flag> cache)
            {
                return new List<Flag>(cache);
            }
        }
    }
}
