using System;
using System.Linq;
using System.Collections.Generic;
using KRpgLib.Affixes.ModTemplates;
using KRpgLib.Utility.KomponentObject;
using System.Collections;

namespace KRpgLib.Affixes
{
    // Each affix has one of these for each category of mod (stat detla, flag, etc...).
    public interface IModTemplateCollection : IKomponent
    {
        ModCollection GetNewRolledModCollection();
    }
    public abstract class ModTemplateCollection<TModTemplate, TRolledResult> : IModTemplateCollection, IEnumerable<TModTemplate>
        where TModTemplate : class, IModTemplate<TRolledResult>
    {
        protected readonly List<TModTemplate> _modTemplateList;
        protected ModTemplateCollection(IEnumerable<TModTemplate> modTemplates)
        {
            _modTemplateList = new List<TModTemplate>(modTemplates?.Where(mt => mt != null) ?? throw new ArgumentNullException(nameof(modTemplates)));
        }

        public bool HasAnyModTemplate => _modTemplateList.Count > 0;
        public bool HasModTemplate(TModTemplate modTemplate) => _modTemplateList.Exists(mt => mt.Equals(modTemplate));
        public int CountModTemplates() => _modTemplateList.Count;
        public int CountModTemplates(TModTemplate modTemplate) => _modTemplateList.Count(mt => mt.Equals(modTemplate));

        public ModCollection<TModTemplate, TRolledResult> GetNewRolledModCollection()
        {
            var mods = new List<Mod<TModTemplate, TRolledResult>>(_modTemplateList.ConvertAll(mt => new Mod<TModTemplate, TRolledResult>(mt, mt.GetNewRolledResult())));
            var combinedResult = CreateCombinedResult(mods.Select(m => m.RolledResult));

            return new ModCollection<TModTemplate, TRolledResult>(mods, combinedResult);
        }
        ModCollection IModTemplateCollection.GetNewRolledModCollection() => this.GetNewRolledModCollection();

        public IEnumerator<TModTemplate> GetEnumerator()
        {
            return ((IEnumerable<TModTemplate>)_modTemplateList).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_modTemplateList).GetEnumerator();

        protected abstract TRolledResult CreateCombinedResult(IEnumerable<TRolledResult> individualResults);
    }
}
