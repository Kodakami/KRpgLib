namespace KRpgLib.Stats.Compound
{
    // TODO: Best replaced when stat backing values have settled.

    //public static class QuickAlgo
    //{
    //    /// <summary>
    //    /// Get a new CompoundStatAlgorithm that follows a simple (stat X * stat Y) formula. Use this method to generate the algorithm, then save the reference and return it from your ICompoundStatTemplate's Algorithm property.
    //    /// </summary>
    //    /// <param name="x">any IStatTemplate</param>
    //    /// <param name="y">any IStatTemplate</param>
    //    /// <param name="useLegalizedValues">if true, the values will be legalized between each step (this is typically false)</param>
    //    /// <returns>a new CompoundStatAlgorithm</returns>
    //    public static CompoundStatAlgorithm StatXTimesStatY(IStatTemplate x, IStatTemplate y, bool useLegalizedValues)
    //    {
    //        return new CompoundStatAlgorithm(
    //            new Step_BinaryOperation(CommonInstances.Add, new StatLiteral(x, useLegalizedValues)),
    //            new Step_BinaryOperation(CommonInstances.Multiply, new StatLiteral(y, useLegalizedValues))
    //            );
    //    }
    //    /// <summary>
    //    /// Get a new CompoundStatAlgorithm that follows a simple (stat X + (Y * stat Z)) formula.
    //    /// </summary>
    //    /// <param name="x">any IStatTemplate</param>
    //    /// <param name="yMultiplier">a multiplier to apply to Z</param>
    //    /// <param name="z">any IStatTemplate</param>
    //    /// <param name="useLegalizedValues">if true, the values will be legalized between each step (this is typically false)</param>
    //    /// <returns>a new CompoundStatAlgorithm</returns>
    //    public static CompoundStatAlgorithm StatXPlusYOfStatZ(IStatTemplate x, float yMultiplier, IStatTemplate z, bool useLegalizedValues)
    //    {
    //        return new CompoundStatAlgorithm(
    //            new Step_BinaryOperation(CommonInstances.Add, new StatLiteral(x, useLegalizedValues)),
    //            new Step_BinaryOperation(CommonInstances.Add,
    //                new Operation_Binary(CommonInstances.Multiply, new StatLiteral(z, useLegalizedValues), new Literal(yMultiplier))
    //                )
    //            );
    //    }
    //}
}
