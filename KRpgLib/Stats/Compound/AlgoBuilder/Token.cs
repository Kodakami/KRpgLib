namespace KRpgLib.Stats.Compound.AlgoBuilder
{
    public enum TokenType
    {
        NUMBER = 0,         //[numeric literal]

        CASH = 1,           //$ [for raw stat value]
        ASTERISK = 2,       //* [for legalized stat value]

        IDENTIFIER = 3,
        PAREN_OPEN = 4,     //( [begin expression scope]
        PAREN_CLOSED = 5,   //) [end expression scope]
    }
    public class Token
    {
        public readonly string Lexeme;
        public readonly TokenType TokenType;
        public readonly object Literal;
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
            return $"TokenType[{TokenType}] Lexeme[{Lexeme}] Literal[{Literal}]";
        }
    }
}
