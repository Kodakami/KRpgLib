using System.Collections.Generic;
using KRpgLib.Utility;
using KRpgLib.Utility.KomponentObject;
using KRpgLib.Utility.TemplateObject;

namespace KRpgLib.Items
{
    public interface IItemTemplate :ITemplate
    {
        Item CreateItem();
    }
    public interface IItemTemplate<TItem> : IItemTemplate
        where TItem : Item
    {
        new TItem CreateItem();
    }
    public class Item : KomponentObject<IItemComponent>, ITemplateObject<IItemTemplate>, IInternallyNamed
    {
        public IItemTemplate Template { get; }
        public string InternalName { get; }

        /// <summary>
        /// Create an item with a name and no stacking component.
        /// </summary>
        public Item(IItemTemplate template, string internalName, IEnumerable<IItemComponent> komponents)
            :base(komponents)
        {
            Template = template;
            InternalName = internalName;
        }
        public Item(IItemTemplate template, string internalName, StackingComponent stackingComponent, IEnumerable<IItemComponent> komponents)
            :this(template, internalName, komponents)
        {
            // Will throw on registration if null or invalid.
            RegisterKomponent(stackingComponent);
        }
    }
}
