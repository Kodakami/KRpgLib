using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public interface IFlagSet
    {
        bool HasFlagTemplate(IFlagTemplate template);
        bool HasFlag(Flag flagInstance);
    }
    public abstract class AbstractFlagSet : IFlagSet
    {
        protected virtual int MaxRecursionDepth { get; } = 7;

        /// <summary>
        /// Get a collection of top-level flags in the flag set (implied flags not included). Collection should be a set of unique items.
        /// </summary>
        protected abstract List<Flag> GetFlags();

        public bool HasFlagTemplate(IFlagTemplate template)
        {
            foreach (Flag flag in GetFlags())
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
            foreach (Flag flag in GetFlags())
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
    }
}
