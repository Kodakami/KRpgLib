using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// Base class for Algo parsers. Make a new subclass of this if your stat system uses non-int, non-float backing values. Override TryParserNumber() with your backing value's string-to-number parsing function.
    /// </summary>
    /// <typeparam name="TValue">stat backing value</typeparam>
    public abstract class AbstractParser<TValue> where TValue : struct
    {
        protected List<Token> Tokens { get; }
        protected int CurrentTokenIndex { get; private set; }   // Don't set this to Tokens.Count, use Count - 1.
        protected ExpressionRegistry<TValue> ExpressionRegistry { get; }
        protected StatTemplateRegistry<TValue> StatTemplateRegistry { get; }
        protected Stack<object> TheStack { get; } = new Stack<object>();

        /// <summary>
        /// The state of the parser after the last call to TryParseTokens().
        /// </summary>
        public Result ParseResult { get; private set; } = Result.UNDEFINED;
        /// <summary>
        /// The status message from the parser after the last call to TryParseTokens().
        /// </summary>
        public string StatusMessage { get; private set; } = "Incomplete.";

        protected AbstractParser(List<Token> tokens, ExpressionRegistry<TValue> expressionRegistry, StatTemplateRegistry<TValue> statTemplateRegistry)
        {
            Tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            ExpressionRegistry = expressionRegistry ?? throw new ArgumentNullException(nameof(expressionRegistry));
            StatTemplateRegistry = statTemplateRegistry ?? throw new ArgumentNullException(nameof(statTemplateRegistry));
        }
        public bool TryParseTokens(out ValueExpression<TValue> algo)
        {
            algo = null;
            ParseResult = Result.UNDEFINED;
            StatusMessage = "Incomplete.";

            // Reset to end.
            CurrentTokenIndex = Tokens.Count - 1;

            // We parse backwards.
            // Each loop decrements CurrentTokenIndex until it falls below 0.
            for (; CurrentTokenIndex >= 0;
                CurrentTokenIndex--)
            {
                Token current = Tokens[CurrentTokenIndex];

                switch (current.TokenType)
                {
                    case TokenType.NUMBER:
                        if (!DoPushNumber(current))
                        {
                            return false;
                        }
                        break;
                    case TokenType.CASH:
                        if (!DoPushStatLiteral(false))
                        {
                            return false;
                        }
                        break;
                    case TokenType.ASTERISK:
                        if (!DoPushStatLiteral(true))
                        {
                            return false;
                        }
                        break;
                    case TokenType.IDENTIFIER:
                        if (!DoPushIdentifier(current))
                        {
                            return false;
                        }
                        break;
                    case TokenType.PAREN_OPEN:
                        if (!DoBeginExpression())
                        {
                            return false;
                        }
                        break;
                    case TokenType.PAREN_CLOSED:
                        if (!DoEndExpression())
                        {
                            return false;
                        }
                        break;
                    default:
                        Error($"Unhandled token type (\"{Enum.GetNames(typeof(TokenType))[(int)current.TokenType]}\"). This is an issue with the scanner or the parser, not the script.");
                        return false;
                }
            }

            if (!TryPopAsType("Value expression", out ValueExpression<TValue> finalValueExpression))
            {
                Error("Expression tree does not resolve to a value expression.");
                return false;
            }

            if (TheStack.Count > 1)
            {
                Error("Script ended with leftover objects on the stack. Check for extraneous characters at end.");
                return false;
            }

            StatusMessage = "Parse completed without errors.";
            ParseResult = Result.OK;
            algo = finalValueExpression;
            return true;
        }
        /// <summary>
        /// Call this when your custom parser functions encounter an unrecoverable error, then return false from the function if applicable.
        /// </summary>
        /// <param name="message">error message</param>
        protected void Error(string message)
        {
            string currentIndexStr = CurrentTokenIndex >= 0 && CurrentTokenIndex < Tokens.Count ? Tokens[CurrentTokenIndex].CharIndex.ToString() : "[Invalid index]";
            StatusMessage = $"Parse error at index {currentIndexStr}. {message}";
            ParseResult = Result.FAIL;
        }
        private bool TryPop(out object obj)
        {
            try
            {
                obj = TheStack.Pop();
                return true;
            }
            catch (InvalidOperationException)
            {
                // Stack is empty.
                obj = null;
                return false;
            }
        }
        private bool TryCastAsType<T>(object obj, out T value)
        {
            try
            {
                value = (T)obj;
            }
            catch (InvalidCastException)
            {
                // Object was not of the correct type.

                value = default;
                return false;
            }

            return true;
        }
        private bool DoPushNumber(Token token)
        {
            string literalString;
            try
            {
                literalString = (string)token.Literal;
            }
            catch (InvalidCastException)
            {
                Error("Number token literal was not a string. This is an issue with the scanner itself, not the script.");
                return false;
            }

            if (!TryParseNumber(literalString, out TValue value))
            {
                Error($"Unable to parse number ({literalString}).");
                return false;
            }

            // Push literal expression onto the stack.
            TheStack.Push(new Literal<TValue>(value));
            return true;
        }
        private bool DoPushStatLiteral(bool useLegalizedValue)
        {
            if (!TryPopAsType("Stat template identifier", out string identifier))
            {
                return false;
            }
            if (!StatTemplateRegistry.TryGetStatTemplate(identifier, out IStat<TValue> statTemplate))
            {
                Error($"\"{identifier.ToLowerInvariant()}\" is not a registered stat template identifier.");
                return false;
            }

            // Push stat literal expression onto the stack.
            TheStack.Push(new StatLiteral<TValue>(statTemplate, useLegalizedValue));
            return true;
        }
        private bool DoPushIdentifier(Token token)
        {
            TheStack.Push(token.Literal);
            return true;
        }
        private bool DoBeginExpression()
        {
            // Pop stack for string identifier.
            if (!TryPopAsType("Expression keyword", out string expressionKeyword))
            {
                // Stack was empty.
                // Error report is created in TryPopAsType().
                return false;
            }

            // Expression processing and end-of-expression consumption.
            if (!ExpressionRegistry.TryBuildAndPushExpression(expressionKeyword, this, out string message))
            {
                if (message != null)
                {
                    Error(message);
                }

                // Otherwise the message was already set by something else.
                return false;
            }

            return true;
        }
        private bool DoEndExpression()
        {
            TheStack.Push(null);
            return true;
        }
        /// <summary>
        /// Override this with your backing value's own string-to-number function.
        /// </summary>
        /// <param name="str">the number string (numerals and decimal points)</param>
        /// <param name="value">the parsed value</param>
        /// <returns>true if parse was successful</returns>
        protected abstract bool TryParseNumber(string str, out TValue value);
        /// <summary>
        /// Tries to pop the top object off the stack and cast to provided type. Calls to Error() beforehand if returning false.
        /// </summary>
        /// <typeparam name="T">casting type</typeparam>
        /// <param name="expectedObjName">expected object name for status message</param>
        /// <param name="result">popped and casted value</param>
        /// <returns>true if both pop and cast were successful</returns>
        public bool TryPopAsType<T>(string expectedObjName, out T result)
        {
            if (!TryPop(out object obj))
            {
                Error($"Expected: {expectedObjName}.");
                result = default;
                return false;
            }
            if (!TryCastAsType(obj, out result))
            {
                Error($"Expected: {expectedObjName}.");
                return false;
            }

            return true;
        }
        /// <summary>
        /// Tries to pop the top object off the stack and cast to provided type until NULL object is the next item (does not consume the NULL object). Calls to Error() beforehand if returning false.
        /// </summary>
        /// <typeparam name="T">casting type</typeparam>
        /// <param name="expectedObjName">expected object name for status message</param>
        /// <param name="args">new list of popped and casted values</param>
        /// <returns>true if all pops and casts were successful, and null object terminator was correctly encountered</returns>
        public bool TryPopManyAsType<T>(string expectedObjName, out List<T> args)
        {
            // Out list.
            args = new List<T>();

            while (TheStack.Count != 0)
            {
                // Check to see if the next object on the stack is null.
                if (TheStack.Peek() == null)
                {
                    // If null, then this is the end of the expression (don't consume the null object).
                    return true;
                }

                // Pop the next object off the stack.
                // This cannot fail, since the while loop checks the only cause of failure.
                TryPop(out object obj);

                // Try to cast the popped object as the type we want.
                if (!TryCastAsType(obj, out T expressionObj))
                {
                    // If failed, then there is a syntax error of some kind.
                    Error($"Expected: {expectedObjName}.");
                    return false;
                }

                // The object is of the type we want, so add it to the out list and loop again.
                args.Add(expressionObj);
            }

            // If failed, then stack was empty, meaning there was no end of expression.
            Error($"Expected: {expectedObjName} or end of expression.");
            return false;
        }
        /// <summary>
        /// Tries to pop a NULL object off the top of the stack (the object is consumed). Calls to Error() beforehand if returning false.
        /// </summary>
        /// <returns>true if the popped object was NULL</returns>
        public bool TryConsumeEndExpression()
        {
            if (!TryPop(out object obj))
            {
                Error("Expected: End of expression.");
                return false;
            }

            if (obj != null)
            {
                Error("Expected: End of expression. Top of stack was not null.");
                return false;
            }

            return true;
        }
        /// <summary>
        /// Push an object onto the stack.
        /// </summary>
        /// <param name="obj">any object</param>
        public void DoPushObject(object obj)
        {
            TheStack.Push(obj);
        }
    }
    /// <summary>
    /// Concrete parser class for stats with int (System.Int32) backing values.
    /// </summary>
    public sealed class Parser_Int : AbstractParser<int>
    {
        public Parser_Int(List<Token> tokens, ExpressionRegistry<int> expressionRegistry, StatTemplateRegistry<int> statTemplateRegistry)
            :base(tokens, expressionRegistry, statTemplateRegistry) { }

        protected override bool TryParseNumber(string str, out int value)
        {
            return int.TryParse(str, out value);
        }
    }
    /// <summary>
    /// Concrete parser class for stats with float (System.Single) backing values.
    /// </summary>
    public sealed class Parser_Float : AbstractParser<float>
    {
        public Parser_Float(List<Token> tokens, ExpressionRegistry<float> expressionRegistry, StatTemplateRegistry<float> statTemplateRegistry)
            : base(tokens, expressionRegistry, statTemplateRegistry) { }

        protected override bool TryParseNumber(string str, out float value)
        {
            return float.TryParse(str, out value);
        }
    }
}
