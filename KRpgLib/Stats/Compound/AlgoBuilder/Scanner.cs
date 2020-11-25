using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    // Scans an Algo script string and emits Tokens.
    internal sealed class Scanner
    {
        private readonly string _script;
        private readonly List<Token> _tokens = new List<Token>();

        private int _startIndex;
        private int _currentIndex;

        public string StatusMessage { get; private set; } = "Incomplete.";
        public Result ScanResult { get; private set; } = Result.UNDEFINED;

        public Scanner(string script)
        {
            if (string.IsNullOrWhiteSpace(script))
            {
                throw new System.ArgumentException(nameof(script));
            }

            _script = script;
        }
        private void Error(string message)
        {
            StatusMessage = $"Scanner error at token: {(_currentIndex < _tokens.Count ? _tokens[_currentIndex].ToString() : "Invalid token index.")}. {message}";
            ScanResult = Result.FAIL;
        }
        public bool TryScanTokens(out List<Token> tokens)
        {
            tokens = _tokens;
            _currentIndex = 0;

            while (!IsAtEnd())
            {
                _startIndex = _currentIndex;

                if (!ScanNextToken())
                {
                    return false;
                }
            }

            // Anything else to do before end of scan goes here.
            tokens = _tokens;
            return true;
        }
        private bool ScanNextToken()
        {
            //_currentIndex++;
            char c = Advance();
            switch (c)
            {
                case ' ':
                case '\n':
                case '\t':
                    break;
                case '(':
                    AddToken(TokenType.PAREN_OPEN);
                    break;
                case ')':
                    AddToken(TokenType.PAREN_CLOSED);
                    break;
                case '$':
                    AddToken(TokenType.CASH);
                    break;
                case '*':
                    AddToken(TokenType.ASTERISK);
                    break;

                default:
                    if (char.IsDigit(c))
                    {
                        Number();
                    }
                    else if (char.IsLetter(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Error($"Unrecognized character \"{c}\"");
                        return false;
                    }
                    break;
            }
            return true;
        }
        private bool IsAtEnd()
        {
            return _currentIndex >= _script.Length;
        }
        private void AddToken(TokenType tokenType, object literal = null)
        {
            string text = GetCurrentSubstring();
            _tokens.Add(new Token(text, tokenType, literal, _startIndex));
        }
        private char Advance()
        {
            _currentIndex++;
            return _script[_currentIndex - 1];
        }
        private char Peek()
        {
            return _script[_currentIndex];
        }
        private string GetCurrentSubstring()
        {
            return _script.Substring(_startIndex, _currentIndex - _startIndex);
        }
        private void Number()
        {
            // Numbers are not parsed here, only scanned and left as strings. The parser will need to do type-specific parsing for the numbers.

            while (!IsAtEnd())
            {
                // Look ahead to the next char.
                char c = Peek();

                // If it is not a number or decimal point, then it is past the end of the number lexeme.
                if (!char.IsDigit(c) && c != '.')
                {
                    // Break the loop.
                    break;
                }
                
                //If it is a number or decimal point, then it is part of the number lexeme and we need to consume the character.
                Advance();
            }

            AddToken(TokenType.NUMBER, GetCurrentSubstring());
        }
        private void Identifier()
        {
            // Something like an expression name, stat name, or macro name.
            while (!IsAtEnd())
            {
                char c = Peek();
                if (!char.IsLetter(c))
                {
                    break;
                }
                Advance();
            }

            AddToken(TokenType.IDENTIFIER, GetCurrentSubstring());
        }
    }
}
