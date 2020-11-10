namespace KRpgLib.Stats
{
    public interface ILegalizeableValue<TValue> where TValue : struct
    {
        TValue GetLegalizedValue(TValue rawValue);
    }
}