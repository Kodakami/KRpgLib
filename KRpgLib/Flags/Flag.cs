namespace KRpgLib.Flags
{
    public struct Flag
    {
        public IFlagTemplate Template { get; }
        public int VariantIndex { get; }

        // ctor
        private Flag(IFlagTemplate template, int variantIndex)
        {
            Template = template;
            VariantIndex = variantIndex;
        }
        public bool SameTemplateAs(IFlagTemplate template)
        {
            // Reference equality.
            return Template == template;
        }
        public bool SameTemplateAs(Flag otherFlag) => SameTemplateAs(otherFlag.Template);
        public bool SameAs(IFlagTemplate template, int variantIndex)
        {
            // The same if the templates are the same and, if there are variants, the variant index is the same.
            return SameTemplateAs(template) && (Template.VariantCount <= 0 || VariantIndex == variantIndex);
        }
        public bool SameAs(Flag otherflag) => SameAs(otherflag.Template, otherflag.VariantIndex);

        // Static creation method.
        public static bool TryCreate(IFlagTemplate template, int variantIndex, out Flag newFlag)
        {
            if (template == null)
            {
                newFlag = default;
                return false;
            }
            
            if (template.VariantCount >= 0)
            {
                if (variantIndex < template.VariantCount)
                {
                    newFlag = new Flag(template, variantIndex);
                    return true;
                }

                newFlag = default;
                return false;
            }

            newFlag = new Flag(template, 0);
            return true;
        }
        public static bool TryCreate(IFlagTemplate template, out Flag newFlag) => TryCreate(template, 0, out newFlag);
    }
}
