using KRpgLib.Utility.KomponentObject;
using KRpgLib.Utility.TemplateObject;
using System;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class for all types of mod templates.
    /// </summary>
    public abstract class ModTemplate : ITemplate
    {
        // TODO: Tags.

        public Mod CreateNewModInstance(Random rng)
        {
            var mod = CreateNewModInstance_Internal();

            // Bad developers may return null.
            mod?.RollNewArg(rng);

            return mod;
        }

        /// <summary>
        /// Return a new randomly-rolled arg value for the mod instance.
        /// </summary>
        public virtual object GetNewArg(Random rng, Mod modInstance) { return null; }

        /// <summary>
        /// Create a new mod instance of the correct type and pass it back (no need for pre-rolling).
        /// </summary>
        protected virtual Mod CreateNewModInstance_Internal() => new Mod(this);

        public abstract IModEffect GetModEffect(Mod modInstance);
    }
    /// <summary>
    /// Base class for mod templates with arguments (rolls).
    /// </summary>
    /// <typeparam name="TArg">Arbitrary Type for the rolled argument value</typeparam>
    public abstract class ModTemplate<TArg> : ModTemplate
        // TArg is an arbitrary type.
    {
        /// <summary>
        /// Return a new randomly-rolled arg value for the mod instance.
        /// </summary>
        public abstract TArg GetNewArg(Random rng, Mod<TArg> modInstance);
        public override object GetNewArg(Random rng, Mod modInstance)
        {
            var modInstaceType = modInstance.GetType();
            var targetType = typeof(Mod<TArg>);

            if (modInstaceType == targetType || modInstaceType.IsSubclassOf(targetType))
            {
                var castMod = (Mod<TArg>)modInstance;
                return GetNewArg(rng, castMod);
            }
            throw new ArgumentException($"Argument must be of type {targetType}.", nameof(modInstance));
        }

        public abstract IModEffect GetModEffect(Mod<TArg> modInstance);
        public override IModEffect GetModEffect(Mod modInstance)
        {
            var modInstaceType = modInstance.GetType();
            var targetType = typeof(Mod<TArg>);

            if (modInstaceType == targetType || modInstaceType.IsSubclassOf(targetType))
            {
                var castMod = (Mod<TArg>)modInstance;
                return GetModEffect(castMod);
            }
            throw new ArgumentException($"Argument must be of type {targetType}.", nameof(modInstance));
        }

        protected override Mod CreateNewModInstance_Internal() => new Mod<TArg>(this);
    }
}
