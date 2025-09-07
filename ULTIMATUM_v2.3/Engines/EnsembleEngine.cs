using cAlgo.API;
using Ultimatum.Models;
using Ultimatum.Utils;

namespace Ultimatum.Engines
{
    /// <summary>
    /// The "brain" of the bot. This engine aggregates signals from all other analysis
    /// engines to make a final, unified trading decision using a weighted consensus model.
    /// </summary>
    public class EnsembleEngine
    {
        private readonly ICTEngine _ictEngine;
        private readonly MLEngine _mlEngine;
        private readonly ClusterEngine _clusterEngine;
        private readonly MarketStateEngine _marketStateEngine;
        private readonly double _mlConfidenceThreshold;

        public EnsembleEngine(ICTEngine ictEngine, MLEngine mlEngine, ClusterEngine clusterEngine, MarketStateEngine marketStateEngine, double mlConfidenceThreshold)
        {
            _ictEngine = ictEngine;
            _mlEngine = mlEngine;
            _clusterEngine = clusterEngine;
            _marketStateEngine = marketStateEngine;
            _mlConfidenceThreshold = mlConfidenceThreshold;
        }

        /// <summary>
        /// Gathers all signals and produces a final TradingDecision.
        /// This is the core decision-making method of the entire bot.
        /// </summary>
        /// <param name="barIndex">The bar index to analyze.</param>
        /// <returns>A TradingDecision object (Buy, Sell, or Hold).</returns>
        public TradingDecision GenerateDecision(int barIndex)
        {
            // 1. Get signals from all available sources
            var ictPattern = _ictEngine.FindLastPattern();
            var mlPrediction = _mlEngine.Predict(_mlEngine.GenerateFeatures(barIndex));
            var marketRegime = _clusterEngine.CurrentMarketRegime;
            // var wyckoffPhase = _marketStateEngine.CurrentPhase; // For future use

            // --- Simple Consensus Logic (Placeholder for a more advanced weighted model) ---
            // A basic example: We need a strong signal from both the ICT and ML engines to trade.
            // The market regime acts as a filter.

            bool isBullishRegime = marketRegime.Regime == RegimeType.TrendingBull;
            bool isBearishRegime = marketRegime.Regime == RegimeType.TrendingBear;

            bool hasIctBuySignal = ictPattern?.Direction == TradeType.Buy;
            bool hasMlBuySignal = mlPrediction.HasPrediction && mlPrediction.Direction == PredictedDirection.Bullish && mlPrediction.Confidence >= _mlConfidenceThreshold;

            if (hasIctBuySignal && hasMlBuySignal && isBullishRegime)
            {
                var decision = new TradingDecision
                {
                    Action = TradingAction.Buy,
                    ConfidenceScore = (mlPrediction.Confidence + 0.8) / 1.8, // Simple avg, weighted towards ML
                    StopLossInPips = 20, // Example SL, would be dynamically calculated later
                    TakeProfitInPips = 40 // Example TP
                };
                decision.Reasoning.Add($"ICT Signal: {ictPattern.PatternType}");
                decision.Reasoning.Add($"ML Signal: Bullish (Conf: {mlPrediction.Confidence:P0})");
                decision.Reasoning.Add($"Regime Filter: {marketRegime.Regime} (Passed)");
                return decision;
            }

            bool hasIctSellSignal = ictPattern?.Direction == TradeType.Sell;
            bool hasMlSellSignal = mlPrediction.HasPrediction && mlPrediction.Direction == PredictedDirection.Bearish && mlPrediction.Confidence >= _mlConfidenceThreshold;

            if (hasIctSellSignal && hasMlSellSignal && isBearishRegime)
            {
                var decision = new TradingDecision
                {
                    Action = TradingAction.Sell,
                    ConfidenceScore = (mlPrediction.Confidence + 0.8) / 1.8,
                    StopLossInPips = 20,
                    TakeProfitInPips = 40
                };
                decision.Reasoning.Add($"ICT Signal: {ictPattern.PatternType}");
                decision.Reasoning.Add($"ML Signal: Bearish (Conf: {mlPrediction.Confidence:P0})");
                decision.Reasoning.Add($"Regime Filter: {marketRegime.Regime} (Passed)");
                return decision;
            }

            // If no strong consensus is reached, return the default "Hold" decision.
            return TradingDecision.Hold();
        }
    }
}
