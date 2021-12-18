using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("KRpgLibTests")]
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
        public bool TryScanTokens(out IReadOnlyList<Token> tokens)
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
            return true;
        }
        private bool ScanNextToken()
        {
            //_currentIndex++;
            char c = Advance();
            switch (c)
            {
                // Skip over whitespace.
                case ' ':
                case '\n':
                case '\t':
                    break;
                // Begin new expression.
                case '(':
                    AddToken(TokenType.PAREN_OPEN);
                    break;
                // End of expression.
                case ')':
                    AddToken(TokenType.PAREN_CLOSED);
                    break;
                // Raw stat value literal.
                case '$':
                    AddToken(TokenType.CASH);
                    break;
                // Legalized stat value literal.
                case '*':
                    AddToken(TokenType.ASTERISK);
                    break;

                default:
                    // Number literal.
                    if (char.IsDigit(c))
                    {
                        Number();
                    }
                    // Expression keyword or stat identifier.
                    else if (char.IsLetter(c) || c == '_')
                    {
                        Identifier();
                    }
                    // Unrecognized or syntax error.
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
            // Numbers are not parsed here, only scanned and left as strings.

            while (!IsAtEnd())
            {
                // Look ahead to the next char.
                char c = Peek();

                // If it is not a number, then it is past the end of the number lexeme.
                if (!char.IsDigit(c))
                {
                    // Break the loop.
                    break;
                }

                // If it is a number, then it is part of the number lexeme and we need to consume the character.
                Advance();
            }

            AddToken(TokenType.NUMBER, GetCurrentSubstring());
        }
        private void Identifier()
        {
            // Something like an expression name or stat name.
            while (!IsAtEnd())
            {
                char c = Peek();
                if (!char.IsLetter(c) && c != '_')
                {
                    break;
                }
                Advance();
            }

            AddToken(TokenType.IDENTIFIER, GetCurrentSubstring());
        }
    }
}
