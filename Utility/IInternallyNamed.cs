namespace KRpgLib.Utility
{
    /// <summary>
    /// Interface for objects which have internal string identifiers. Used for looking up external names or the like from a localization system.
    /// </summary>
    public interface IInternallyNamed
    {
        string InternalName { get; }
    }
}
