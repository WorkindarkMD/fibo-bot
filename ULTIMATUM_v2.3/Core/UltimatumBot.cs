using System;
using cAlgo.API;
using Ultimatum.Utils;
using Ultimatum.Core.Configuration;
using Ultimatum.Engines;
using Ultimatum.Services;

namespace Ultimatum.Core
{
    [Robot("ULTIMATUM v2.3", TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class UltimatumBot : Robot
    {
        #region Parameters

        [Parameter("--- Main Settings ---", Group = "Main", DefaultValue = "")]
        public string MainSettingsSeparator { get; set; }

        [Parameter("Enable Trading", DefaultValue = true, Group = "Main")]
        public bool EnableTrading { get; set; }

        [Parameter("Lot Size", DefaultValue = 0.01, MinValue = 0.01, Group = "Main")]
        public double LotSize { get; set; }

        [Parameter("Risk Percentage", DefaultValue = 2.0, MinValue = 0.1, MaxValue = 10.0, Group = "Main")]
        public double RiskPercent { get; set; }

        [Parameter("Magic Number", DefaultValue = 230323, Group = "Main")]
        public int MagicNumber { get; set; }

        [Parameter("--- AI/ML Settings ---", Group = "AI/ML", DefaultValue = "")]
        public string AiMlSettingsSeparator { get; set; }

        [Parameter("Enable Machine Learning", DefaultValue = true, Group = "AI/ML")]
        public bool EnableML { get; set; }

        [Parameter("ML Confidence Threshold", DefaultValue = 0.7, MinValue = 0.1, MaxValue = 0.99, Group = "AI/ML")]
        public double MLConfidenceThreshold { get; set; }

        [Parameter("Enable Cluster Analysis", DefaultValue = true, Group = "AI/ML")]
        public bool EnableClusterAnalysis { get; set; }

        [Parameter("Cluster Count (0 = auto)", DefaultValue = 0, Group = "AI/ML")]
        public int ClusterCount { get; set; }

        [Parameter("--- API Settings ---", Group = "API", DefaultValue = "")]
        public string ApiSettingsSeparator { get; set; }

        [Parameter("News API Key", DefaultValue = "889bc03accb645a984e07f265b4f16f9", Group = "API")]
        public string NewsAPIKey { get; set; }

        [Parameter("Enable News Filter", DefaultValue = true, Group = "API")]
        public bool EnableNewsFilter { get; set; }

        [Parameter("Enable Real-time Data", DefaultValue = true, Group = "API")]
        public bool EnableRealTimeData { get; set; }

        #endregion

        #region Services and Engines

        private ICTEngine _ictEngine;
        private MarketStateEngine _marketStateEngine;
        private RiskManager _riskManager;
        private TradeManager _tradeManager;

        #endregion

        protected override void OnStart()
        {
            Logger.Initialize(this);
            Logger.Info("--- Bot Starting ---");

            // Initialize services and engines
            _ictEngine = new ICTEngine(this);
            _marketStateEngine = new MarketStateEngine(this);
            _riskManager = new RiskManager(this);
            _tradeManager = new TradeManager(this, $"ULTIMATUM_v2.3_{MagicNumber}");

            Logger.Info("All services and engines initialized successfully.");
        }

        protected override void OnTick()
        {
            // This is the main trading loop. It will become more complex.
            // For now, this just demonstrates the intended flow of data and decisions.

            // 1. Update the market state using the Wyckoff engine
            _marketStateEngine.UpdateState();

            // 2. Look for high-probability trading signals using the ICT engine
            var ictPattern = _ictEngine.FindLastPattern();

            // 3. If a signal is found, proceed to evaluation
            if (ictPattern != null)
            {
                Logger.Info($"ICT Pattern found: {ictPattern.PatternType} ({ictPattern.Direction}) at {ictPattern.PriceLevel}");

                // 4. If trading is enabled, calculate risk and potentially execute a trade
                if (EnableTrading)
                {
                    // This is example logic. A real trade would require more checks (e.g., from ML engine, news filter).
                    // double stopLossPips = 20; // Example SL
                    // double volume = _riskManager.CalculateVolumeInLots(stopLossPips, RiskPercent);
                    // _tradeManager.ExecuteMarketOrder(ictPattern.Direction, volume, stopLossPips, takeProfitPips: 40);
                }
            }
        }

        protected override void OnStop()
        {
            Logger.Info("--- Bot Stopped ---");
        }
    }
}
