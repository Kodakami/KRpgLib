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
        public object ArgValue { get; private set; }
        public bool HasArg => Template.HasArg;

        internal Mod(ModTemplate template, object argValue)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));

            SetArgManually(argValue);
        }

        /// <summary>
        /// Randomly (or not), roll a new argument for the mod. Return true if the value changed.
        /// </summary>
        public bool RollNewArg(Random rng)
        {
            // Roll the new value.
            var newArgValue = Template.GetNewArgValue(rng);

            return SetArgManually(newArgValue);
        }
        public bool SetArgManually(object argValue)
        {
            if (Template.HasArg)
            {
                // Save the previous value.
                var oldArgValue = ArgValue;

                // Assign the new value to the property.
                ArgValue = argValue;

                // Return true if the values are different.
                return !ArgValue.Equals(oldArgValue);
            }
            return false;
        }

        // Delegate to mod template.
        public IModEffect GetModEffect() => Template.GetModEffect(this);
    }

    /// <summary>
    /// A mod instance with a rolled arg value.
    /// </summary>
    public sealed class Mod<T> : Mod
    {
        // Hides inherrited member with strong-typed access.
        public new ModTemplate<T> Template => (ModTemplate<T>)base.Template;
        public new T ArgValue => (T)base.ArgValue;

        public Mod(ModTemplate<T> template, T arg) : base(template, arg) { }
    }
    public sealed class Mod_NoArg : Mod
    {
        public Mod_NoArg(ModTemplate template) : base(template, null) { }
    }
}
