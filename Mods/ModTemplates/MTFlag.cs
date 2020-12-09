using System;
using KRpgLib.Flags;

namespace KRpgLib.Mods.ModTemplates
{
    public abstract class AbstractMTFlag : IModTemplate<Flag>
    {
        public IFlagTemplate Template { get; }

        protected AbstractMTFlag(IFlagTemplate template)
        {
            Template = template;
        }
        public Flag GetNewRolledResult()
        {
            int variantIndex = GetVariantIndex();
            return Flag.Create(Template, variantIndex);
        }
        protected abstract int GetVariantIndex();
    }
    public class MTFlag : AbstractMTFlag
    {
        private readonly int _variantIndex;
        public MTFlag(IFlagTemplate template, int variantIndex)
            : base(template)
        {
            _variantIndex = variantIndex;
        }
        protected override int GetVariantIndex()
        {
            return _variantIndex;
        }
    }
    public class MTFlag_RandomVariant : AbstractMTFlag
    {
        public MTFlag_RandomVariant(IFlagTemplate template)
            : base(template) { }

        protected override int GetVariantIndex()
        {
            return Template.VariantCount > 1 ? Utility.Environment.Rng.Next(0, Template.VariantCount) : 0;
        }
    }
    public class MTFlag_LimitedRandomVariants : AbstractMTFlag
    {
        private readonly int[] _rollableVariants;
        public MTFlag_LimitedRandomVariants(IFlagTemplate template, params int[] rollableVariants)
            : base(template)
        {
            if (rollableVariants == null)
            {
                throw new ArgumentNullException(nameof(rollableVariants));
            }
            if (rollableVariants.Length == 0)
            {
                throw new ArgumentException("Argument must not be an empty array.", nameof(rollableVariants));
            }
            for (int i = 0; i < rollableVariants.Length; i++)
            {
                if (rollableVariants[i] < 0 || rollableVariants[i] >= rollableVariants.Length)
                {
                    throw new ArgumentException("Argument must not contain items out of range.", nameof(rollableVariants));
                }
            }

            _rollableVariants = rollableVariants;
        }
        protected override int GetVariantIndex()
        {
            return _rollableVariants[Utility.Environment.Rng.Next(0, _rollableVariants.Length)];
        }
    }
}
