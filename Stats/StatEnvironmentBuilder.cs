using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    public sealed class StatEnvironmentBuilder
    {
        private readonly PriorityRegistry<DeltaType> _registry = new PriorityRegistry<DeltaType>();
        private IUniqueRepo<Stat> _statRepo;

        /// <summary>
        /// Register a new delta type with the stat environment, giving it a priority value for order of operations. Delta types with the same priority value will be evaluated in the order they were registered.
        /// </summary>
        /// <param name="priority">Priority value for the delta type. Delta types with lower priority values will be applied to a stat value sooner.</param>
        /// <param name="baselineValue">The neutral delta value (0 for addition, 1 for multiplication). If a delta has this value, it represents "no change".</param>
        /// <param name="deltaTypeFunc">The operation to perform on a stat value. Given a starting value and a delta value, return the result of the operation.</param>
        public void RegisterDeltaType(int priority, int baselineValue, DeltaType.DeltaTypeFunc deltaTypeFunc)
        {
            var newType = new DeltaType(deltaTypeFunc, baselineValue);
            RegisterDeltaType(priority, newType);
        }
        /// <summary>
        /// Register a new delta type with the stat environment, giving it a priority value for order of operations. Delta types with the same priority value will be evaluated in the order they were registered.
        /// </summary>
        /// <param name="priority">Priority value for the delta type. Delta types with lower priority values will be applied to a stat value sooner.</param>
        public void RegisterDeltaType(int priority, DeltaType deltaType)
        {
            _registry.RegisterItem(deltaType, priority);
        }

        /// <summary>
        /// Register a unique repo of stats with the stat environment. The unique repo acts as a lookup table and fetches a unique ID value for each stat. The unique ID values returned by the repo are mostly used for serialization, but the means by which they are determined is left to developer implementation.
        /// </summary>
        public void RegisterStatRepo(IUniqueRepo<Stat> statRepo)
        {
            _statRepo = statRepo ?? throw new System.ArgumentNullException(nameof(statRepo));
        }

        /// <summary>
        /// Build the custom stat environment. Optionally, set the new stat environment as the singleton instance.
        /// </summary>
        public StatEnvironment Build(bool andRegisterAsSingletonInstance)
        {
            var newInstance = new StatEnvironment(new StatEnvironment.DeltaTypeRepo(_registry), _statRepo);

            if (andRegisterAsSingletonInstance)
            {
                StatEnvironment.SetInstance(newInstance);
            }

            return newInstance;
        }
    }
}
