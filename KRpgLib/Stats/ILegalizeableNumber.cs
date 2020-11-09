namespace KRpgLib.Stats
{
    public interface ILegalizeableNumber<TValue> where TValue : struct
    {
        TValue GetLegalizedValue(TValue rawValue);
    }
}