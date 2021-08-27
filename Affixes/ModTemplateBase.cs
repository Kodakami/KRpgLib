using System;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class for all types of mod templates.
    /// </summary>
    public abstract class ModTemplateBase
    {
        // TODO: Tags.

        public ModBase CreateNewModInstance(Random rng)
        {
            var mod = CreateNewModInstance_Internal();
            mod.RollNewValue(rng);
            return mod;
        }

        /// <summary>
        /// Create a new mod instance of the correct type and pass it back (no need for pre-rolling).
        /// </summary>
        protected abstract ModBase CreateNewModInstance_Internal();
    }
    /// <summary>
    /// Base class for mod templates with no arguments (no rolls).
    /// </summary>
    /// <typeparam name="TData">Type of moddable data set the mod will act on</typeparam>
    public abstract class ModTemplate<TData> : ModTemplateBase
        where TData : IModdableDataSet
    {
        protected override ModBase CreateNewModInstance_Internal() => new Mod<TData>(this);
        public void Modify(ModdableDataManager manager)
        {
            var foundDataSet = manager.GetKomponent<TData>();
            if (foundDataSet != null)
            {
                Modify_Internal(foundDataSet);
            }
        }
        protected abstract void Modify_Internal(TData safeDataSet);
    }
    /// <summary>
    /// Base class for mod templates with arguments (rolls).
    /// </summary>
    /// <typeparam name="TData">Type of moddable data set the mod will act on</typeparam>
    /// <typeparam name="TArg">Arbitrary Type for the rolled argument value</typeparam>
    public abstract class ModTemplate<TData, TArg> : ModTemplateBase
        where TData : IModdableDataSet
        // TArg is an arbitrary type.
    {
        protected override ModBase CreateNewModInstance_Internal() => new Mod<TData, TArg>(this);

        /// <summary>
        /// Return a new randomly-rolled arg value for the mod instance.
        /// </summary>
        public abstract TArg GetNewArg(Random rng);
        /// <summary>
        /// Modify the moddable's component, supplying the rolled arg value from the mod instance.
        /// </summary>
        public void Modify(ModdableDataManager manager, TArg argValue)
        {
            var foundDataSet = manager.GetKomponent<TData>();
            if (foundDataSet != null)
            {
                Modify_Internal(foundDataSet, argValue);
            }
        }
        protected abstract void Modify_Internal(TData safeDataSet, TArg argValue);
    }
}
