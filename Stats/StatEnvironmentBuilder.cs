using KRpgLib.Utility;

namespace KRpgLib.Stats
{
    public sealed class StatEnvironmentBuilder<TValue> where TValue : struct
    {
        private readonly PriorityRegistry<DeltaType<TValue>> _registry = new PriorityRegistry<DeltaType<TValue>>();

        /// <summary>
        /// Register a new delta type with the stat environment, giving it a priority value for order of operations. Delta types with the same priority value will be evaluated in the order they were registered.
        /// </summary>
        /// <param name="priority">Priority value for the delta type. Delta types with lower priority values will be applied to a stat value sooner.</param>
        /// <param name="baselineValue">The neutral delta value (0 for addition, 1 for multiplication). If a delta has this value, it represents "no change".</param>
        /// <param name="deltaTypeFunc">The operation to perform on a stat value. Given a starting value and a delta value, return the result of the operation.</param>
        /// <param name="combineFunc">The operation for combining two delta values. Given two delta values, return the result of combining them. (simple addition, when using regular numeric types)</param>
        public void RegisterDeltaType(int priority, TValue baselineValue, DeltaType<TValue>.DeltaTypeFunc deltaTypeFunc, DeltaType<TValue>.DeltaTypeFunc combineFunc)
        {
            var newType = new DeltaType<TValue>(deltaTypeFunc, baselineValue, combineFunc);
            RegisterDeltaType(priority, newType);
        }
        /// <summary>
        /// Register a new delta type with the stat environment, giving it a priority value for order of operations. Delta types with the same priority value will be evaluated in the order they were registered.
        /// </summary>
        /// <param name="priority">Priority value for the delta type. Delta types with lower priority values will be applied to a stat value sooner.</param>
        public void RegisterDeltaType(int priority, DeltaType<TValue> deltaType)
        {
            _registry.RegisterItem(deltaType, priority);
        }

        /// <summary>
        /// Build the custom stat environment. Optionally, set the new stat environment as the singleton instance for the backing type.
        /// </summary>
        /// <param name="andRegisterAsSingletonInstance"></param>
        /// <returns></returns>
        public StatEnvironment<TValue> Build(bool andRegisterAsSingletonInstance)
        {
            var newInstance = new StatEnvironment<TValue>(new StatEnvironment<TValue>.DeltaTypeRepo(_registry));

            if (andRegisterAsSingletonInstance)
            {
                StatEnvironment<TValue>.SetInstance(newInstance);
            }

            return newInstance;
        }
    }
}
