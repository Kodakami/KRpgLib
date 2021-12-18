using System;
using System.Linq;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// Manager for stats registered with an instance of AlgoBuilder.
    /// </summary>
    public sealed class StatRegistry
    {
        private static string GetIllegalIdentifierMessage(string rejectedIdentifier) => $"\"{rejectedIdentifier}\" is not a legal identifier. Identifiers may not be null or empty, and may only contain letters and underscores(_).";

        private readonly Dictionary<string, Stat> _dict = new Dictionary<string, Stat>();

        /// <summary>
        /// Add a stat to the registry. Identifier may only consist of letters and underscores(_) (no numbers, other symbols, punctuation, white-space, or control characters). Throws ArgumentException if the identifier is invalid or taken by another stat.
        /// </summary>
        /// <param name="identifier">case-insensitive identifier for the stat</param>
        /// <param name="stat">stat represented by the identifier</param>
        public void Add(string identifier, Stat stat)
        {
            // Stat is null.
            if (stat == null)
            {
                throw new ArgumentNullException(nameof(stat));
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
            _dict.Add(identifierToLower, stat);
        }
        /// <summary>
        /// Tries to find the stat indicated by the provided identifier.
        /// </summary>
        /// <param name="identifier">case-insensitive identifier for the desired stat</param>
        /// <param name="stat">the found stat</param>
        /// <returns>true if a stat is registered with the provided identifier</returns>
        public bool TryGetStat(string identifier, out Stat stat)
        {
            if (!IdentifierIsLegal(identifier))
            {
                throw new ArgumentException(GetIllegalIdentifierMessage(identifier));
            }

            return _dict.TryGetValue(identifier.ToLowerInvariant(), out stat);
        }
        private bool IdentifierIsLegal(string identifier)
        {
            // Null and empty checks.
            if (string.IsNullOrEmpty(identifier))
            {
                return false;
            }

            // Returns true if all characters of the keyword string are letters (no symbols, no white-space, no numbers, no control chars).
            return identifier.All(c => char.IsLetter(c) || c == '_');
        }
        private bool IdentifierExists_CaseInsensitive(string identifier)
        {
            return _dict.ContainsKey(identifier.ToLowerInvariant());
        }
    }
}
