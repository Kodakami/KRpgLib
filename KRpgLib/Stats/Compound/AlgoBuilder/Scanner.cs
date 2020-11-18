using System.Collections.Generic;

namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    public sealed class Scanner<TValue> where TValue : struct
    {
        private readonly string _script;
        private readonly List<Token> _tokens = new List<Token>();

        private int _startIndex;
        private int _currentIndex;

        public string StatusMessage { get; private set; } = "Incomplete.";
        public Result ScanResult { get; private set; } = Result.UNDEFINED;

        public Scanner(string script)
        {
            _script = script;
        }
        private void Error(string message)
        {
            StatusMessage = $"Scanner error at character index {_currentIndex}. {message}";
            ScanResult = Result.FAIL;
        }
        public bool TryScanTokens(out List<Token> tokens)
        {
            while (!IsAtEnd())
            {
                _startIndex = _currentIndex;

                if (!ScanNextToken())
                {
                    tokens = null;
                    return false;
                }
            }

            // Anything else to do before end of scan goes here.
            tokens = _tokens;
            return true;
        }
        private bool ScanNextToken()
        {
            _currentIndex++;
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
        private string GetCurrentSubstring()
        {
            return _script.Substring(_startIndex, _currentIndex - _startIndex);
        }
        private void Number()
        {
            // Numbers are not parsed here, only scanned and left as strings. The parser will need to do type-specific parsing for the numbers.

            while (!IsAtEnd())
            {
                char c = Advance();
                if (!char.IsDigit(c) && c != '.')
                {
                    break;
                }
            }

            AddToken(TokenType.NUMBER, GetCurrentSubstring());
        }
        private void Identifier()
        {
            // Something like an expression name, stat name, or macro name.
            while (!IsAtEnd())
            {
                char c = Advance();
                if (!char.IsLetter(c) && !char.IsNumber(c))
                {
                    break;
                }
            }

            AddToken(TokenType.IDENTIFIER, GetCurrentSubstring());
        }
    }
}
