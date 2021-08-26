namespace KRpgLib.Affixes
{
    /// <summary>
    /// Interface for something that should be moddable by the affix system.
    /// </summary>
    public interface IModdable
    {
        TData GetModdableDataSet<TData>() where TData : IModdableDataSet;
    }
}
