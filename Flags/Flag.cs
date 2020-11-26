using System;
namespace KRpgLib.Flags
{
    /// <summary>
    /// A flag that represents a state or condition of something. If this flag is included in a flag manager, it means that something is true of the object represented by that manager. Flags are value types (structs) and therefore not passed by reference. Use SameTemplateAs() and SameAs() methods when checking for equality.
    /// </summary>
    public struct Flag
    {
        /// <summary>
        /// The template this flag is an instance of.
        /// </summary>
        public IFlagTemplate Template { get; }
        /// <summary>
        /// The index representing which variant of the template this flag is.
        /// </summary>
        public int VariantIndex { get; }

        // ctor
        private Flag(IFlagTemplate template, int variantIndex)
        {
            // We don't throw ArgNullEx or ArgEx here. Our Create() methods act as the gatekeepers of valid state.
            Template = template;
            VariantIndex = variantIndex;
        }
        /// <summary>
        /// Check whether this flag has a particular template (uses reference equality).
        /// </summary>
        /// <param name="template">template to compare</param>
        /// <returns>true if template is the same</returns>
        public bool SameTemplateAs(IFlagTemplate template)
        {
            // Reference equality.
            return Template == template;
        }
        /// <summary>
        /// Check whether this flag has the same template as another flag (uses reference equality).
        /// </summary>
        /// <param name="otherFlag">flag to compare</param>
        /// <returns>true if both flags share the same template</returns>
        public bool SameTemplateAs(Flag otherFlag) => SameTemplateAs(otherFlag.Template);
        /// <summary>
        /// Check whether this flag has a given template and variant index (uses reference equality for template).
        /// </summary>
        /// <param name="template">template to compare</param>
        /// <param name="variantIndex">variant index to compare</param>
        /// <returns>true if both template and variant index are the same</returns>
        public bool SameAs(IFlagTemplate template, int variantIndex)
        {
            // The same if the templates and variant indexes are the same.
            return SameTemplateAs(template) && VariantIndex == variantIndex;
        }
        /// <summary>
        /// Check whether this flag has the same template and variant index as another flag. If so, they represent the same state or condition.
        /// </summary>
        /// <param name="otherflag">flag to compare</param>
        /// <returns>true if both flags share the same template and variant index</returns>
        public bool SameAs(Flag otherflag) => SameAs(otherflag.Template, otherflag.VariantIndex);

        // Static creation methods.
        /// <summary>
        /// Create a new flag from a template and variant index. If the template or the variant index is invalid, an ArgumentException will be thrown.
        /// </summary>
        /// <param name="template">template this flag is an instance of</param>
        /// <param name="variantIndex">index representing which variant of the template this flag is</param>
        /// <returns>a new flag instance</returns>
        public static Flag Create(IFlagTemplate template, int variantIndex)
        {
            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }
            if (template.VariantCount < 1)
            {
                throw new ArgumentException("Template must have a variant count of at least 1.", nameof(template));
            }

            if (variantIndex < 0)
            {
                throw new ArgumentException("Variant index may not be negative.", nameof(variantIndex));
            }
            if (variantIndex >= template.VariantCount)
            {
                throw new ArgumentException($"Variant index for this flag may not be greater than {template.VariantCount - 1}.", nameof(variantIndex));
            }

            return new Flag(template, variantIndex);
        }
        ///<summary>
        /// Create a new flag from a template and variant index. If the template or the variant index is invalid, an ArgumentException will be thrown. This overload always returns a flag with a variant index of 0 (for templates with only one variant).
        /// </summary>
        /// <param name="template">template this flag is an instance of</param>
        /// <returns>a new flag instance</returns>
        public static Flag Create(IFlagTemplate template) => Create(template, 0);
    }
}
