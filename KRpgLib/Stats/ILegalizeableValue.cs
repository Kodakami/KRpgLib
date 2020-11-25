namespace KRpgLib.Stats
{
    /// <summary>
    /// Interface for an object which returns a legalized value, given a raw value.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ILegalizeableValue<TValue> where TValue : struct
    {
        TValue GetLegalizedValue(TValue rawValue);
    }
}