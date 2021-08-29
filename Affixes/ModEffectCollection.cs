using KRpgLib.Utility.KomponentObject;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Affixes
{
    public sealed class ModEffectCollection
    {
        private readonly ModEffectKomponentObject _komponentObject;

        public ModEffectCollection(IEnumerable<IModEffect> modEffects)
        {
            _komponentObject = new ModEffectKomponentObject(modEffects ?? throw new System.ArgumentNullException(nameof(modEffects)));
        }
        public ModEffectCollection(IEnumerable<ModEffectCollection> othersToCombine)
        {
            _komponentObject = new ModEffectKomponentObject(othersToCombine.Select(other => other._komponentObject));
        }

        public bool HasModEffect<TModEffect>() where TModEffect : IModEffect => _komponentObject.HasKomponent<TModEffect>();
        public IEnumerable<TModEffect> GetModEffects<TModEffect>() where TModEffect : IModEffect => _komponentObject.GetKomponents<TModEffect>();

        private sealed class ModEffectKomponentObject : KomponentObject<IModEffect>
        {
            public ModEffectKomponentObject(IEnumerable<IModEffect> komponents) : base(komponents) { }
            public ModEffectKomponentObject(IEnumerable<ModEffectKomponentObject> others) : base(others) { }
        }
    }
}
