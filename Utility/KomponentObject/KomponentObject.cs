using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Utility.KomponentObject
{
    /// <summary>
    /// Base class for objects which manage components through a type-keyed dictionary. Similar to Unity's GameObject/Component system.
    /// </summary>
    /// <typeparam name="TKomponentBase">The base class for valid components, must implement IKomponent</typeparam>
    public abstract class KomponentObject<TKomponentBase> : IEnumerable<KeyValuePair<Type, IReadOnlyList<TKomponentBase>>>
        where TKomponentBase : IKomponent
    {
        private readonly Dictionary<Type, List<TKomponentBase>> _komponentDict = new Dictionary<Type, List<TKomponentBase>>();
        protected KomponentObject() { }
        protected KomponentObject(TKomponentBase komponent)
        {
            RegisterKomponent(komponent);
        }
        protected KomponentObject(IEnumerable<TKomponentBase> komponents)
        {
            foreach (var k in komponents ?? throw new ArgumentNullException(nameof(komponents)))
            {
                RegisterKomponent(k);
            }
        }
        protected KomponentObject(KomponentObject<TKomponentBase> other)
        {
            // Shallow copy.
            _komponentDict = new Dictionary<Type, List<TKomponentBase>>(other._komponentDict);
        }
        protected KomponentObject(IEnumerable<KomponentObject<TKomponentBase>> others)
        {
            foreach (var other in others)
            {
                foreach (var kvp in other)
                {
                    foreach (var komponent in kvp.Value)
                    {
                        // Will throw if required component has not been registered during the process. Don't use "required component" attributes if you're going to use this constructor.
                        RegisterKomponent(komponent);
                    }
                }
            }
        }

        protected void RegisterKomponent(TKomponentBase komponent)
        {
            /* 
             *  This method uses reflection to check attributes on the provided TKomponent.
             *  It is expected to be used between 100 and 10000 times (on startup) in the average program that uses it.
             *  This will add at most a handful of milliseconds to total startup time in exchange for ease of use and clarity.
             */

            // No null komponents.
            if (komponent == null)
            {
                throw new ArgumentNullException(nameof(komponent));
            }

            // Get the type of komponent.
            var argType = komponent.GetType();

            // No components of abstract base type.
            if (argType.Equals(typeof(TKomponentBase)))
            {
                throw new ArgumentException($"Komponent may not be of base type {argType}.");
            }

            // This value may be changed by an attribute later on down.
            bool allowMultipleInstances = false;

            // For each of the custom attributes on the komponent type.
            foreach (var att in argType.GetCustomAttributes(true))
            {
                // Grab a handle for the attribute's type.
                var attType = att.GetType();

                // If it is a "required component" attribute,
                if (attType.Equals(typeof(RequireKomponentAttribute)))
                {
                    // Cast to known type.
                    RequireKomponentAttribute rka = (RequireKomponentAttribute)att;

                    // If there is no key in the komponent dictionary (no required komponent registered at this time),
                    if (!_komponentDict.ContainsKey(rka.RequiredType))
                    {
                        // Throw out.
                        throw new ArgumentException($"Object has no registered komponents of required type: {rka.RequiredType}.");
                    }
                }
                // Otherwise if multiple instances are not yet allowed AND it is an "allow multiple instances" attribute,
                else if (!allowMultipleInstances && attType.Equals(typeof(AllowMultipleKomponentInstancesAttribute)))
                {
                    // Set the flag for that (and stop checking).
                    allowMultipleInstances = true;
                }
            }

            // If this type of komponent hasn't been registered yet (make a handle for the related list),
            if (!_komponentDict.TryGetValue(argType, out List<TKomponentBase> komponentList))
            {
                // Make a new list.
                komponentList = new List<TKomponentBase>();

                // Add it as a new entry in the dictionary.
                _komponentDict.Add(argType, komponentList);
            }
            // Otherwise (it has been defined),
            else
            {
                // If multiple instances are not allowed and there is already an instance.
                if (!allowMultipleInstances && komponentList.Count > 0)
                {
                    // Throw out.
                    // There may be a need to change this functionality in the future if it ends up being more practical to replace the old komponent instead.
                    throw new ArgumentException($"Object already contains a komponent of type \"{argType}\", and only one instance of this type is allowed.");
                }
            }

            // Add the provided component to the list.
            komponentList.Add(komponent);
        }
        protected void UnregisterKomponent(TKomponentBase komponent)
        {
            var key = komponent.GetType();

            if (_komponentDict.TryGetValue(key, out List<TKomponentBase> list))
            {
                list.Remove(komponent);

                if (list.Count == 0)
                {
                    _komponentDict.Remove(key);
                }
            }
        }
        public bool HasKomponent<TKomponent>() where TKomponent : TKomponentBase => _komponentDict.ContainsKey(typeof(TKomponent));
        public TKomponent GetKomponent<TKomponent>() where TKomponent : TKomponentBase
        {
            if (_komponentDict.TryGetValue(typeof(TKomponent), out List<TKomponentBase> found))
            {
                return (TKomponent)found[0];    // No need to check for empty list or null. Impossible internal state.
            }
            return default;
        }
        public IEnumerable<TKomponent> GetKomponents<TKomponent>() where TKomponent : TKomponentBase
        {
            if (_komponentDict.TryGetValue(typeof(TKomponent), out List<TKomponentBase> found))
            {
                return found.Cast<TKomponent>();
            }
            return new TKomponent[0];
        }
        public IEnumerable<TKomponentBase> GetAllKomponents()
        {
            return _komponentDict.SelectMany(kvp => kvp.Value);
        }
        public IReadOnlyDictionary<Type, IEnumerable<TKomponentBase>> GetAsReadOnlyDictionary()
        {
            var dict = new Dictionary<Type, IEnumerable<TKomponentBase>>();
            foreach (var kvp in this)
            {
                dict.Add(kvp.Key, kvp.Value);
            }
            return dict;
        }

        public IEnumerator<KeyValuePair<Type, IReadOnlyList<TKomponentBase>>> GetEnumerator()
        {
            return _komponentDict.Select(kvp => new KeyValuePair<Type, IReadOnlyList<TKomponentBase>>(kvp.Key, kvp.Value)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _komponentDict.GetEnumerator();
        }
    }
}
