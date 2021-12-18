using KRpgLib.Stats.Compound.AlgoBuilder;
using KRpgLib.Utility;
using System.Collections.Generic;
using KRpgLib.Stats.Compound;

namespace KRpgLib.Stats
{
    public sealed class StatEnvironment
    {
        // Static singleton management.
        public static StatEnvironment Instance { get; private set; }
        public static void SetInstance(StatEnvironment instance) => Instance = instance ?? throw new System.ArgumentNullException(nameof(instance));

        // Instance members.
        public DeltaTypeRepo DeltaTypes { get; }
        public IUniqueRepo<Stat> StatRepo { get; }

        /// <summary>
        /// Manually create a new stat environment. It is recommended that StatEnvironmentBuilder is used to create this instance instead.
        /// </summary>
        /// <param name="deltaTypeRepo">a delta type repo to govern the order in which deltas are applied to form stat values</param>
        public StatEnvironment(DeltaTypeRepo deltaTypeRepo, IUniqueRepo<Stat> statRepo)
        {
            DeltaTypes = deltaTypeRepo ?? throw new System.ArgumentNullException(nameof(deltaTypeRepo));
            StatRepo = statRepo ?? throw new System.ArgumentNullException(nameof(statRepo));
        }

        public sealed class DeltaTypeRepo
        {
            private readonly PriorityRegistry<DeltaType> _registry;

            public DeltaTypeRepo(PriorityRegistry<DeltaType> deltaTypeRegistry)
            {
                _registry = deltaTypeRegistry ?? throw new System.ArgumentNullException(nameof(deltaTypeRegistry));
            }

            /// <summary>
            /// Get all delta types in order of priority.
            /// </summary>
            /// <returns>read-only list of delta types</returns>
            public IReadOnlyList<DeltaType> GetAllByPriority() => _registry.GetAllByPriority();
        }
    }
}
