using KRpgLib.Stats.Compound.AlgoBuilder;
using KRpgLib.Utility;
using System.Collections.Generic;
using KRpgLib.Stats.Compound;

namespace KRpgLib.Stats
{
    public sealed class StatEnvironment<TValue> where TValue : struct
    {
        // Static singleton management.
        public static StatEnvironment<TValue> Instance { get; private set; }
        public static void SetInstance(StatEnvironment<TValue> instance) => Instance = instance ?? throw new System.ArgumentNullException(nameof(instance));

        // Instance members.
        public DeltaTypeRepo DeltaTypes { get; }

        /// <summary>
        /// Manually create a new stat environment. It is recommended that StatEnvironmentBuilder is used to create this instance instead.
        /// </summary>
        /// <param name="deltaTypeRepo">a delta type repo to govern the order in which deltas are applied to form stat values</param>
        public StatEnvironment(DeltaTypeRepo deltaTypeRepo)
        {
            DeltaTypes = deltaTypeRepo ?? throw new System.ArgumentNullException(nameof(deltaTypeRepo));
        }

        public sealed class DeltaTypeRepo
        {
            private readonly PriorityRegistry<DeltaType<TValue>> _registry;

            public DeltaTypeRepo(PriorityRegistry<DeltaType<TValue>> deltaTypeRegistry)
            {
                _registry = deltaTypeRegistry ?? throw new System.ArgumentNullException(nameof(deltaTypeRegistry));
            }

            /// <summary>
            /// Get all delta types in order of priority.
            /// </summary>
            /// <returns>read-only list of delta types</returns>
            public IReadOnlyList<DeltaType<TValue>> GetAllByPriority() => _registry.GetAllByPriority();
        }
    }
}
