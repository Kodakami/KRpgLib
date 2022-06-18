using KRpgLib.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Basic implementation of a unique repo of stats for when you just don't need anything fancy. Create this quickly with a SimpleStatRepoBuilder.
    /// </summary>
    public sealed class SimpleStatRepo : IUniqueRepo<Stat>
    {
        private readonly Dictionary<uint, Stat> _dict = new Dictionary<uint, Stat>();

        /// <summary>
        /// Create a new SimpleStatRepo. It is recommended you use SimpleStatRepoBuilder instead of calling this method directly.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public SimpleStatRepo(Dictionary<uint, Stat> newDictionaryToOwn)
        {
            _dict = newDictionaryToOwn;
        }

        public bool TryGetObject(uint uniqueID, out Stat obj)
        {
            return _dict.TryGetValue(uniqueID, out obj);
        }

        public bool TryGetUniqueID(Stat obj, out uint uniqueID)
        {
            var foundEntry = _dict.FirstOrDefault(kvp => kvp.Value.Equals(obj));
            if (foundEntry.Value != null)
            {
                uniqueID = foundEntry.Key;
                return true;
            }

            uniqueID = 0;
            return false;
        }
    }
    public sealed class SimpleStatRepoBuilder
    {
        private readonly Dictionary<uint, Stat> _dict = new Dictionary<uint, Stat>();

        private uint _nextID = 0;

        private bool _done = false;

        public SimpleStatRepoBuilder() { }
        private SimpleStatRepoBuilder(Dictionary<uint, Stat> dict, uint nextID, bool done)
        {
            _dict = dict;
            _nextID = nextID;
            _done = done;
        }

        public bool TryAddStat(Stat stat, out uint uniqueID)
        {
            if (!_dict.Values.Contains(stat) && !_done)
            {
                uniqueID = GetNewID();
                _dict.Add(uniqueID, stat);
                return true;
            }

            uniqueID = default;
            return false;
        }
        public void AddStat(Stat stat) => TryAddStat(stat, out _);

        private uint GetNewID()
        {
            if (_nextID == uint.MaxValue)
            {
                throw new System.InvalidOperationException($"Wow! You're trying to add the {(long)uint.MaxValue + 1}th unique stat to the repo! If this was intended, we recommend you write a custom implementation of IUniqueRepo<Stat> instead of using the simple one. It's just not powerful enough to handle your massive stat masterpiece...");
            }

            return _nextID++;
        }

        public SimpleStatRepo Build()
        {
            if (!_done)
            {
                _done = true;
                return new SimpleStatRepo(_dict);
            }
            else
            {
                throw new System.InvalidOperationException("You may not use the same SimpleStatRepoBuilder more than once. If you need to create multiple unique stat repos with the same contents, make a copy of the builder after all stats have been added (but before calling Build).");
            }
        }

        public SimpleStatRepoBuilder CreateCopy()
        {
            return new SimpleStatRepoBuilder(_dict, _nextID, _done);
        }
    }
}
