﻿using System.Linq;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Utility class with static instances of common operation types for expression objects.
    /// </summary>
    public static class CommonInstances
    {
        /// <summary>
        /// Operation types for stats with int (System.Int32) value backing.
        /// </summary>
        public static class Int
        {
            /// <summary>
            /// n => -n
            /// </summary>
            public static readonly UnaryOperationType<int> Negative = new UnaryOperationType<int>(n => -n);

            /// <summary>
            /// param1 + param2 + ... + paramN
            /// </summary>
            public static readonly MultiaryOperationType<int> Add =
                new MultiaryOperationType<int>(values => values.Aggregate((total, curr) => total + curr));  // Same as values.Sum()
            /// <summary>
            /// param1 - param2 - ... - paramN
            /// </summary>
            public static readonly MultiaryOperationType<int> Subtract = new MultiaryOperationType<int>(values => values.Aggregate((total, curr) => total - curr));
            /// <summary>
            /// param1 * param2 * ... * paramN
            /// </summary>
            public static readonly MultiaryOperationType<int> Multiply = new MultiaryOperationType<int>(value => value.Aggregate((total, curr) => total * curr));
            /// <summary>
            /// left / right [Division by 0 returns 0]
            /// </summary>
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
            /// <summary>
            /// left to the right power
            /// </summary>
            public static readonly BinaryOperationType<int> PowerOf = new BinaryOperationType<int>((l, r) => (int)System.Math.Pow(l, r));
            /// <summary>
            /// System.Math.Min(param1, param2, ... paramN)
            /// </summary>
            public static readonly MultiaryOperationType<int> Min =
                new MultiaryOperationType<int>(values => values.Aggregate((total, curr) => System.Math.Min(total, curr)));
            /// <summary>
            /// System.Math.Max(param1, param2, ... paramN)
            /// </summary>
            public static readonly MultiaryOperationType<int> Max =
                new MultiaryOperationType<int>(values => values.Aggregate((total, curr) => System.Math.Max(total, curr)));
            /// <summary>
            /// left % right [Modulo by 0 returns 0]
            /// </summary>
            public static readonly BinaryOperationType<int> Modulo = new BinaryOperationType<int>(
                (l, r) =>
                {
                    if (r == 0)
                    {
                        // Division By Zero avoidance.
                        return 0;
                    }
                    return l % r;
                });

            /// <summary>
            /// left == right
            /// </summary>
            public static readonly ComparisonType<int> EqualTo = new ComparisonType<int>((l, r) => l == r);
            /// <summary>
            /// left != right
            /// </summary>
            public static readonly ComparisonType<int> NotEqualTo = new ComparisonType<int>((l, r) => l != r);  // Exists for convenience.
            /// <summary>
            /// left > right
            /// </summary>
            public static readonly ComparisonType<int> GreaterThan = new ComparisonType<int>((l, r) => l > r);
            /// <summary>
            /// left < right
            /// </summary>
            public static readonly ComparisonType<int> LessThan = new ComparisonType<int>((l, r) => l < r);
            /// <summary>
            /// left >= right
            /// </summary>
            public static readonly ComparisonType<int> GreaterThanOrEqualTo = new ComparisonType<int>((l, r) => l >= r);
            /// <summary>
            /// left <= right
            /// </summary>
            public static readonly ComparisonType<int> LessThanOrEqualTo = new ComparisonType<int>((l, r) => l <= r);
        }
        
        // I hate to copy-paste, but...

        /// <summary>
        /// Operation types for stats with float (System.Single) value backing.
        /// </summary>
        public static class Float
        {
            // Unary numeric operations.
            public static readonly UnaryOperationType<float> Negative = new UnaryOperationType<float>(n => -n);

            // Binary numeric operations.
            public static readonly MultiaryOperationType<float> Add =
                new MultiaryOperationType<float>(values => values.Aggregate((total, curr) => total + curr));    // Same as values.Sum()
            public static readonly MultiaryOperationType<float> Subtract = new MultiaryOperationType<float>(values => values.Aggregate((total, curr) => total - curr));
            public static readonly MultiaryOperationType<float> Multiply = new MultiaryOperationType<float>(value => value.Aggregate((total, curr) => total * curr));
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
            public static readonly MultiaryOperationType<float> Min = new MultiaryOperationType<float>(values => values.Aggregate((total, curr) => System.Math.Min(total, curr)));
            public static readonly MultiaryOperationType<float> Max = new MultiaryOperationType<float>(values => values.Aggregate((total, curr) => System.Math.Max(total, curr)));
            public static readonly BinaryOperationType<float> Modulo = new BinaryOperationType<float>(
                (l, r) =>
                {
                    if (r == 0)
                    {
                        // Division By Zero avoidance.
                        return 0;
                    }
                    return l % r;
                });

            // Numeric comparisons.
            public static readonly ComparisonType<float> EqualTo = new ComparisonType<float>((l, r) => l == r);
            public static readonly ComparisonType<float> NotEqualTo = new ComparisonType<float>((l, r) => l != r);  // Exists for convenience.
            public static readonly ComparisonType<float> GreaterThan = new ComparisonType<float>((l, r) => l > r);
            public static readonly ComparisonType<float> LessThan = new ComparisonType<float>((l, r) => l < r);
            public static readonly ComparisonType<float> GreaterThanOrEqualTo = new ComparisonType<float>((l, r) => l >= r);
            public static readonly ComparisonType<float> LessThanOrEqualTo = new ComparisonType<float>((l, r) => l <= r);
        }

        /// <summary>
        /// Operation types for stats with bool (System.Bool) value backing. This seems fun.
        /// </summary>
        public static class Bool
        {
            public static readonly UnaryOperationType<bool> Not = new UnaryOperationType<bool>(x => !x);
            public static readonly BinaryOperationType<bool> And = new BinaryOperationType<bool>((l, r) => l && r);
            public static readonly BinaryOperationType<bool> Or = new BinaryOperationType<bool>((l, r) => l || r);
            public static readonly BinaryOperationType<bool> Xor = new BinaryOperationType<bool>((l, r) => l ^ r);
            public static readonly MultiaryOperationType<bool> All = new MultiaryOperationType<bool>(values => values.TrueForAll(val => val));    // All are true.
            public static readonly MultiaryOperationType<bool> Any = new MultiaryOperationType<bool>(values => values.Any(val => val));           // At least one is true.
            public static readonly MultiaryOperationType<bool> One = new MultiaryOperationType<bool>(values => values.Count(val => val) == 1);    // Exactly one is true.
        }
    }
}