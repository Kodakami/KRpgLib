using System.Collections.Generic;

namespace KRpgLib.Utility.KomponentObject
{
    /// <summary>
    /// A plain implementation of KomponentObject for objects which use component management but can't or don't want to inherit from KomponentObject.
    /// </summary>
    public sealed class BasicKomponentManager : KomponentObject<IKomponent>
    {
        public BasicKomponentManager() { }
        public BasicKomponentManager(IKomponent komponent) : base(komponent) { }
        public BasicKomponentManager(IEnumerable<IKomponent> komponents) : base(komponents) { }
        public BasicKomponentManager(BasicKomponentManager other) : base(other) { }
        public BasicKomponentManager(IEnumerable<BasicKomponentManager> others) : base(others) { }

        public new void RegisterKomponent(IKomponent komponent) => base.RegisterKomponent(komponent);
        public new void UnregisterKomponent(IKomponent komponent) => base.UnregisterKomponent(komponent);
    }
    /// <summary>
    /// A plain implementation of KomponentObject for objects which use component management but can't or don't want to inherit from KomponentObject.
    /// </summary>
    public sealed class BasicKomponentManager<TKomponent> : KomponentObject<TKomponent> where TKomponent : IKomponent
    {
        public BasicKomponentManager() { }
        public BasicKomponentManager(TKomponent komponent) : base(komponent) { }
        public BasicKomponentManager(IEnumerable<TKomponent> komponents) : base(komponents) { }
        public BasicKomponentManager(BasicKomponentManager<TKomponent> other) : base(other) { }
        public BasicKomponentManager(IEnumerable<BasicKomponentManager<TKomponent>> others) : base(others) { }

        public new void RegisterKomponent(TKomponent komponent) => base.RegisterKomponent(komponent);
        public new void UnregisterKomponent(TKomponent komponent) => base.UnregisterKomponent(komponent);
    }
}
