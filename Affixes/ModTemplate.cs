using KRpgLib.Utility;
using KRpgLib.Utility.TemplateObject;
using System;
using KRpgLib.Utility.Serialization;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class for all types of mod templates.
    /// </summary>
    public abstract class ModTemplate : ITemplate
    {
        // TODO: Tags.

        public ModArgType ArgType { get; }
        public bool HasArg => ArgType != null;

        // Only subclasses in Utility can implement.
        // EDIT: Is that right?
        protected internal ModTemplate(ModArgType argType)
        {
            // Can be null (for no-arg mods).
            ArgType = argType;
        }
        protected internal ModTemplate() { }

        public Mod CreateNewModInstance(Random rng)
        {
            var mod = CreateNewModInstance_Internal();

            // Bad developers might return null.
            mod?.RollNewArg(rng);

            return mod;
        }

        /// <summary>
        /// Create a new mod instance of the correct type and pass it back (no need for pre-rolling).
        /// </summary>
        protected virtual Mod CreateNewModInstance_Internal() => default;

        public IModEffect GetModEffect(Mod modInstance)
        {
            if (modInstance?.Template == this)
            {
                return GetModEffect_Internal(modInstance);
            }
            throw new ArgumentException("Argument must have the same mod template.", nameof(modInstance));
        }
        protected abstract IModEffect GetModEffect_Internal(Mod safeModInstance);

        /// <summary>
        /// Return a new randomly-rolled arg value for the mod instance (if it has args).
        /// </summary>
        public abstract object GetNewArgValue(Random rng);
    }

    /// <summary>
    /// Base class for mod templates with arguments (rolls).
    /// </summary>
    /// <typeparam name="T">Arbitrary Type for the rolled argument value</typeparam>
    public abstract class ModTemplate<T> : ModTemplate
        // TArg is an arbitrary type.
    {
        // ArgType is required here.
        protected ModTemplate(ModArgType<T> argType) : base(argType ?? throw new ArgumentNullException(nameof(argType))) { }

        protected override Mod CreateNewModInstance_Internal() => new Mod<T>(this, default);

        public override object GetNewArgValue(Random rng)
        {
            return GetNewArgStrongValue(rng);
        }
        protected abstract T GetNewArgStrongValue(Random rng);
    }
    public abstract class ModTemplate_NoArg : ModTemplate
    {
        // No ArgType here.
        protected ModTemplate_NoArg() { }

        protected override Mod CreateNewModInstance_Internal() => new Mod_NoArg(this);
        public override object GetNewArgValue(Random rng) => null;
    }
}