namespace Ultimatum.Models
{
    /// <summary>
    /// Defines the types of market regimes the ClusterEngine can identify.
    /// The specification mentions 9 types. This enum will be expanded.
    /// </summary>
    public enum RegimeType
    {
        Undefined,
        TrendingBull,
        TrendingBear,
        RangeBoundWeak,
        RangeBoundStrong,
        VolatileBreakoutUp,
        VolatileBreakoutDown,
        LowVolatility,
        MeanReversion
    }

    /// <summary>
    /// Describes the current market regime identified by the ClusterEngine.
    /// This model corresponds to the UI elements in section 4.1.1 of the spec.
    /// </summary>
    public class MarketRegime
    {
        /// <summary>
        /// The type of market regime identified.
        /// </summary>
        public RegimeType Regime { get; set; } = RegimeType.Undefined;

        /// <summary>
        /// A score indicating how confident the engine is about this regime classification.
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// A measure of how stable the current regime is. High stability means it's less likely to change soon.
        /// </summary>
        public double Stability { get; set; }

        /// <summary>
        * How long this regime has been active, measured in number of bars.
        /// </summary>
        public int DurationInBars { get; set; }

        /// <summary>
        /// A human-readable trading recommendation based on the regime.
        /// </summary>
        public string Recommendation { get; set; }
    }
}
