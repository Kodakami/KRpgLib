using System;
using System.Collections.Generic;
using KRpgLib.Utility;
using System.Linq;

namespace KRpgLib.Stats
{
    public class StatDeltaCollection<TValue> where TValue : struct
    {
        // Internal fields.
        private readonly Dictionary<IStatTemplate<TValue>, DeltaTypeDictHelper> _controllerDict;

        // Ctors.
        public StatDeltaCollection() { }
        public StatDeltaCollection(StatDeltaCollection<TValue> otherForDeepCopy)
        {
            if (otherForDeepCopy == null)
            {
                throw new ArgumentNullException(nameof(otherForDeepCopy));
            }

            _controllerDict = new Dictionary<IStatTemplate<TValue>, DeltaTypeDictHelper>();
            foreach (var kvp in otherForDeepCopy._controllerDict)
            {
                _controllerDict.Add(kvp.Key, new DeltaTypeDictHelper(kvp.Value));
            }
        }
        public StatDeltaCollection(IEnumerable<StatDeltaCollection<TValue>> combineFrom)
        {
            if (combineFrom == null)
            {
                throw new ArgumentNullException(nameof(combineFrom));
            }

            // Use that dictionary smasher!
            _controllerDict = DictionarySmasher<IStatTemplate<TValue>, DeltaTypeDictHelper>.Smash(
                valueSmasher: (_, values) => new DeltaTypeDictHelper(values),
                dictionaries: combineFrom.Select(collection => collection._controllerDict).ToList());
        }
        public StatDeltaCollection(IEnumerable<StatTemplateAndDelta<TValue>> statTemplateAndDeltas)
        {
            if (statTemplateAndDeltas == null)
            {
                throw new ArgumentNullException(nameof(statTemplateAndDeltas));
            }

            _controllerDict = new Dictionary<IStatTemplate<TValue>, DeltaTypeDictHelper>();
            foreach (var stad in statTemplateAndDeltas)
            {
                Add_Internal(stad.Template, stad.DeltaType, stad.DeltaValue);
            }
        }

        // Protected methods.
        protected void Add_Internal(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType, TValue deltaValue)
        {
            if (!_controllerDict.ContainsKey(statTemplate))
            {
                _controllerDict.Add(statTemplate, new DeltaTypeDictHelper());
            }

            _controllerDict[statTemplate].Add(deltaType, deltaValue);
        }
        protected void Remove_Internal(IStatTemplate<TValue> statTemplate, StatDeltaType<TValue> deltaType, TValue deltaValue)
        {
            if (_controllerDict.ContainsKey(statTemplate))
            {
                var controller = _controllerDict[statTemplate];
                controller.Remove(deltaType, deltaValue);

                if (!controller.HasValues)
                {
                    _controllerDict.Remove(statTemplate);
                }
            }
        }

        // Public methods.
        public StatSnapshot<TValue> GetStatSnapshot()
        {
            var dict = new Dictionary<IStatTemplate<TValue>, TValue>();
            foreach (var kvp in _controllerDict)
            {
                // Get total stat deltas for this stat template.
                var deltaDict = kvp.Value.GetCacheCopy();

                // Start with the default value.
                TValue statValue = kvp.Key.DefaultValue;

                // For each type of stat delta (addition, multiplication)...
                foreach (StatDeltaType<TValue> deltaType in StatDeltaType<TValue>.GetAllByPriority())
                {
                    // If there is a delta of that type...
                    if (deltaDict.TryGetValue(deltaType, out TValue found))
                    {
                        // Get the combined value (combining baseline value and total delta value).
                        TValue combinedDeltaValues = deltaType.Combine(deltaType.BaselineValue, found);

                        // Apply the total delta value to the current stat value.
                        statValue = deltaType.Apply(statValue, combinedDeltaValues);
                    }
                }

                // Add the total stat value to the dictionary.
                dict.Add(kvp.Key, statValue);
            }

            return StatSnapshot<TValue>.Create(dict);
        }

        // Internal classes.
        protected sealed class DeltaTypeDictHelper : CachedValueController<Dictionary<StatDeltaType<TValue>, TValue>>
        {
            private readonly Dictionary<StatDeltaType<TValue>, ValueListHelper> _controllerDict;

            public bool HasValues => _controllerDict.Count > 0;

            public DeltaTypeDictHelper() { }
            public DeltaTypeDictHelper(DeltaTypeDictHelper otherForDeepCopy)
            {
                _controllerDict = new Dictionary<StatDeltaType<TValue>, ValueListHelper>();
                foreach (var kvp in otherForDeepCopy._controllerDict)
                {
                    _controllerDict.Add(kvp.Key, new ValueListHelper(kvp.Value));
                }
            }
            public DeltaTypeDictHelper(IEnumerable<DeltaTypeDictHelper> combineFrom)
            {
                // Use that dictionary smasher!
                _controllerDict = DictionarySmasher<StatDeltaType<TValue>, ValueListHelper>.Smash(
                    valueSmasher: (key, values) => new ValueListHelper(key, values),
                    dictionaries: combineFrom.Select(controller => controller._controllerDict).ToList());
            }

            public void Add(StatDeltaType<TValue> deltaType, TValue deltaValue)
            {
                if (!_controllerDict.ContainsKey(deltaType))
                {
                    _controllerDict.Add(deltaType, new ValueListHelper(deltaType));
                }

                _controllerDict[deltaType].Add(deltaValue);

                SetDirty();
            }
            public void Add(StatDelta<TValue> statDelta)
            {
                Add(statDelta.Type, statDelta.Value);
            }

            public void Remove(StatDeltaType<TValue> deltaType, TValue deltaValue)
            {
                if (_controllerDict.ContainsKey(deltaType))
                {
                    var controller = _controllerDict[deltaType];

                    controller.Remove(deltaValue);

                    if (!controller.HasValues)
                    {
                        _controllerDict.Remove(deltaType);
                    }

                    SetDirty();
                }
            }
            public void Remove(StatDelta<TValue> statDelta)
            {
                Remove(statDelta.Type, statDelta.Value);
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

            private sealed class ValueListHelper : CachedValueController<TValue, StatDeltaType<TValue>>
            {
                private readonly List<TValue> _valueList = new List<TValue>();
                private StatDeltaType<TValue> RepresentedType => Context;

                public bool HasValues => _valueList.Count > 0;

                public ValueListHelper(StatDeltaType<TValue> representedType) : base(representedType) { }
                public ValueListHelper(ValueListHelper otherForDeepCopy)
                    :base(otherForDeepCopy.Context)
                {
                    _valueList = new List<TValue>(otherForDeepCopy._valueList);
                }
                public ValueListHelper(StatDeltaType<TValue> representedType, IEnumerable<ValueListHelper> combineFrom)
                    :base(representedType)
                {
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
                    TValue total = RepresentedType.BaselineValue;
                    foreach (var val in _valueList)
                    {
                        RepresentedType.Combine(total, val);
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
}