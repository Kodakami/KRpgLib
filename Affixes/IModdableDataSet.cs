using KRpgLib.Utility.KomponentObject;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Interface for some arbitrary data that can be manipulated by the Affix system. A moddable object should be able to supply a ModdableDataManager which collects and manipulates these in order to interact with the system.
    /// </summary>
    public interface IModdableDataSet : IKomponent { }
}
