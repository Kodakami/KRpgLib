using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using System.Linq;

namespace KRpgLib.Stats
{
    public sealed class StatDeltaCollection<TValue> : IStatProvider<TValue> where TValue : struct
    {
        private readonly Dictionary<IStatTemplate<TValue>, DeltaTypeController> _controllerDict = new Dictionary<IStatTemplate<TValue>, DeltaTypeController>();

        public StatDeltaCollection() { }
        public StatDeltaCollection(StatDeltaCollection<TValue> otherForDeepCopy)
        {
            _controllerDict = new Dictionary<IStatTemplate<TValue>, DeltaTypeController>();
            foreach (var kvp in otherForDeepCopy._controllerDict)
            {
                _controllerDict.Add(kvp.Key, new DeltaTypeController(kvp.Value));
            }
        }
        public StatDeltaCollection(IEnumerable<StatDeltaCollection<TValue>> combineFrom)
        {
            // Use that dictionary smasher!
            _controllerDict = DictionarySmasher<IStatTemplate<TValue>, DeltaTypeController>.Smash(
                valueSmasher: (_, values) => new DeltaTypeController(values),
                dictionaries: combineFrom.Select(controller => controller._controllerDict));
        }

        public void Add(StatTemplateAndDelta<TValue> statTemplateAndDelta)
        {
            if (!_controllerDict.ContainsKey(statTemplateAndDelta.Template))
            {
                _controllerDict.Add(statTemplateAndDelta.Template, new DeltaTypeController());
            }

            _controllerDict[statTemplateAndDelta.Template].Add(statTemplateAndDelta.Delta);
        }
        public void Remove(StatTemplateAndDelta<TValue> statTemplateAndDelta)
        {
            if (_controllerDict.ContainsKey(statTemplateAndDelta.Template))
            {
                var controller = _controllerDict[statTemplateAndDelta.Template];
                controller.Remove(statTemplateAndDelta.Delta);

                if (!controller.HasValues)
                {
                    _controllerDict.Remove(statTemplateAndDelta.Template);
                }
            }
        }
        public bool HasDeltasForStat(IStatTemplate<TValue> statTemplate)
        {
            return _controllerDict.ContainsKey(statTemplate);
        }
        public List<StatDelta<TValue>> GetDeltasForStat(IStatTemplate<TValue> statTemplate)
        {
            var outList = new List<StatDelta<TValue>>();
            if (_controllerDict.ContainsKey(statTemplate))
            {
                foreach (var kvp in _controllerDict[statTemplate].GetCacheCopy())
                {
                    outList.Add(new StatDelta<TValue>(kvp.Key, kvp.Value));
                }
            }
            return outList;
        }
        public List<IStatTemplate<TValue>> GetStatsWithDeltas()
        {
            return new List<IStatTemplate<TValue>>(_controllerDict.Keys);
        }

        public StatDeltaCollection<TValue> GetDeepCopy() => new StatDeltaCollection<TValue>(this);
        public ReadOnlyStatDeltaCollection<TValue> GetReadOnlyDeepCopy() => new ReadOnlyStatDeltaCollection<TValue>(this);

        private sealed class DeltaTypeController : CachedValueController<Dictionary<StatDeltaType<TValue>,TValue>>
        {
            private readonly Dictionary<StatDeltaType<TValue>, ValueListController> _controllerDict;

            public bool HasValues => _controllerDict.Count > 0;

            public DeltaTypeController() { }
            public DeltaTypeController(DeltaTypeController otherForDeepCopy)
            {
                _controllerDict = new Dictionary<StatDeltaType<TValue>, ValueListController>();
                foreach (var kvp in otherForDeepCopy._controllerDict)
                {
                    _controllerDict.Add(kvp.Key, new ValueListController(kvp.Value));
                }
            }
            public DeltaTypeController(IEnumerable<DeltaTypeController> combineFrom)
            {
                // Use that dictionary smasher!
                _controllerDict = DictionarySmasher<StatDeltaType<TValue>, ValueListController>.Smash(
                    valueSmasher: (key, values) => new ValueListController(key, values),
                    dictionaries: combineFrom.Select(controller => controller._controllerDict));
            }

            public void Add(StatDelta<TValue> statDelta)
            {
                if (!_controllerDict.ContainsKey(statDelta.Type))
                {
                    _controllerDict.Add(statDelta.Type, new ValueListController(statDelta.Type));
                }

                _controllerDict[statDelta.Type].Add(statDelta.Value);

                SetDirty();
            }

            public void Remove(StatDelta<TValue> statDelta)
            {
                if (_controllerDict.ContainsKey(statDelta.Type))
                {
                    var controller = _controllerDict[statDelta.Type];
                    controller.Remove(statDelta.Value);
                    if (!controller.HasValues)
                    {
                        _controllerDict.Remove(statDelta.Type);
                    }

                    SetDirty();
                }
            }

            protected override Dictionary<StatDeltaType<TValue>, TValue> CalculateNewCache()
            {
                var newDeltaDict = new Dictionary<StatDeltaType<TValue>, TValue>();
                foreach (var controller in _controllerDict)
                {
                    TValue totalDelta = controller.Value.GetCacheCopy();
                    newDeltaDict.Add(controller.Key, totalDelta);
                }
                return newDeltaDict;
            }

            protected override Dictionary<StatDeltaType<TValue>, TValue> CreateCacheCopy(Dictionary<StatDeltaType<TValue>, TValue> cache)
            {
                return new Dictionary<StatDeltaType<TValue>, TValue>(cache);
            }

            private sealed class ValueListController : CachedValueController<TValue>
            {
                private readonly List<TValue> _valueList = new List<TValue>();
                private readonly StatDeltaType<TValue> _representedType;

                public bool HasValues => _valueList.Count > 0;

                public ValueListController(StatDeltaType<TValue> representedType)
                {
                    _representedType = representedType;
                }
                public ValueListController(ValueListController otherForDeepCopy)
                {
                    _representedType = otherForDeepCopy._representedType;
                    _valueList = new List<TValue>(otherForDeepCopy._valueList);
                }
                public ValueListController(StatDeltaType<TValue> representedType, IEnumerable<ValueListController> combineFrom)
                {
                    _representedType = representedType;
                    _valueList = combineFrom.SelectMany(controller => controller._valueList).ToList();
                }

                public void Add(TValue value)
                {
                    _valueList.Add(value);
                    SetDirty();
                }

                public void Remove(TValue value)
                {
                    _valueList.Remove(value);
                    SetDirty();
                }

                protected override TValue CalculateNewCache()
                {
                    TValue total = _representedType.BaselineValue;
                    foreach (var val in _valueList)
                    {
                        _representedType.Combine(total, val);
                    }
                    return total;
                }

                protected override TValue CreateCacheCopy(TValue cache)
                {
                    // Since TValue is a struct.
                    return cache;
                }
            }
        }
    }
    public sealed class ReadOnlyStatDeltaCollection<TValue>: IStatProvider<TValue> where TValue : struct
    {
        private readonly StatDeltaCollection<TValue> _statDeltaCollection;

        /// <summary>
        /// This constructor creates a completely empty collection with no means of modifying the contents.
        /// </summary>
        public ReadOnlyStatDeltaCollection()
        {
            _statDeltaCollection = new StatDeltaCollection<TValue>();
        }
        /// <summary>
        /// Creates a new read-only collection and initializes it with the supplied values. It will only ever contain these values.
        /// </summary>
        /// <param name="statTemplateAndDeltas">initialization values</param>
        public ReadOnlyStatDeltaCollection(IEnumerable<StatTemplateAndDelta<TValue>> statTemplateAndDeltas)
        {
            _statDeltaCollection = new StatDeltaCollection<TValue>();
            foreach (var stad in statTemplateAndDeltas)
            {
                _statDeltaCollection.Add(stad);
            }
        }
        /// <summary>
        /// Creates a deep copy. Values will only reflect what the collection contained at the time of instantiation.
        /// </summary>
        /// <param name="statDeltaCollection">a stat delta collection to copy and encapsulate</param>
        public ReadOnlyStatDeltaCollection(StatDeltaCollection<TValue> statDeltaCollection)
        {
            // Deep copy.
            _statDeltaCollection = new StatDeltaCollection<TValue>(statDeltaCollection);
        }

        public List<StatDelta<TValue>> GetDeltasForStat(IStatTemplate<TValue> stat) => _statDeltaCollection.GetDeltasForStat(stat);
        public List<IStatTemplate<TValue>> GetStatsWithDeltas() => _statDeltaCollection.GetStatsWithDeltas();
        public bool HasDeltasForStat(IStatTemplate<TValue> stat) => _statDeltaCollection.HasDeltasForStat(stat);
    }
}
