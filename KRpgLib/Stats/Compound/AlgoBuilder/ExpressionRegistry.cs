using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// Pop parameters from the stack (left to right) and push the resulting expression object.
    /// </summary>
    /// <returns>expression object</returns>
    public delegate void ExpressionObjectBuildAction<TValue>(AbstractParser<TValue> parser) where TValue : struct;
    public sealed class ExpressionInfo<TValue> where TValue : struct
    {
        public string ExpressionName { get; }
        public List<string> Keywords { get; }
        public ExpressionObjectBuildAction<TValue> ExpressionObjectBuildAction { get; }

        public ExpressionInfo(string expressionName, List<string> keywords, ExpressionObjectBuildAction<TValue> expressionObjectBuilder)
        {
            ExpressionName = expressionName;
            Keywords = keywords;
            ExpressionObjectBuildAction = expressionObjectBuilder;
        }
    }
    public sealed class ExpressionRegistry<TValue> where TValue : struct
    {
        private readonly List<ExpressionInfo<TValue>> _list;

        public ExpressionRegistry()
        {
            _list = new List<ExpressionInfo<TValue>>();
        }
        public ExpressionRegistry(params ExpressionInfo<TValue>[] uncheckedExpressions)
        {
            _list = new List<ExpressionInfo<TValue>>();
            _list.AddRange(uncheckedExpressions);
        }
        public bool KeywordExists(string keyword)
        {
            // Returns true if any of the keywords for any of the expressions is a match (ignoring case).
            return _list.Exists(ei => ei.Keywords.Exists(str => str.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)));
        }
        private bool ExpressionNameExists(string tokenName)
        {
            return _list.Exists(ei => ei.ExpressionName.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase));
        }
        public void Add(string expressionName, List<string> keywords, ExpressionObjectBuildAction<TValue> expressionObjectBuilder)
        {
            // Expression name null, empty, white-space check.
            if (string.IsNullOrWhiteSpace(expressionName))
            {
                throw new ArgumentException("Argument cannot be null, empty, or consist only of white-space characters.", nameof(expressionName));
            }
            // Expression name taken check.
            if (ExpressionNameExists(expressionName))
            {
                throw new ArgumentException($"Token name already in use (\"{expressionName}\").");
            }
            // Keywords list null or empty check.
            if (keywords == null || keywords.Count == 0)
            {
                throw new ArgumentException("Argument cannot be null or empty.", nameof(keywords));
            }
            // For each keyword in list...
            foreach (var keyword in keywords)
            {
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
            // Expression object builder function is null.
            if (expressionObjectBuilder == null)
            {
                throw new ArgumentNullException(nameof(expressionObjectBuilder));
            }

            // Ran the gauntlet.
            _list.Add(new ExpressionInfo<TValue>(expressionName, keywords, expressionObjectBuilder));
        }
        public bool TryGetExpressionInfo(string keyword, out ExpressionInfo<TValue> expressionInfo)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                throw new ArgumentException("Argument may not be null, empty, or consist only of white-space characters.", nameof(keyword));
            }

            // Return the corresponding action to a given keyword.
            expressionInfo = _list.Find(ei => ei.Keywords.Exists(k => k.Equals(keyword, StringComparison.InvariantCultureIgnoreCase)));
            return expressionInfo != null;
        }

        private bool KeywordIsLegal(string keyword)
        {
            // Returns true if each letter of the keyword string is a letter (no symbols, no space, no numbers, no control chars).
            // Slightly slower then doing the loops myself, but so much nicer-looking.
            return new List<char>(keyword).TrueForAll(c => char.IsLetter(c));
        }
    }
}
