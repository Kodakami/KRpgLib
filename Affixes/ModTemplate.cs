using KRpgLib.Utility;
using KRpgLib.Utility.TemplateObject;
using System;
using KRpgLib.Utility.Serialization;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class for all types of mod templates.
    /// </summary>
    public abstract class ModTemplate : ITemplate
    {
        // TODO: Tags.

        // Only subclasses in Utility can implement.
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

        public abstract IModEffect GetModEffect(Mod modInstance);

        public abstract ModSerializer GetModSerializer();
    }
    /// <summary>
    /// Base class for mod templates with arguments (rolls).
    /// </summary>
    /// <typeparam name="TArg">Arbitrary Type for the rolled argument value</typeparam>
    public abstract class ModTemplate<TArg> : ModTemplate
        // TArg is an arbitrary type.
    {
        private static ModSerializer<TArg> _modSerializerFlyweight;

        /// <summary>
        /// Return a new randomly-rolled arg value for the mod instance.
        /// </summary>
        public abstract TArg GetNewArg(Random rng);
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

        // TODO: Find a balance between the number of parallel mod inheritence lines. Branch serializing between arg and no-arg mods is ugly.
        // Need to remember that mods are getting serialized, not mod templates.

        public override ModSerializer GetModSerializer()
        {
            return _modSerializerFlyweight ?? (_modSerializerFlyweight = new ModSerializer<TArg>());
        }
        public abstract Serializer<TArg> GetArgSerializer();
    }
}