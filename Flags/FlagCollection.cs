using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Flags
{
    /// <summary>
    /// A set of Flags (each member of the top-level collection is unique) with methods for accessing implied Flags. Enumerating the collection will only enumerate top-level flags. To check implied Flags, call HasFlag() or HasFlagTemplate().
    /// </summary>
    public class FlagCollection : IReadOnlyFlagCollection
    {
        private readonly List<Flag> _topLevelSet = new List<Flag>();

        protected virtual int MaxRecursionDepth => 7;

        public int Count => _topLevelSet.Count;

        // TODO: Rewrite as immutable collection just like Stats.

        public FlagCollection() { }
        public FlagCollection(IEnumerable<Flag> flags)
        {
            // Flags cannot have invalid internal state (null template, out of range index). Create() methods in struct ensure validity.

            foreach (var flag in flags ?? throw new ArgumentNullException(nameof(flags)))
            {
                AddIfNotPresent(flag);
            }
        }
        public FlagCollection(IEnumerable<IReadOnlyFlagCollection> flagCollections)
        {
            foreach (var fc in flagCollections ?? throw new ArgumentNullException(nameof(flagCollections)))
            {
                foreach (var flag in fc)
                {
                    AddIfNotPresent(flag);
                }
            }
        }
        public FlagCollection(params IReadOnlyFlagCollection[] flagCollections)
            :this((IEnumerable<IReadOnlyFlagCollection>)flagCollections) { }

        public bool HasFlagTemplate(IFlagTemplate template)
        {
            foreach (Flag flag in _topLevelSet)
            {
                if (HasFlagRecursive(flag, template, 0, false, 0))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasFlagTemplate(Flag flag) => HasFlagTemplate(flag.Template);

        public bool HasFlag(IFlagTemplate template, int variantIndex)
        {
            foreach (Flag flag in _topLevelSet)
            {
                if (HasFlagRecursive(flag, template, variantIndex, true, 0))
                {
                    return true;
                }
            }
            return false;
        }
        public bool HasFlag(Flag flag) => HasFlag(flag.Template, flag.VariantIndex);

        protected bool HasFlagRecursive(Flag thisFlag, IFlagTemplate targetTemplate, int targetVariantIndex, bool checkExactVariant, int recursionDepth)
        {
            if (checkExactVariant ? thisFlag.SameAs(targetTemplate, targetVariantIndex) : thisFlag.SameTemplateAs(targetTemplate))
            {
                return true;
            }

            if (recursionDepth > MaxRecursionDepth)
            {
                return false;
            }

            var impliedFlags = thisFlag.Template.GetAllImpliedFlags();
            if (impliedFlags != null)
            {
                foreach (var impliedFlag in impliedFlags)
                {
                    if (HasFlagRecursive(impliedFlag, targetTemplate, targetVariantIndex, checkExactVariant, recursionDepth + 1))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Add each Flag to the top-level set of flags if the same flag (same template, same variant) is not present. Returns true if all flags were added, returns false if at least one flag was already present.
        /// </summary>
        public bool AddRange(IEnumerable<Flag> flags)
        {
            bool allAdded = true;

            // Cloned list for iteration.
            foreach (var flag in new List<Flag>(flags))
            {
                // All are added if all have been added before now and this flag was added.
                allAdded = allAdded && AddIfNotPresent(flag);
            }
            return allAdded;
        }
        /// <summary>
        /// Remove each Flag from the top-level set of flags if it is present (same template, same variant). Returns true if all flags were removed, false if at least one flag was not present.
        /// </summary>
        public bool RemoveAll(IEnumerable<Flag> flags)
        {
            bool allRemoved = true;

            // Cloned list for iteration.
            foreach (var flag in new List<Flag>(flags))
            {
                // All are removed if all have been removed before now and this flag was removed.
                allRemoved = allRemoved && RemoveIfPresent(flag);
            }
            return allRemoved;
        }

        /// <summary>
        /// Add the Flag to the top-level set of flags if the same flag (same template, same variant) is not present. Returns true if flag was added.
        /// </summary>
        public bool AddIfNotPresent(Flag flag)
        {
            if (!_topLevelSet.Exists(f => f.SameAs(flag)))
            {
                _topLevelSet.Add(flag);
                return true;
            }
            return false;
        }
        /// <summary>
        /// Remove the Flag from the top-level set of flags if it is present (same template, same variant). Returns true if flag was removed.
        /// </summary>
        public bool RemoveIfPresent(Flag flag)
        {
            return _topLevelSet.Remove(flag);
        }

        public IEnumerator<Flag> GetEnumerator()
        {
            return ((IEnumerable<Flag>)_topLevelSet).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_topLevelSet).GetEnumerator();
        }
    }
    public interface IReadOnlyFlagCollection : IReadOnlyCollection<Flag>
    {
        bool HasFlagTemplate(IFlagTemplate template);
        bool HasFlagTemplate(Flag flag);
        bool HasFlag(IFlagTemplate template, int variantIndex);
        bool HasFlag(Flag flag);
    }
}
