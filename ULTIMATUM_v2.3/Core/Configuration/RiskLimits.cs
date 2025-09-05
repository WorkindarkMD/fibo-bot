namespace Ultimatum.Core.Configuration
{
    /// <summary>
    /// Contains hard-coded risk limits to prevent catastrophic losses.
    /// These values act as a final safety net.
    /// </summary>
    public static class RiskLimits
    {
        /// <summary>
        /// Maximum risk per single trade as a percentage of account balance (0.15 = 15%).
        /// </summary>
        public const double MAX_RISK_PER_TRADE = 0.15;

        /// <summary>
        /// Maximum loss for a single day as a percentage of the day's starting balance (0.05 = 5%).
        /// </summary>
        public const double MAX_DAILY_LOSS = 0.05;

        /// <summary>
        /// Maximum total drawdown from the account's peak equity (0.20 = 20%).
        /// </summary>
        public const double MAX_DRAWDOWN = 0.20;

        /// <summary>
        /// Maximum number of simultaneously open trades allowed.
        /// </summary>
        public const int MAX_CONCURRENT_TRADES = 10;
    }
}
