namespace Ultimatum.Models
{
    /// <summary>
    /// Represents a single data point for market regime clustering.
    /// The features should capture aspects of trend, volatility, and momentum.
    /// </summary>
    public class ClusterData
    {
        // Example features for clustering. These will be calculated from indicators.

        /// <summary>
        /// A measure of market volatility (e.g., Average True Range).
        /// </summary>
        public double Volatility { get; set; }

        /// <summary>
        /// A measure of the strength of the trend (e.g., ADX).
        /// </summary>
        public double TrendStrength { get; set; }

        /// <summary>
        /// A measure of market momentum (e.g., RSI or a custom momentum indicator).
        /// </summary>
        public double Momentum { get; set; }

        /// <summary>
        /// Converts the data point into a double array, which is a common
        /// format required by machine learning and clustering libraries.
        /// </summary>
        public double[] ToArray()
        {
            return new double[] { Volatility, TrendStrength, Momentum };
        }
    }
}
