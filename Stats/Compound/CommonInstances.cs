using System.Linq;

namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// Utility class with static instances of common operation types for expression objects.
    /// </summary>
    public static class CommonInstances
    {
        /// <summary>
        /// n => -n
        /// </summary>
        public static readonly UnaryOperationType Negative = new UnaryOperationType(n => -n);

        /// <summary>
        /// param1 + param2 + ... + paramN
        /// </summary>
        public static readonly MultiaryOperationType Add =
            new MultiaryOperationType(values => values.Aggregate((total, curr) => total + curr));  // Same as values.Sum()
        /// <summary>
        /// param1 - param2 - ... - paramN
        /// </summary>
        public static readonly MultiaryOperationType Subtract = new MultiaryOperationType(values => values.Aggregate((total, curr) => total - curr));
        /// <summary>
        /// param1 * param2 * ... * paramN
        /// </summary>
        public static readonly MultiaryOperationType Multiply = new MultiaryOperationType(value => value.Aggregate((total, curr) => total * curr));
        /// <summary>
        /// left / right [Division by 0 throws exception]
        /// </summary>
        public static readonly BinaryOperationType Divide = new BinaryOperationType(
            (l, r) =>
            {
                if (r == 0)
                {
                    throw new System.DivideByZeroException($"A CompoundStatAlgorithm attempted to divide {l} by {r}.");
                }
                return l / r;
            });
        /// <summary>
        /// left to the right power
        /// </summary>
        public static readonly BinaryOperationType PowerOf = new BinaryOperationType((l, r) => (int)System.Math.Pow(l, r));
        /// <summary>
        /// System.Math.Min(param1, param2, ... paramN)
        /// </summary>
        public static readonly MultiaryOperationType Smallest =
            new MultiaryOperationType(values => values.Aggregate((total, curr) => System.Math.Min(total, curr)));
        /// <summary>
        /// System.Math.Max(param1, param2, ... paramN)
        /// </summary>
        public static readonly MultiaryOperationType Biggest =
            new MultiaryOperationType(values => values.Aggregate((total, curr) => System.Math.Max(total, curr)));
        /// <summary>
        /// left % right [Modulo by 0 throws exception]
        /// </summary>
        public static readonly BinaryOperationType Modulo = new BinaryOperationType(
            (l, r) =>
            {
                if (r == 0)
                {
                    throw new System.DivideByZeroException($"A CompoundStatAlgorithm attempted to modulo {l} by {r}.");
                }
                return l % r;
            });

        /// <summary>
        /// left == right
        /// </summary>
        public static readonly ComparisonType EqualTo = new ComparisonType((l, r) => l == r);
        /// <summary>
        /// left != right
        /// </summary>
        public static readonly ComparisonType NotEqualTo = new ComparisonType((l, r) => l != r);  // Exists for convenience.
        /// <summary>
        /// left > right
        /// </summary>
        public static readonly ComparisonType GreaterThan = new ComparisonType((l, r) => l > r);
        /// <summary>
        /// left < right
        /// </summary>
        public static readonly ComparisonType LessThan = new ComparisonType((l, r) => l < r);
        /// <summary>
        /// left >= right
        /// </summary>
        public static readonly ComparisonType GreaterThanOrEqualTo = new ComparisonType((l, r) => l >= r);
        /// <summary>
        /// left <= right
        /// </summary>
        public static readonly ComparisonType LessThanOrEqualTo = new ComparisonType((l, r) => l <= r);
    }
}
