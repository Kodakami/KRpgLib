namespace KRpgLib.Stats.Compound
{
    public static class CommonInstances
    {
        // Preserved code for when I implement Boolean logic in comparison expresssions.

        //public static class Flag
        //{
        //    // Unary boolean operations.
        //    public static readonly UnaryOperationType<bool> Not = new UnaryOperationType<bool>(b => !b);

        //    // Binary boolean operations.
        //    public static readonly BinaryOperationType<bool> And = new BinaryOperationType<bool>((l, r) => l && r);
        //    public static readonly BinaryOperationType<bool> Or = new BinaryOperationType<bool>((l, r) => l || r);
        //    public static readonly BinaryOperationType<bool> Xor = new BinaryOperationType<bool>((l, r) => l ^ r);

        //    // Comparisons.
        //    public static readonly ComparisonType<bool> EqualTo = new ComparisonType<bool>((l, r) => l == r);
        //    public static readonly ComparisonType<bool> NotEqualTo = new ComparisonType<bool>((l, r) => l != r);
        //}

        // Unary numeric operations.
        public static readonly UnaryOperationType Negative = new UnaryOperationType(n => -n);

        // Binary numeric operations.
        public static readonly BinaryOperationType Add = new BinaryOperationType((l, r) => l + r);
        public static readonly BinaryOperationType Subtract = new BinaryOperationType((l, r) => l - r);
        public static readonly BinaryOperationType Multiply = new BinaryOperationType((l, r) => l * r);
        public static readonly BinaryOperationType Divide = new BinaryOperationType(
            (l, r) =>
            {
                if (r == 0)
                {
                    // Division By Zero avoidance.
                    return 0;
                }
                return l / r;
            });
        public static readonly BinaryOperationType PowerOf = new BinaryOperationType((l, r) => (float)System.Math.Pow(l, r));
        public static readonly BinaryOperationType Min = new BinaryOperationType((l, r) => (float)System.Math.Min(l, r));
        public static readonly BinaryOperationType Max = new BinaryOperationType((l, r) => (float)System.Math.Max(l, r));
        public static readonly BinaryOperationType Modulo = new BinaryOperationType((l, r) => l % r);
        
        public static readonly BinaryOperationType SetTo = new BinaryOperationType((_, r) => r);

        // Numeric comparisons.
        public static readonly ComparisonType EqualTo = new ComparisonType((l, r) => l == r);
        public static readonly ComparisonType NotEqualTo = new ComparisonType((l, r) => l != r);  // Exists for convenience.
        public static readonly ComparisonType GreaterThan = new ComparisonType((l, r) => l > r);
        public static readonly ComparisonType LessThan = new ComparisonType((l, r) => l < r);
        public static readonly ComparisonType GreaterThanOrEqualTo = new ComparisonType((l, r) => l >= r);
        public static readonly ComparisonType LessThanOrEqualTo = new ComparisonType((l, r) => l <= r);
    }
}
