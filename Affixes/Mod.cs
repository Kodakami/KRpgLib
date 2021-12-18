using KRpgLib.Utility.KomponentObject;
using KRpgLib.Utility.TemplateObject;
using System;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class with shared functionality for mod instances with and without rolled arg values.
    /// </summary>
    public abstract class Mod : ITemplateObject<ModTemplate>
    {
        // NOTE: Do not make this class non-abstract. Make a subclass with no modifications if desired. Important for serialization.

        public ModTemplate Template { get; }

        protected internal Mod(ModTemplate template)
        {
            Template = template;
        }

        /// <summary>
        /// Randomly (or not), roll a new argument for the mod. Return true if the value changed. This implementation for mods without arguments always returns false.
        /// </summary>
        public virtual bool RollNewArg(Random rng) => false;

        // Delegate to mod template.
        public IModEffect GetModEffect() => Template.GetModEffect(this);
    }

    // TODO: Make concrete Mod class with no args.

    /// <summary>
    /// A mod instance with a rolled arg value.
    /// </summary>
    public sealed class Mod<TArg> : Mod
        // TArg is an arbitrary Type
    {
        // Hides inherrited member with strong-typed access.
        public new ModTemplate<TArg> Template => (ModTemplate<TArg>)base.Template;
        public TArg Arg { get; private set; }

        // Ctor
        public Mod(ModTemplate<TArg> template) : base(template) { }
        public Mod(ModTemplate<TArg> template, TArg initialArgValue) : base(template)
        {
            SetArgValueManually(initialArgValue);
        }

        /// <summary>
        /// Randomly (or not), roll a new argument for the mod. Return true if the value changed.
        /// </summary>
        public override bool RollNewArg(Random rng)
        {
            // Roll the new value.
            var newArgValue = Template.GetNewArg(rng);

            return SetArgValueManually(newArgValue);
        }
        public bool SetArgValueManually(TArg arg)
        {
            // Save the previous value.
            var oldArgValue = Arg;

            // Assign the new value to the property.
            Arg = arg;

            // Return true if the values are different.
            return !Arg.Equals(oldArgValue);
        }
    }
}
