using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Affixes
{
    public class AffixManager
    {
        private readonly AffixCollectionHelper _affixCollectionHelper;
        public AffixManager()
        {
            _affixCollectionHelper = new AffixCollectionHelper();
        }
        public void RerollAllAffixValues()
        {
            // Collect each template.
            var templates = _affixCollectionHelper.AppliedAffixes.Select(a => a.Template);

            // Remove all affixes.
            ClearAffixes_Internal();

            // Reapply each affix from template with new rolls.
            foreach (var affix in templates.Select(t => t.GetNewRolledAffix()))
            {
                AddAffix_Internal(affix);
            }
        }
        public void RerollAffixValuesWhere(Predicate<Affix> predicate)
        {
            if (predicate == null)
            {
                return;
            }

            var affixesWhere = FindAffixesWhere(predicate);
            if (affixesWhere.Any())
            {
                var newAffixes = new List<Affix>();
                foreach (var oldAffix in affixesWhere)
                {
                    var newAffix = oldAffix.Template.GetNewRolledAffix();
                    newAffixes.Add(newAffix);   //For setting dirty.

                    RemoveAffix_Internal(oldAffix);

                    AddAffix_Internal(newAffix);
                }
            }
        }
        public void RerollAllAffixValues(AffixType withAffixType) => RerollAffixValuesWhere(a => a.Template.AffixType == withAffixType);

        public void AddAffix(Affix preRolledAffix)
        {
            if (CheckAffixIsAllowed(preRolledAffix))
            {
                AddAffix_Internal(preRolledAffix);
            }
        }
        public void AddAffix(IAffixTemplate affixToRoll)
        {
            if (affixToRoll == null)
            {
                return;
            }

            AddAffix(affixToRoll.GetNewRolledAffix());
        }

        public void AddAffixes(IEnumerable<Affix> preRolledAffixes)
        {
            if (preRolledAffixes == null)
            {
                return;
            }

            var appliedAffixes = new List<Affix>();
            foreach (var affix in preRolledAffixes)
            {
                if (CheckAffixIsAllowed(affix))
                {
                    AddAffix_Internal(affix);
                    appliedAffixes.Add(affix);
                }
            }
        }

        public enum RemovalMethod
        {
            JUST_ONE = 0,
            ALL_INSTANCES = 1,
        }
        public void RemoveAffix(IAffixTemplate affixTemplate, RemovalMethod removalMethod = RemovalMethod.ALL_INSTANCES)
        {
            var appliedAffixes = _affixCollectionHelper.AppliedAffixes;
            if (removalMethod == RemovalMethod.JUST_ONE)
            {
                if (appliedAffixes.Any(a => a.Template == affixTemplate))
                {
                    var found = appliedAffixes.First(a => a.Template == affixTemplate);
                    RemoveAffix_Internal(found);
                }
            }
            // ALL_INSTANCES.
            else
            {
                var found = FindAffixesWhere(a => a.Template == affixTemplate);

                // Same as "found.Count() > 0".
                if (found.Any())
                {
                    foreach (var foundAffix in found)
                    {
                        RemoveAffix_Internal(foundAffix);
                    }
                }
            }
        }
        public void RemoveAffixes(IEnumerable<IAffixTemplate> affixTemplates, RemovalMethod removalMethod = RemovalMethod.ALL_INSTANCES)
        {
            foreach (var t in affixTemplates)
            {
                RemoveAffix(t, removalMethod);
            }
        }
        public void RemoveAllAffixes()
        {
            foreach (var removedAffix in new List<Affix>(_affixCollectionHelper.AppliedAffixes))
            {
                // In case events are raised on each affix removed.
                RemoveAffix(removedAffix.Template, RemovalMethod.ALL_INSTANCES);
            }
        }
        public bool HasAffixWhere(Predicate<Affix> predicate)
        {
            return _affixCollectionHelper.AppliedAffixes.Any(a => predicate(a));
        }
        public bool HasAffix(IAffixTemplate withTemplate)
        {
            if (withTemplate == null)
            {
                return false;
            }
            return HasAffixWhere(a => a.Template.Equals(withTemplate));
        }
        public bool HasAffixesWhere(int count, Predicate<Affix> predicate)
        {
            var appliedAffixes = _affixCollectionHelper.AppliedAffixes;
            if (count <= 0 || count > appliedAffixes.Count())
            {
                return false;
            }
            int found = 0;
            foreach (var affix in appliedAffixes)
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
        public int CountAffixesWhere(Predicate<Affix> predicate)
        {
            return FindAffixesWhere(predicate).Count();
        }
        public virtual string GetModifiedObjectName(string objectName)
        {
            foreach (var type in AffixType.GetAllByPriority())
            {
                foreach (var affixOfType in FindAffixes(type))
                {
                    objectName = type.ApplyName(affixOfType.Template.ExternalName, objectName);
                }
            }
            return objectName;
        }

        protected IEnumerable<Affix> FindAffixesWhere(Predicate<Affix> predicate)
        {
            return _affixCollectionHelper.AppliedAffixes.Where(a => predicate(a));
        }
        protected IEnumerable<Affix> FindAffixes(AffixType withAffixType)
        {
            return FindAffixesWhere(a => a.Template.AffixType == withAffixType);
        }
        // TODO: Replace with helper class that manages return codes (reasons why it's not allowed).
        protected bool CheckAffixIsAllowed(Affix affix)
        {
            // Check affixType limitations.

            // Check for max affixes of type.
            var affixType = affix.Template.AffixType;
            if (_affixCollectionHelper.AppliedAffixes.Count(a => a.Template.AffixType == affixType) >= affixType.MaxAffixesOfType)
            {
                return false;
            }

            // Other limitations here.

            return true;
        }
        protected virtual void LateUpdateCache() { }
        protected void AddAffix_NoQuestionsAsked(Affix affix)
        {
            AddAffix_Internal(affix);
        }
        protected void AddAffixes_NoQuestionsAsked(IEnumerable<Affix> affixes)
        {
            foreach (var affix in affixes)
            {
                AddAffix_Internal(affix);
            }
        }

        private void AddAffix_Internal(Affix affix)
        {
            // It may be useful to raise an event here.

            _affixCollectionHelper.AddAffix(affix);
        }
        private void RemoveAffix_Internal(Affix affix)
        {
            // It may be useful to raise an event here.

            _affixCollectionHelper.RemoveAffix(affix);
        }
        private void ClearAffixes_Internal()
        {
            // It may be useful to raise an event here.

            _affixCollectionHelper.ClearAffixes();
        }

        private sealed class AffixCollectionHelper : CachedValueController<Dictionary<Type, List<ModCollection>>>
        {
            private readonly List<Affix> _appliedAffixes = new List<Affix>();
            public IEnumerable<Affix> AppliedAffixes => _appliedAffixes;

            // Add internal.
            public void AddAffix(Affix affix)
            {
                _appliedAffixes.Add(affix);

                SetDirty();
            }

            // Remove internal.
            public void RemoveAffix(Affix affix)
            {
                _appliedAffixes.Remove(affix);

                SetDirty();
            }

            // Clear internal.
            public void ClearAffixes()
            {
                _appliedAffixes.Clear();

                SetDirty();
            }

            public IEnumerable<TCollection> GetModCollections<TCollection>()
                where TCollection : ModCollection
            {
                return (IEnumerable<TCollection>)GetCacheCopy()[typeof(TCollection)];
            }

            protected override Dictionary<Type, List<ModCollection>> CalculateNewCache()
            {
                var workingDict = new Dictionary<Type, List<ModCollection>>();
                foreach (var affix in _appliedAffixes)
                {
                    foreach (var affixKvp in affix.ModCollectionDictionary)
                    {
                        if (workingDict.TryGetValue(affixKvp.Key, out List<ModCollection> foundCollection))
                        {
                            foundCollection.Add(affixKvp.Value);
                        }
                        else
                        {
                            workingDict.Add(affixKvp.Key, new List<ModCollection>() { affixKvp.Value });
                        }
                    }
                }
                return workingDict;
            }

            protected override Dictionary<Type, List<ModCollection>> CreateCacheCopy(Dictionary<Type, List<ModCollection>> cache)
            {
                return cache;
            }
        }
    }
}
