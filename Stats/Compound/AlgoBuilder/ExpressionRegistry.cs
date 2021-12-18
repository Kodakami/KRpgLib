using System;
using System.Collections.Generic;
using System.Linq;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// Manager for types of expressions registered with an instance of AlgoBuilder.
    /// </summary>
    public sealed class ExpressionRegistry
    {
        private readonly List<ExpressionInfo> _list = new List<ExpressionInfo>();

        /// <summary>
        /// Returns true if any of the keywords for any of the expressions is a match (ignoring case).
        /// </summary>
        public bool KeywordExists(string keyword)
        {
            // Returns true if any of the keywords for any of the expressions is a match (ignoring case).
            return _list.Exists(ei => ei.Keywords.Any(str => str.Equals(keyword, StringComparison.OrdinalIgnoreCase)));
        }
        /// <summary>
        /// Add an expression to the registry. Throws ArgumentException if any of the keywords are taken by another expression.
        /// </summary>
        /// <param name="keywords">list of case-insensitive keyword identifiers for the expression</param>
        /// <param name="popFunc">function for popping parameters off of the parser stack</param>
        /// <param name="expressionCtor">function for constructing a new object given a parameter queue</param>
        public void Add(IEnumerable<string> keywords, PopParamsFunc popFunc, ExpressionCtorDelegate expressionCtor)
        {
            // Pop function is null.
            if (popFunc == null)
            {
                throw new ArgumentNullException(nameof(popFunc));
            }
            // Expression constructor (push function) is null.
            if (expressionCtor == null)
            {
                throw new ArgumentNullException(nameof(expressionCtor));
            }
            // Keywords list null check.
            if (keywords == null)
            {
                throw new ArgumentNullException(nameof(keywords));
            }
            // For each keyword in list...
            foreach (var keyword in keywords)
            {
                // Null/empty check
                if (string.IsNullOrEmpty(keyword))
                {
                    throw new ArgumentException("Argument may not contain null or empty keywords.", nameof(keywords));
                }
                // Keyword character legality check.
                if (!KeywordIsLegal(keyword))
                {
                    throw new ArgumentException($"\"{keyword}\" is not a legal keyword. Keywords may only contain letters.");
                }
                // Keyword taken check.
                if (KeywordExists(keyword))
                {
                    throw new ArgumentException($"Keyword is already in use (\"{keyword}\").");
                }
            }

            // Ran the gauntlet.
            _list.Add(new ExpressionInfo(keywords, popFunc, expressionCtor));
        }
        private bool TryGetExpressionInfo(string keyword, out ExpressionInfo expressionInfo)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                expressionInfo = null;
                return false;
            }

            // Return the corresponding expression info for a given keyword.
            expressionInfo = _list.Find(ei => ei.Keywords.Any(k => k.Equals(keyword, StringComparison.OrdinalIgnoreCase)));

            return expressionInfo != null;
        }
        private bool KeywordIsLegal(string keyword)
        {
            // Returns true if each letter of the keyword string is a letter (no symbols, no space, no numbers, no control chars).
            return keyword.All(c => char.IsLetter(c));
        }
        /// <summary>
        /// Tries to find a registered expression by keyword, pop its parameters, construct the object, and push it to the parser's stack. Sets message to description of failure (or NULL if successful).
        /// </summary>
        /// <param name="keyword">expression keyword</param>
        /// <param name="parser">AbstractParser<YourBackingType> to manipulate</param>
        /// <param name="message">description of failure or NULL</param>
        /// <returns>true if successful</returns>
        public bool TryBuildAndPushExpression(string keyword, Parser parser, out string message)
        {
            if (TryGetExpressionInfo(keyword, out ExpressionInfo expressionInfo))
            {
                if (expressionInfo.TryBuildAndPushExpression(parser))
                {
                    message = null;
                    return true;
                }
                message = $"Expression \"{keyword}\" did not process correctly. This is a problem with the expression itself.";
                return false;
            }
            message = $"\"{keyword}\" is not a registered expression keyword.";
            return false;
        }
    }
}
