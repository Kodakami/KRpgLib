using KRpgLib.Utility;
using KRpgLib.Utility.KomponentObject;
using System;
using System.Linq;
using System.Collections.Generic;
using KRpgLib.Affixes.ModTemplates;

namespace KRpgLib.Affixes
{
    public sealed class Affix
    {
        private readonly Dictionary<Type, ModCollection> _collectionDict;
        public IDictionary<Type, ModCollection> ModCollectionDictionary => _collectionDict;

        public IAffixTemplate Template { get; }

        private Affix(IAffixTemplate template, Dictionary<Type, ModCollection> collectionDict)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            _collectionDict = collectionDict;
        }
        public Affix(IAffixTemplate template, IEnumerable<ModCollection> collections)
            : this(template, collections?.ToDictionary(collection => collection.GetType()) ?? throw new ArgumentNullException(nameof(collections)))
        { }

        public TModCollection GetModCollection<TModCollection>()
            where TModCollection : ModCollection
        {
            // Type coordination is guaranteed in constructors.
            return (TModCollection)_collectionDict[typeof(TModCollection)];
        }
        public ModCollection<TModTemplate, TRolledResult> GetModCollection<TModTemplate, TRolledResult>()
            where TModTemplate : class, IModTemplate<TRolledResult>
        {
            return (ModCollection<TModTemplate, TRolledResult>)_collectionDict[typeof(ModCollection<TModTemplate, TRolledResult>)];
        }
    }
}
