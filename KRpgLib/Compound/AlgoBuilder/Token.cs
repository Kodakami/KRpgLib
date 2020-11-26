namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    /// <summary>
    /// A type of language token in Algo script.
    /// </summary>
    public enum TokenType
    {
        NUMBER = 0,         //[numeric literal]

        CASH = 1,           //$ [for raw stat value]
        ASTERISK = 2,       //* [for legalized stat value]

        IDENTIFIER = 3,
        PAREN_OPEN = 4,     //( [begin expression scope]
        PAREN_CLOSED = 5,   //) [end expression scope]
    }
    /// <summary>
    /// A language token in Algo script. Numbers are left as strings by the scanner.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// The string that was converted into this token.
        /// </summary>
        public readonly string Lexeme;
        /// <summary>
        /// The type of token identified by the scanner.
        /// </summary>
        public readonly TokenType TokenType;
        /// <summary>
        /// The literal representation of the token (or null if not applicable).
        /// </summary>
        public readonly object Literal;
        /// <summary>
        /// The index of the starting character in the script.
        /// </summary>
        public readonly int CharIndex;

        public Token(string lexeme, TokenType tokenType, object literal, int charIndex)
        {
            Lexeme = lexeme;
            TokenType = tokenType;
            Literal = literal;
            CharIndex = charIndex;
        }
        public override string ToString()
        {
            return $"TokenType[{TokenType}] Lexeme[{Lexeme}] Literal[{Literal}] CharIndex[{CharIndex}]";
        }
    }
}
