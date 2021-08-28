using KRpgLib.Utility.KomponentObject;
using KRpgLib.Utility.TemplateObject;
using System;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class with shared functionality for mod instances with and without rolled arg values.
    /// </summary>
    public class Mod : ITemplateObject<ModTemplate>
    {
        public ModTemplate Template { get; }

        // The rolled arg value.
        public object WeakArg { get; private set; }

        public Mod(ModTemplate template)
        {
            Template = template;
        }
        /// <summary>
        /// Randomly (or not), roll a new argument for the mod. Return true if the value changed.
        /// </summary>
        public bool RollNewArg(Random rng)
        {
            // Save the previous value.
            var oldArgValue = WeakArg;

            // Roll and assign the new value to the property.
            WeakArg = Template.GetNewArg(rng, this);

            // Return true if the values are different.
            return !WeakArg.Equals(oldArgValue);
        }

        // Delegate to mod template.
        public IModEffect GetModEffect() => Template.GetModEffect(this);
    }
    /// <summary>
    /// A mod instance with a rolled arg value.
    /// </summary>
    public sealed class Mod<TArg> : Mod
        // TArg is an arbitrary Type
    {
        // Hides inherrited member with strong-typed access.
        public new ModTemplate<TArg> Template => (ModTemplate<TArg>)base.Template;
        public TArg StrongArg => (TArg)WeakArg;

        // Ctor
        public Mod(ModTemplate<TArg> template) : base(template) { }
    }
}
