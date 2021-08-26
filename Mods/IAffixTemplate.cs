using KRpgLib.Affixes.ModTemplates;
using KRpgLib.Utility.KomponentObject;
using System;
using System.Linq;
using System.Collections.Generic;
using KRpgLib.Utility;

namespace KRpgLib.Affixes
{
    public interface IAffixTemplate : INamedObject
    {
        // ExternalName from INamedObject

        AffixType AffixType { get; }
        TModTemplateCollection GetModTemplateCollection<TModTemplateCollection, TModTemplate, TRolledResult>()
            where TModTemplateCollection : ModTemplateCollection<TModTemplate, TRolledResult>
            where TModTemplate : class, IModTemplate<TRolledResult>;
        Affix GetNewRolledAffix();
    }
    public abstract class AbstractAffixTemplate : KomponentObject<IModTemplateCollection>, IAffixTemplate
    {
        public string ExternalName { get; }
        public AffixType AffixType { get; }

        protected AbstractAffixTemplate(string externalName, AffixType affixType, IEnumerable<IModTemplateCollection> components)
            :base(components)
        {
            ExternalName = externalName ?? throw new ArgumentNullException(nameof(externalName));
            AffixType = affixType ?? throw new ArgumentNullException(nameof(affixType));
        }
        public TModTemplateCollection GetModTemplateCollection<TModTemplateCollection, TModTemplate, TRolledResult>()
            where TModTemplateCollection : ModTemplateCollection<TModTemplate, TRolledResult>
            where TModTemplate : class, IModTemplate<TRolledResult>
        {
            return GetKomponent<TModTemplateCollection>();
        }
        public Affix GetNewRolledAffix()
        {
            var components = new List<ModCollection>(GetAllKomponents().Select(c => c.GetNewRolledModCollection()));
            return new Affix(this, components);
        }
    }
    
}
