using System;
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
            AddFlagProvider_Internal(new FlagProviderController(flagProvider ?? throw new ArgumentNullException(nameof(flagProvider))));
        }
        public void AddFlagProvider(IFlagProvider_Dynamic dynamicFlagProvider)
        {
            AddFlagProvider_Internal(new FlagProviderController_Dynamic(dynamicFlagProvider ?? throw new ArgumentNullException(nameof(dynamicFlagProvider))));
        }
        private void AddFlagProvider_Internal(FlagProviderController controller)
        {
            _controllers.Add(controller);
        }
        public void RemoveFlagProvider(IFlagProvider flagProvider)
        {
            if (flagProvider == null)
            {
                throw new ArgumentNullException(nameof(flagProvider));
            }

            for (int i = 0; i < _controllers.Count; i++)
            {
                var inspected = _controllers[i];
                if (inspected.Provider == flagProvider)
                {
                    if (inspected.IsDynamic())
                    {
                        var casted = (FlagProviderController_Dynamic)inspected;
                        casted.Dispose();
                    }

                    _controllers.RemoveAt(i);
                    return;
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

        // Internal classes.
        protected class FlagProviderController
        {
            public IFlagProvider Provider { get; }
            protected List<Flag> _flagCache;
            public virtual bool IsDynamic() => false;

            public FlagProviderController(IFlagProvider flagProvider)
            {
                Provider = flagProvider;
                _flagCache = flagProvider.GetAllFlags();
            }
            public virtual List<Flag> GetFlags()
            {
                return _flagCache;
            }
        }
        protected class FlagProviderController_Dynamic : FlagProviderController, System.IDisposable
        {
            public override bool IsDynamic() => true;

            private bool _isDirty = false;  // No need to set true. Base constructor gets current value regardless.
            private void SetDirty() => _isDirty = true;

            public FlagProviderController_Dynamic(IFlagProvider_Dynamic flagProvider)
                :base(flagProvider)
            {
                flagProvider.OnFlagsChanged += SetDirty;
            }
            public override List<Flag> GetFlags()
            {
                if (_isDirty)
                {
                    UpdateCache();
                }
                return base.GetFlags();
            }
            public void UpdateCache()
            {
                _flagCache = Provider.GetAllFlags();
                _isDirty = false;
            }
            public void Dispose()
            {
                var casted = (IFlagProvider_Dynamic)Provider;

                casted.OnFlagsChanged -= SetDirty;
            }
        }
    }
}
