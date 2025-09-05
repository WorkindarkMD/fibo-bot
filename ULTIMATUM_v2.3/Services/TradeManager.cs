using cAlgo.API;
using cAlgo.API.Internals;
using Ultimatum.Utils;

namespace Ultimatum.Services
{
    /// <summary>
    /// Service responsible for executing and managing trades.
    /// It acts as a wrapper around the cTrader trading API to centralize trade logic.
    /// </summary>
    public class TradeManager
    {
        private readonly Robot _robot;
        private readonly Symbol _symbol;
        private readonly string _botLabel;

        public TradeManager(Robot robot, string botLabel)
        {
            _robot = robot;
            _symbol = robot.Symbol;
            _botLabel = botLabel;
        }

        /// <summary>
        /// Executes a market order with the specified parameters.
        /// </summary>
        /// <returns>The result of the trade execution.</returns>
        public TradeResult ExecuteMarketOrder(TradeType tradeType, double volumeInLots, double? stopLossPips, double? takeProfitPips)
        {
            var volumeInUnits = _symbol.QuantityToVolumeInUnits(volumeInLots);
            Logger.Info($"Executing Market Order: {tradeType} {volumeInLots} lots of {_symbol.Name}");
            return _robot.ExecuteMarketOrder(tradeType, _symbol.Name, volumeInUnits, _botLabel, stopLossPips, takeProfitPips);
        }

        /// <summary>
        /// Closes a specific position.
        /// </summary>
        /// <returns>The result of the closing trade.</returns>
        public TradeResult ClosePosition(Position position)
        {
            Logger.Info($"Closing position #{position.Id}");
            return _robot.ClosePosition(position);
        }

        /// <summary>
        /// Closes all open positions for the current symbol and bot label.
        /// </summary>
        public void CloseAllPositions()
        {
            var positionsToClose = _robot.Positions.FindAll(_botLabel, _symbol.Name);
            Logger.Info($"Closing {positionsToClose.Length} open positions for {_symbol.Name}.");
            foreach (var position in positionsToClose)
            {
                _robot.ClosePosition(position);
            }
        }

        /// <summary>
        /// Modifies the Stop Loss and Take Profit of an existing position.
        /// </summary>
        /// <returns>The result of the modification.</returns>
        public TradeResult ModifyPosition(Position position, double? newStopLossPips, double? newTakeProfitPips)
        {
            Logger.Info($"Modifying position #{position.Id} with SL: {newStopLossPips} pips, TP: {newTakeProfitPips} pips.");
            return _robot.ModifyPosition(position, newStopLossPips, newTakeProfitPips);
        }
    }
}
