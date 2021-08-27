using System;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Base class with shared functionality for mod instances with and without rolled arg values.
    /// </summary>
    public abstract class ModBase
    {
        public ModTemplateBase Template { get; }

        protected ModBase(ModTemplateBase template)
        {
            Template = template;
        }
        public abstract void Modify(ModdableDataManager manager);
        public virtual void RollNewValue(Random rng) { }
    }
    /// <summary>
    /// A mod instance without rolled arg values.
    /// </summary>
    /// <typeparam name="TData">Type of moddable data set the mod will act on</typeparam>
    public class Mod<TData> : ModBase where TData : IModdableDataSet
    {
        public Mod(ModTemplate<TData> template) : base(template) { }

        // Hides inherrited member with strong-typed access.
        public new ModTemplate<TData> Template => (ModTemplate<TData>)base.Template;

        // Delegate to mod template.
        public override void Modify(ModdableDataManager manager) => Template.Modify(manager);
    }
    /// <summary>
    /// A mod instance with a rolled arg value.
    /// </summary>
    /// <typeparam name="TData">Type of moddable data set the mod will act on</typeparam>
    public class Mod<TData, TArg> : ModBase where TData : IModdableDataSet
        // TArg is an arbitrary Type
    {
        // Hides inherrited member with strong-typed access.
        public new ModTemplate<TData, TArg> Template => (ModTemplate<TData, TArg>)base.Template;

        // The rolled arg value.
        public TArg Arg { get; protected set; }

        // Ctor
        public Mod(ModTemplate<TData, TArg> template) : base(template) { }

        // Delegate to mod template.
        public override void Modify(ModdableDataManager manager) => Template.Modify(manager, Arg);
        public override void RollNewValue(Random rng) => Arg = Template.GetNewArg(rng);
    }
}
