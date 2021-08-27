using KRpgLib.Utility.KomponentObject;
using System.Collections.Generic;

namespace KRpgLib.Affixes
{
    /// <summary>
    /// Collects and manipulates components whose data is considered "moddable". A moddable object should be able to supply one of these in order to interact with the Affix system.
    /// </summary>
    public class ModdableDataManager : KomponentObject<IModdableDataSet>
    {
        public ModdableDataManager() { }
        public ModdableDataManager(IModdableDataSet komponent) : base(komponent) { }
        public ModdableDataManager(IEnumerable<IModdableDataSet> allModdableDataSets) : base(allModdableDataSets) { }
    }
}
