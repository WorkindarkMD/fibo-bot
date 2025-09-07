using System;
using cAlgo.API;
using Ultimatum.Core.Configuration;
using Ultimatum.Utils;

namespace Ultimatum.Services
{
    /// <summary>
    /// Service responsible for risk calculations, primarily position sizing.
    /// </summary>
    public class RiskManager
    {
        private readonly Account _account;
        private readonly Symbol _symbol;

        public RiskManager(Robot robot)
        {
            _account = robot.Account;
            _symbol = robot.Symbol;
        }

        /// <summary>
        /// Calculates the appropriate position size in lots based on a risk percentage of the account balance.
        /// </summary>
        /// <param name="stopLossPips">The stop loss for the trade in pips. Must be positive.</param>
        /// <param name="riskPercent">The percentage of the account to risk (e.g., 2.0 for 2%).</param>
        /// <returns>The calculated volume in lots, normalized and within symbol limits.</returns>
        public double CalculateVolumeInLots(double stopLossPips, double riskPercent)
        {
            if (stopLossPips <= 0)
            {
                Logger.Warning("Stop loss must be greater than 0 to calculate position size. Defaulting to minimum volume.");
                return _symbol.VolumeInUnitsToQuantity(_symbol.VolumeMin);
            }

            // Use the lesser of the input risk and the hard-coded max risk from config
            double effectiveRiskPercent = Math.Min(riskPercent, RiskLimits.MAX_RISK_PER_TRADE * 100.0);

            double amountToRisk = _account.Balance * (effectiveRiskPercent / 100.0);

            // This is how much one unit of volume is worth per pip
            double valuePerPipPerUnit = _symbol.PipValue;

            if (valuePerPipPerUnit <= 0)
            {
                 Logger.Warning("Could not calculate value per pip for the symbol. Defaulting to minimum volume.");
                 return _symbol.VolumeInUnitsToQuantity(_symbol.VolumeMin);
            }

            // Total value of the planned stop loss in currency
            double totalRiskValuePerUnit = stopLossPips * valuePerPipPerUnit;

            // How many units can we trade to match our risk amount
            double volumeInUnits = amountToRisk / totalRiskValuePerUnit;

            // Normalize the volume to the symbol's allowed steps and limits
            volumeInUnits = _symbol.NormalizeVolumeInUnits(volumeInUnits, RoundingMode.Down);
            volumeInUnits = Math.Max(volumeInUnits, _symbol.VolumeMin);
            volumeInUnits = Math.Min(volumeInUnits, _symbol.VolumeMax);

            double volumeInLots = _symbol.VolumeInUnitsToQuantity(volumeInUnits);

            Logger.Info($"Calculated position size: {volumeInLots} lots for a {stopLossPips} pip SL with {effectiveRiskPercent}% risk.");

            return volumeInLots;
        }
    }
}
