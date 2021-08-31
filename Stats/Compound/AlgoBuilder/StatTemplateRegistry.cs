using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// Manager for stat templates registered with an instance of AlgoBuilder.
    /// </summary>
    /// <typeparam name="TValue">stat backing type</typeparam>
    public sealed class StatTemplateRegistry<TValue> where TValue : struct
    {
        private static string GetIllegalIdentifierMessage(string rejectedIdentifier) => $"\"{rejectedIdentifier}\" is not a legal identifier. Identifiers may not be null or empty, and may only contain letters (no white-space characters or numbers).";

        private readonly Dictionary<string, IStat<TValue>> _dict = new Dictionary<string, IStat<TValue>>();

        /// <summary>
        /// Add a stat template to the registry. Identifier must consist only of letters (no numbers, symbols, punctuation, white-space, or control characters). Throws ArgumentException if the identifier is invalid or taken by another stat template.
        /// </summary>
        /// <param name="identifier">case-insensitive identifier for the stat template</param>
        /// <param name="statTemplate">stat template represented by the identifier</param>
        public void Add(string identifier, IStat<TValue> statTemplate)
        {
            // Stat template is null.
            if (statTemplate == null)
            {
                throw new ArgumentNullException(nameof(statTemplate));
            }

            // Identifier legality check.
            if (!IdentifierIsLegal(identifier))
            {
                throw new ArgumentException(GetIllegalIdentifierMessage(identifier));
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
        /// <summary>
        /// Tries to find the stat template indicated by the provided identifier.
        /// </summary>
        /// <param name="identifier">case-insensitive identifier for the desired stat template</param>
        /// <param name="statTemplate">the found stat template</param>
        /// <returns>true if a stat template is registered with the provided identifier</returns>
        public bool TryGetStatTemplate(string identifier, out IStat<TValue> statTemplate)
        {
            if (!IdentifierIsLegal(identifier))
            {
                throw new ArgumentException(GetIllegalIdentifierMessage(identifier));
            }

            return _dict.TryGetValue(identifier.ToLowerInvariant(), out statTemplate);
        }
        private bool IdentifierIsLegal(string identifier)
        {
            // Null and empty checks.
            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            // Returns true if each letter of the keyword string is a letter (no symbols, no white-space, no numbers, no control chars).
            // Slightly slower than doing the loops myself, but so much nicer-looking.
            return new List<char>(identifier).TrueForAll(c => char.IsLetter(c));
        }
        private bool IdentifierExists_CaseInsensitive(string identifier)
        {
            return _dict.ContainsKey(identifier.ToLowerInvariant());
        }
    }
}
