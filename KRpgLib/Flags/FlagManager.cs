using System.Collections.Generic;

namespace KRpgLib.Flags
{
    public class FlagManager : AbstractFlagSet
    {
        protected List<FlagProviderController> _controllers = new List<FlagProviderController>();

        public FlagManager() { }
        public FlagManager(List<IFlagProvider> initialProviders)
        {
            foreach (var ip in initialProviders)
            {
                AddFlagProvider(ip);
            }
        }

        public void AddFlagProvider(IFlagProvider flagProvider)
        {
            if (flagProvider != null)
            {
                var newController = new FlagProviderController(flagProvider);
                _controllers.Add(newController);
            }
        }
        public void RemoveFlagProvider(IFlagProvider flagProvider)
        {
            if (flagProvider != null)
            {
                for (int i = 0; i < _controllers.Count; i++)
                {
                    if (_controllers[i].Provider == flagProvider)
                    {
                        _controllers[i].Dispose();
                        _controllers.RemoveAt(i);
                        return;
                    }
                }
            }
        }

        protected override List<Flag> GetFlags()
        {
            List<Flag> outList = new List<Flag>();
            foreach (var controller in _controllers)
            {
                var flagList = controller.GetFlags();
                if (flagList != null)
                {
                    foreach (var flag in flagList)
                    {
                        if (!outList.Exists(f => f.SameAs(flag)))
                        {
                            outList.Add(flag);
                        }
                    }
                }
            }
            return outList;
        }

        // Internal class.
        protected sealed class FlagProviderController : System.IDisposable
        {
            public IFlagProvider Provider { get; }
            private List<Flag> _flagCache;

            private bool _isDirty;
            private void SetDirty() => _isDirty = true;

            public FlagProviderController(IFlagProvider flagProvider)
            {
                Provider = flagProvider;
                _isDirty = true;

                Provider.OnFlagsChanged += SetDirty;
            }
            public List<Flag> GetFlags()
            {
                if (_isDirty)
                {
                    UpdateCache();
                }

                return _flagCache;
            }
            public void UpdateCache()
            {
                _flagCache = Provider.GetAllFlags();
                _isDirty = false;
            }
            public void Dispose()
            {
                Provider.OnFlagsChanged -= SetDirty;
            }
        }
    }
}
