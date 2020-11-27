using KRpgLib.Utility;
using System;
using System.Collections.Generic;

namespace KRpgLib.Mods
{
    public sealed class AffixType
    {
        // Delegate.
        public delegate string ApplyNameDelegate(string affixName, string objectName);

        // Static members.
        private static readonly PriorityRegistry<AffixType> _registry = new PriorityRegistry<AffixType>();
        public static void RegisterAffixType(AffixType affixType, int priority) => _registry.RegisterItem(affixType, priority);
        public static void UnregisterAffixType(AffixType affixType) => _registry.UnregisterItem(affixType);
        public static List<AffixType> GetAllByPriority() => _registry.GetAllByPriority();

        // Instance members.
        private readonly ApplyNameDelegate _applyNameDelegate;
        public int MaxAffixesOfType { get; }

        public AffixType(ApplyNameDelegate applyNameDelegate, int maxAffixesOfType)
        {
            _applyNameDelegate = applyNameDelegate;
            MaxAffixesOfType = maxAffixesOfType;
        }

        public string ApplyName(string affixName, string objectName) => _applyNameDelegate.Invoke(affixName, objectName);
    }
}
