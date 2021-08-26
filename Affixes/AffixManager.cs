using KRpgLib.Flags;
using KRpgLib.Stats;
using KRpgLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// A collector and manipulator of Affixes considered "applied" to something moddable.
    /// </summary>
    public class AffixManager
    {
        private readonly List<Affix> _appliedAffixes;

        public AffixManager()
        {
            _appliedAffixes = new List<Affix>();
        }

        public void Modify(IModdable moddable)
        {
            foreach (var affix in _appliedAffixes)
            {
                affix.Modify(moddable);
            }
        }

        public void RerollAllAffixValues(Random rng)
        {
            _appliedAffixes.ForEach(a => a.RerollAllMods(rng));
        }
        public void RerollAffixValues(Predicate<Affix> predicate, Random rng)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            foreach (var affix in FindAffixes(predicate))
            {
                affix.RerollAllMods(rng);
            }
        }
        public void RerollAffixValues(AffixType withAffixType, Random rng) => RerollAffixValues(a => a.Template.AffixType == withAffixType, rng);

        public void AddAffix(Affix preRolledAffix)
        {
            if (CheckAffixIsAllowed(preRolledAffix ?? throw new ArgumentNullException(nameof(preRolledAffix))))
            {
                AddAffix_Internal(preRolledAffix);
            }
        }
        public void AddAffix(AffixTemplate affixToRoll, Random rng)
        {
            if (affixToRoll == null)
            {
                throw new ArgumentNullException(nameof(affixToRoll));
            }

            AddAffix(affixToRoll.CreateNewAffixInstance(rng));
        }

        public void AddAffixes(IEnumerable<Affix> preRolledAffixes)
        {
            if (preRolledAffixes == null)
            {
                throw new ArgumentNullException(nameof(preRolledAffixes));
            }

            // Add the ones that are allowed.
            AddAffixes_Internal(preRolledAffixes.Where(a => CheckAffixIsAllowed(a)));
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
        protected bool CheckAffixIsAllowed(Affix affix)
        {
            // TODO: Check AffixType limitations.

            // Check if affix template already has an instance here.
            if (HasAffix(affix.Template))
            {
                return false;
            }

            // Check for max affixes of type.
            var affixType = affix.Template.AffixType;
            if (Count(affixType) >= affixType.MaxAffixesOfType)
            {
                return false;
            }

            // Other limitations here.

            return true;
        }
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
        }
        private void AddAffixes_Internal(IEnumerable<Affix> affixes)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.AddRange(affixes);
        }
        private void RemoveAffix_Internal(Affix affix)
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Remove(affix);
        }
        private void RemoveAffixes_Internal(IEnumerable<Affix> affixes)
        {
            // It may be useful to raise an event here.

            foreach (var affix in new List<Affix>(affixes))
            {
                _appliedAffixes.Remove(affix);
            }
        }
        private void ClearAffixes_Internal()
        {
            // It may be useful to raise an event here.

            _appliedAffixes.Clear();
        }
    }
}
