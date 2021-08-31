namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object which returns a legalized value, given a raw value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ILegalizeableValue<TValue> where TValue : struct
    {
        /// <summary>
        /// Given a value, returns the legalized version of that value, such as by minimum or maximum.
        /// </summary>
        TValue GetLegalizedValue(TValue rawValue);
    }
}