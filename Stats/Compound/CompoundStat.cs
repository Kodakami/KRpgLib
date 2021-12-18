namespace KRpgLib.Stats.Compound
{
    /// <summary>
    /// A compound stat which uses minimum, maximum, and precision values, and a CompoundStatAlgorithm to calculate the current value.
    /// </summary>
    public class CompoundStat
    {
        protected CompoundStatAlgorithm Algorithm { get; }
        public StatLegalizer StatLegalizer { get; }

        protected CompoundStat(StatLegalizer statLegalizer, CompoundStatAlgorithm algorithm)
        {
            StatLegalizer = statLegalizer ?? throw new System.ArgumentNullException(nameof(statLegalizer));
            Algorithm = algorithm ?? throw new System.ArgumentNullException(nameof(algorithm));
        }
        public CompoundStat(int? min, int? max, int? precison, CompoundStatAlgorithm algorithm)
            :this(new StatLegalizer(min, max, precison), algorithm) { }

        public int CalculateValue(IStatSet statSet) => Algorithm.CalculateValue(statSet ?? throw new System.ArgumentNullException(nameof(statSet)));

        /// <inheritdoc cref="StatLegalizer.GetLegalizedValue(int)"/>
        public int GetLegalizedValue(int rawValue) => StatLegalizer.GetLegalizedValue(rawValue);
    }
}
