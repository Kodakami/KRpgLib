using KRpgLib.Utility;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Stats
{
    /// <summary>
    /// Basic implementation of a unique repo when you just don't need anything fancy. Create this quickly with a SimpleUniqueRepoBuilder<T>.
    /// </summary>
    public sealed class SimpleUniqueRepo<T> : IUniqueRepo<T>
    {
        private readonly Dictionary<uint, T> _dict = new Dictionary<uint, T>();

        /// <summary>
        /// Create a new SimpleUniqueRepo. It is recommended you use SimpleUniqueRepoBuilder instead of calling this method directly.
        /// </summary>
        /// <param name="keyValuePairs"></param>
        public SimpleUniqueRepo(Dictionary<uint, T> newDictionaryToOwn)
        {
            _dict = newDictionaryToOwn;
        }

        public bool TryGetObject(uint uniqueID, out T obj)
        {
            return _dict.TryGetValue(uniqueID, out obj);
        }

        public bool TryGetUniqueID(T obj, out uint uniqueID)
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

    public sealed class SimpleUniqueRepoBuilder<T>
    {
        private readonly Dictionary<uint, T> _dict = new Dictionary<uint, T>();

        private uint _nextID = 0;

        private bool _done = false;

        public SimpleUniqueRepoBuilder() { }
        private SimpleUniqueRepoBuilder(Dictionary<uint, T> dict, uint nextID, bool done)
        {
            _dict = dict;
            _nextID = nextID;
            _done = done;
        }

        public bool TryAddObject(T obj, out uint uniqueID)
        {
            if (!_dict.Values.Contains(obj) && !_done)
            {
                uniqueID = GetNewID();
                _dict.Add(uniqueID, obj);
                return true;
            }

            uniqueID = default;
            return false;
        }
        public void AddObject(T obj) => TryAddObject(obj, out _);

        private uint GetNewID()
        {
            if (_nextID == uint.MaxValue)
            {
                throw new System.InvalidOperationException($"Wow! You're trying to add the {(long)uint.MaxValue + 1}th unique object to the repo! If this was intended, we recommend you write a custom implementation of IUniqueRepo<{typeof(T).Name}> instead of using the simple one. It's just not powerful enough to handle your massive masterpiece...");
            }

            return _nextID++;
        }

        public SimpleUniqueRepo<T> Build()
        {
            if (!_done)
            {
                _done = true;
                return new SimpleUniqueRepo<T>(_dict);
            }
            else
            {
                throw new System.InvalidOperationException("You may not use the same SimpleUniqueRepoBuilder more than once. If you need to create multiple unique repos with the same contents, make a copy of the builder after all objects have been added (but before calling Build).");
            }
        }

        public SimpleUniqueRepoBuilder<T> CreateCopy()
        {
            return new SimpleUniqueRepoBuilder<T>(_dict, _nextID, _done);
        }
    }
}
