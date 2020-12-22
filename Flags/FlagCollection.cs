using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Flags
{
    /// <summary>
    /// A collection of unique flags with methods for accessing implied flags.
    /// </summary>
    public class FlagCollection
    {
        private readonly List<Flag> _topLevelSet = new List<Flag>();

        protected virtual int MaxRecursionDepth => 7;

        public FlagCollection() { }
        public FlagCollection(IEnumerable<Flag> flags)
        {
            // Flags cannot have invalid internal state (null template, out of range index). Create() methods in struct ensure validity.

            foreach (var flag in flags ?? throw new ArgumentNullException(nameof(flags)))
            {
                AddIfNotInSet(flag);
            }
        }
        public FlagCollection(IEnumerable<FlagCollection> flagCollections)
        {
            foreach (var fc in flagCollections ?? throw new ArgumentNullException(nameof(flagCollections)))
            {
                foreach (var flag in fc._topLevelSet)
                {
                    AddIfNotInSet(flag);
                }
            }
        }
        public FlagCollection(params FlagCollection[] flagCollections)
            :this(new List<FlagCollection>(flagCollections)) { }

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

        protected void AddIfNotInSet(Flag flag)
        {
            if (!_topLevelSet.Exists(f => f.SameAs(flag)))
            {
                _topLevelSet.Add(flag);
            }
        }
        protected void RemoveIfNotInSet(Flag flag)
        {
            _topLevelSet.Remove(flag);
        }
        protected void ClearAll()
        {
            _topLevelSet.Clear();
        }
    }
    /// <summary>
    /// A collection of unique flags with methods for accessing implied flags. If a flag would be added that is already contained in the top-level set, it is silently discarded. If a flag would be removed that is not in the top-level set, the call is silently ignored.
    /// </summary>
    public class WriteableFlagCollection : FlagCollection
    {
        public WriteableFlagCollection() { }
        public WriteableFlagCollection(IEnumerable<Flag> flags) : base(flags) { }
        public WriteableFlagCollection(IEnumerable<FlagCollection> flagCollections) : base(flagCollections) { }
        public WriteableFlagCollection(params FlagCollection[] flagCollections) : base(flagCollections) { }

        public void Add(Flag flag)
        {
            AddIfNotInSet(flag);
        }
        public void AddRange(IEnumerable<Flag> flags)
        {
            foreach (var flag in flags)
            {
                Add(flag);
            }
        }
        public void Remove(Flag flag)
        {
            RemoveIfNotInSet(flag);
        }
        public void RemoveAll(IEnumerable<Flag> flags)
        {
            foreach (var flag in flags)
            {
                Remove(flag);
            }
        }
        new public void ClearAll()
        {
            base.ClearAll();
        }
    }
}
