using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    public sealed class StatTemplateRegistry<TValue> where TValue : struct
    {
        private readonly Dictionary<string, IStatTemplate<TValue>> _dict = new Dictionary<string, IStatTemplate<TValue>>();
        public void Add(string identifier, IStatTemplate<TValue> statTemplate)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Argument may not be null, an empty string, or consist only of white-space characters.", nameof(identifier));
            }
            if (statTemplate == null)
            {
                throw new ArgumentNullException(nameof(statTemplate));
            }
            // Identifier character legality check.
            if (!IdentifierIsLegal(identifier))
            {
                throw new ArgumentException($"\"{identifier}\" is not a legal identifier. Identifiers may only contain letters.");
            }

            string identifierToLower = identifier.ToLowerInvariant();

            // Identifier taken check.
            if (IdentifierExists_CaseInsensitive(identifier))
            {
                throw new ArgumentException($"Identifier is already in use (\"{identifierToLower}\"). Identifiers are not case-sensitive.");
            }

            // Ran the gauntlet.
            _dict.Add(identifierToLower, statTemplate);
        }
        public bool TryGetStatTemplate(string identifier, out IStatTemplate<TValue> statTemplate)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("Argument may not be null, an empty string, or consist only of white-space characters.", nameof(identifier));
            }

            return _dict.TryGetValue(identifier.ToLowerInvariant(), out statTemplate);
        }
        private bool IdentifierIsLegal(string identifier)
        {
            // Returns true if each letter of the keyword string is a letter (no symbols, no space, no numbers, no control chars).
            // Slightly slower then doing the loops myself, but so much nicer-looking.
            return new List<char>(identifier).TrueForAll(c => char.IsLetter(c));
        }
        private bool IdentifierExists_CaseInsensitive(string identifier)
        {
            return _dict.ContainsKey(identifier.ToLowerInvariant());
        }
    }
}
