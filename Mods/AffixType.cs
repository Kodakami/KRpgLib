using KRpgLib.Utility;
using System;
using System.Collections.Generic;

namespace KRpgLib.Affixes
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
            _applyNameDelegate = applyNameDelegate ?? throw new ArgumentNullException(nameof(applyNameDelegate));
            MaxAffixesOfType = maxAffixesOfType >= 0 ? maxAffixesOfType : throw new ArgumentOutOfRangeException(nameof(maxAffixesOfType), "Argument must be greater than or equal to 0.");
        }

        public string ApplyName(string affixName, string objectName) =>_applyNameDelegate.Invoke(
            affixName ?? throw new ArgumentNullException(nameof(affixName)),
            objectName ?? throw new ArgumentNullException(objectName));
    }
}
