﻿using System;
using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    public abstract class AbstractParser<TValue> where TValue : struct
    {
        protected List<Token> Tokens { get; }
        protected int CurrentTokenIndex { get; private set; }   // Don't set this to Tokens.Count, use Count - 1.
        protected ExpressionRegistry<TValue> ExpressionRegistry { get; }
        protected StatTemplateRegistry<TValue> StatTemplateRegistry { get; }
        protected Stack<object> TheStack { get; } = new Stack<object>();

        public Result ParseResult { get; private set; } = Result.UNDEFINED;
        public string StatusMessage { get; private set; } = "Incomplete.";

        protected AbstractParser(List<Token> tokens, ExpressionRegistry<TValue> expressionRegistry, StatTemplateRegistry<TValue> statTemplateRegistry)
        {
            Tokens = tokens;
            ExpressionRegistry = expressionRegistry;
            StatTemplateRegistry = statTemplateRegistry;
        }
        public bool TryParseTokens(out ValueExpression<TValue> algo)
        {
            algo = null;

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
                        if (!DoPushLiteral(current))
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

            if (TheStack.Count > 1)
            {
                Error("Expression tree ended with leftover objects on the stack. Check for extraneous characters at end of script.");
                return false;
            }
            if (!TryPopValueExpression(out ValueExpression<TValue> finalValueExpression))
            {
                Error("Expression tree is not a value expression.");
                return false;
            }

            StatusMessage = "Parse completed without errors.";
            ParseResult = Result.OK;
            algo = finalValueExpression;
            return true;
        }
        public void Error(string message)
        {
            Token current = Tokens[CurrentTokenIndex];
            StatusMessage = $"Parse error at index {current.CharIndex}. {message}";
            ParseResult = Result.FAIL;
        }
        protected bool TryPop(out object obj)
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
        protected bool TryCastAsType<T>(object obj, out T value)
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
        protected bool DoPushLiteral(Token token)
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
        protected bool DoPushStatLiteral(bool useLegalizedValue)
        {
            if (!TryPopAsType("Stat template identifier", out string identifier))
            {
                return false;
            }
            if (!StatTemplateRegistry.TryGetStatTemplate(identifier, out IStatTemplate<TValue> statTemplate))
            {
                Error($"\"{identifier.ToLowerInvariant()}\" is not a registered stat template identifier.");
                return false;
            }

            // Push stat literal expression onto the stack.
            TheStack.Push(new StatLiteral<TValue>(statTemplate, useLegalizedValue));
            return true;
        }
        protected bool DoPushIdentifier(Token token)
        {
            TheStack.Push(token.Literal);
            return true;
        }
        protected bool DoBeginExpression()
        {
            // Pop stack for string identifier.
            if (!TryPopAsType("Expression keyword", out string expressionKeyword))
            {
                // Stack was empty.
                // Error report is created in TryPopAsType().
                return false;
            }
            // Expression registry lookup.
            if (!ExpressionRegistry.TryGetExpressionInfo(expressionKeyword, out ExpressionInfo<TValue> expressionInfo))
            {
                Error($"\"{expressionKeyword}\" is not a registered expression keyword.");
                return false;
            }

            // Will pop params and push result.
            expressionInfo.ExpressionObjectBuildAction(this);

            // TODO: When is the null consumed?

            return true;
        }
        protected bool DoEndExpression()
        {
            TheStack.Push(null);
            return true;
        }
        protected abstract bool TryParseNumber(string str, out TValue value);
        protected bool TryPopAsType<T>(string expectedObjName, out T result)
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
        protected bool TryPopManyAsType<T>(string expectedObjName, out List<T> args)
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
        public bool TryPopLogicExpression(out LogicExpression<TValue> logicExpression) => TryPopAsType("Logic expression", out logicExpression);
        public bool TryPopValueExpression(out ValueExpression<TValue> valueExpression) => TryPopAsType("Value expression", out valueExpression);
        public bool TryPopManyLogicExpressions(out List<LogicExpression<TValue>> logicExpressions) => TryPopManyAsType("Logic expression", out logicExpressions);
        public bool TryPopManyValueExpressions(out List<ValueExpression<TValue>> valueExpressions) => TryPopManyAsType("Value expression", out valueExpressions);
        public void DoPushLogicExpression(LogicExpression<TValue> logicExpression)
        {
            TheStack.Push(logicExpression ?? throw new ArgumentNullException(nameof(logicExpression)));
        }
        public void DoPushValueExpression(ValueExpression<TValue> valueExpression)
        {
            TheStack.Push(valueExpression ?? throw new ArgumentNullException(nameof(valueExpression)));
        }
        public void DoUnaryExpression(System.Func<Unar>)
            // TODO: I just realized I need to remake Compound.cs.
    }
}
