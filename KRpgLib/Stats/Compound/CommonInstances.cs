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

        public static class Int
        {
            // Unary numeric operations.
            public static readonly UnaryOperationType<int> Negative = new UnaryOperationType<int>(n => -n);

            // Binary numeric operations.
            public static readonly BinaryOperationType<int> Add = new BinaryOperationType<int>((l, r) => l + r);
            public static readonly BinaryOperationType<int> Subtract = new BinaryOperationType<int>((l, r) => l - r);
            public static readonly BinaryOperationType<int> Multiply = new BinaryOperationType<int>((l, r) => l * r);
            public static readonly BinaryOperationType<int> Divide = new BinaryOperationType<int>(
                (l, r) =>
                {
                    if (r == 0)
                    {
                        // Division By Zero avoidance.
                        return 0;
                    }
                    return l / r;
                });
            public static readonly BinaryOperationType<int> PowerOf = new BinaryOperationType<int>((l, r) => (int)System.Math.Pow(l, r));
            public static readonly BinaryOperationType<int> Min = new BinaryOperationType<int>((l, r) => System.Math.Min(l, r));
            public static readonly BinaryOperationType<int> Max = new BinaryOperationType<int>((l, r) => System.Math.Max(l, r));
            public static readonly BinaryOperationType<int> Modulo = new BinaryOperationType<int>((l, r) => l % r);

            public static readonly BinaryOperationType<int> SetTo = new BinaryOperationType<int>((_, r) => r);

            // Numeric comparisons.
            public static readonly ComparisonType<int> EqualTo = new ComparisonType<int>((l, r) => l == r);
            public static readonly ComparisonType<int> NotEqualTo = new ComparisonType<int>((l, r) => l != r);  // Exists for convenience.
            public static readonly ComparisonType<int> GreaterThan = new ComparisonType<int>((l, r) => l > r);
            public static readonly ComparisonType<int> LessThan = new ComparisonType<int>((l, r) => l < r);
            public static readonly ComparisonType<int> GreaterThanOrEqualTo = new ComparisonType<int>((l, r) => l >= r);
            public static readonly ComparisonType<int> LessThanOrEqualTo = new ComparisonType<int>((l, r) => l <= r);
        }
        // I hate to copy-paste, but...
        public static class Float
        {
            // Unary numeric operations.
            public static readonly UnaryOperationType<float> Negative = new UnaryOperationType<float>(n => -n);

            // Binary numeric operations.
            public static readonly BinaryOperationType<float> Add = new BinaryOperationType<float>((l, r) => l + r);
            public static readonly BinaryOperationType<float> Subtract = new BinaryOperationType<float>((l, r) => l - r);
            public static readonly BinaryOperationType<float> Multiply = new BinaryOperationType<float>((l, r) => l * r);
            public static readonly BinaryOperationType<float> Divide = new BinaryOperationType<float>(
                (l, r) =>
                {
                    if (r == 0)
                    {
                    // Division By Zero avoidance.
                    return 0;
                    }
                    return l / r;
                });
            public static readonly BinaryOperationType<float> PowerOf = new BinaryOperationType<float>((l, r) => (float)System.Math.Pow(l, r));
            public static readonly BinaryOperationType<float> Min = new BinaryOperationType<float>((l, r) => (float)System.Math.Min(l, r));
            public static readonly BinaryOperationType<float> Max = new BinaryOperationType<float>((l, r) => (float)System.Math.Max(l, r));
            public static readonly BinaryOperationType<float> Modulo = new BinaryOperationType<float>((l, r) => l % r);

            public static readonly BinaryOperationType<float> SetTo = new BinaryOperationType<float>((_, r) => r);

            // Numeric comparisons.
            public static readonly ComparisonType<float> EqualTo = new ComparisonType<float>((l, r) => l == r);
            public static readonly ComparisonType<float> NotEqualTo = new ComparisonType<float>((l, r) => l != r);  // Exists for convenience.
            public static readonly ComparisonType<float> GreaterThan = new ComparisonType<float>((l, r) => l > r);
            public static readonly ComparisonType<float> LessThan = new ComparisonType<float>((l, r) => l < r);
            public static readonly ComparisonType<float> GreaterThanOrEqualTo = new ComparisonType<float>((l, r) => l >= r);
            public static readonly ComparisonType<float> LessThanOrEqualTo = new ComparisonType<float>((l, r) => l <= r);
        }
    }
}
