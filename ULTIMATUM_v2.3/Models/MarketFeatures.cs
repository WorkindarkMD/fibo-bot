namespace Ultimatum.Models
{
    /// <summary>
    /// Represents the set of input features for the machine learning model for a single data point (e.g., a bar).
    /// This class will be expanded to include over 20 indicators as per the specification.
    /// </summary>
    public class MarketFeatures
    {
        // --- Indicator Features (Placeholders) ---
        public double Rsi14 { get; set; }
        public double Sma50 { get; set; }
        public double Sma200 { get; set; }
        public double MacdValue { get; set; }
        public double Atr14 { get; set; }

        // --- Price Action Features (Placeholders) ---
        public double PriceChangePercentage { get; set; }
        public double Volatility { get; set; }

        /// <summary>
        /// Converts the feature set into a double array for use with ML libraries like Accord.NET.
        /// The order of features in this array must be consistent.
        /// </summary>
        /// <returns>A double array representing the feature vector.</returns>
        public double[] ToArray()
        {
            return new double[]
            {
                Rsi14,
                Sma50,
                Sma200,
                MacdValue,
                Atr14,
                PriceChangePercentage,
                Volatility
            };
        }
    }
}
