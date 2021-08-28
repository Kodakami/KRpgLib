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
    public class Item : KomponentObject<ItemComponent>, ITemplateObject<IItemTemplate>
    {
        public IItemTemplate Template { get; }
        public string ExternalName { get; }

        /// <summary>
        /// Create an item with a name and no stacking component.
        /// </summary>
        public Item(IItemTemplate template, string externalName, IEnumerable<ItemComponent> komponents)
            :base(komponents)
        {
            Template = template;
            ExternalName = externalName;
        }
        public Item(IItemTemplate template, string externalName, StackingComponent stackingComponent, IEnumerable<ItemComponent> komponents)
            :this(template, externalName, komponents)
        {
            // Will throw on registration if null or invalid.
            RegisterKomponent(stackingComponent);
        }
    }
}
