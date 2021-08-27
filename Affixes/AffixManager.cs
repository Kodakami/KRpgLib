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

        private readonly AffixApplicationHelper _affixApplicationHelper;

        public AffixManager()
        {
            _appliedAffixes = new List<Affix>();
            _affixApplicationHelper = new AffixApplicationHelper(this);
        }

        public void Modify(ModdableDataManager manager)
        {
            Modify_Internal(manager ?? throw new ArgumentNullException(nameof(manager)), _appliedAffixes);
        }
        /// <summary>
        /// Default implementation appiles each affix in an undefined order. Override for precise order of operations.
        /// </summary>
        /// <param name="safeManager"></param>
        /// <param name="appliedAffixes"></param>
        protected virtual void Modify_Internal(ModdableDataManager safeManager, IEnumerable<Affix> appliedAffixes)
        {
            foreach (var affix in _appliedAffixes)
            {
                affix.Modify(safeManager);
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
        public void AddAffix(AffixTemplate affixToRoll, Random rng)
        {
            if (affixToRoll == null)
            {
                throw new ArgumentNullException(nameof(affixToRoll));
            }

            AddAffix(affixToRoll.CreateNewAffixInstance(rng));
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

                // Check for max affixes of type.
                var affixType = affix.Template.AffixType;
                if (_parent.Count(affixType) >= affixType.MaxAffixesOfType)
                {
                    return new AffixApplicationStatus(AffixApplicationStatusType.AFFIX_TYPE_FULL, "Affix manager has no more room for affixes of the type.");
                }

                // Other limitations here.

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

                // Check for max affixes of type.
                var affixType = affix.Template.AffixType;
                if (_parent.Count(affixType) >= affixType.MaxAffixesOfType)
                {
                    return false;
                }

                // Other limitations here.

                return true;
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
        AFFIX_TYPE_FULL = 3,
    }
}
