using System.Collections.Generic;

namespace Ultimatum.Models
{
    /// <summary>
    /// Defines the possible trading actions the bot can decide to take.
    /// </summary>
    public enum TradingAction
    {
        Hold,
        Buy,
        Sell
    }

    /// <summary>
    /// Represents the final, aggregated trading decision made by the EnsembleEngine.
    /// This object encapsulates not just the action, but also the confidence and reasoning behind it.
    /// </summary>
    public class TradingDecision
    {
        /// <summary>
        /// The type of action to take (Buy, Sell, or Hold).
        /// </summary>
        public TradingAction Action { get; set; }

        /// <summary>
        /// The overall confidence score for this decision, from 0.0 to 1.0.
        /// </summary>
        public double ConfidenceScore { get; set; }

        /// <summary>
        /// A list of reasons from various engines that support this decision. Used for XAI.
        /// </summary>
        public List<string> Reasoning { get; }

        /// <summary>
        /// The suggested Stop Loss in pips from the entry price. Can be null.
        /// </summary>
        public double? StopLossInPips { get; set; }

        /// <summary>
        /// The suggested Take Profit in pips from the entry price. Can be null.
        /// </summary>
        public double? TakeProfitInPips { get; set; }

        /// <summary>
        /// A static factory method to create a default "Hold" decision.
        /// </summary>
        public static TradingDecision Hold(string reason = "No consensus or signal.")
        {
            var decision = new TradingDecision
            {
                Action = TradingAction.Hold,
                ConfidenceScore = 0,
            };
            decision.Reasoning.Add(reason);
            return decision;
        }

        public TradingDecision()
        {
            Action = TradingAction.Hold;
            Reasoning = new List<string>();
        }
    }
}
