using System;
using cAlgo.API;
using Ultimatum.Utils;
using Ultimatum.Core.Configuration;
using Ultimatum.Engines;
using Ultimatum.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ultimatum.Models;

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
        [Parameter("--- API Settings ---", Group = "API", DefaultValue = "")]
        public string ApiSettingsSeparator { get; set; }
        [Parameter("News API Key", DefaultValue = "889bc03accb645a984e07f265b4f16f9", Group = "API")]
        public string NewsAPIKey { get; set; }
        [Parameter("Alpha Vantage API Key", DefaultValue = "XPJXS50Q2ZNZS76P", Group = "API")]
        public string AlphaVantageAPIKey { get; set; }
        [Parameter("Enable News Filter", DefaultValue = true, Group = "API")]
        public bool EnableNewsFilter { get; set; }
        [Parameter("Enable Real-time Data", DefaultValue = true, Group = "API")]
        public bool EnableRealTimeData { get; set; }
        #endregion

        #region Services and Engines
        private APIManager _apiManager;
        private ICTEngine _ictEngine;
        private MarketStateEngine _marketStateEngine;
        private RiskManager _riskManager;
        private TradeManager _tradeManager;
        private MLEngine _mlEngine;
        private ClusterEngine _clusterEngine;
        private EnsembleEngine _ensembleEngine;
        #endregion

        protected override void OnStart()
        {
            Logger.Initialize(this);
            Logger.Info("--- Bot Starting ---");

            _apiManager = new APIManager();
            _ictEngine = new ICTEngine(this);
            _marketStateEngine = new MarketStateEngine(this);
            _riskManager = new RiskManager(this);
            _tradeManager = new TradeManager(this, $"ULTIMATUM_v2.3_{MagicNumber}");
            _mlEngine = new MLEngine(this);
            _clusterEngine = new ClusterEngine(this);
            _ensembleEngine = new EnsembleEngine(_ictEngine, _mlEngine, _clusterEngine, _marketStateEngine, MLConfidenceThreshold);

            if (EnableML) _mlEngine.TrainModel(new List<double[]>(), new int[0]);
            if (EnableClusterAnalysis) _clusterEngine.UpdateModel(new List<double[]>());
            if (EnableRealTimeData) Timer.Start(TimeSpan.FromHours(1), FetchExternalData);

            Logger.Info("All services and engines initialized successfully.");
        }

        private async void FetchExternalData()
        {
            // ... (method is unchanged)
        }

        protected override void OnTick()
        {
            int barIndex = Bars.Count - 2;
            if (barIndex < 1) return; // Not enough history

            var decision = _ensembleEngine.GenerateDecision(barIndex);

            if (EnableTrading && decision.Action != TradingAction.Hold)
            {
                if (Positions.Any(p => p.Label == _tradeManager.BotLabel)) return;

                Logger.Info($"DECISION: {decision.Action} with {decision.ConfidenceScore:P1} confidence.");
                Logger.Info($"REASON: {string.Join(", ", decision.Reasoning)}");

                var tradeType = decision.Action == TradingAction.Buy ? TradeType.Buy : TradeType.Sell;
                double volumeInLots = _riskManager.CalculateVolumeInLots(decision.StopLossInPips.Value, RiskPercent);

                _tradeManager.ExecuteMarketOrder(tradeType, volumeInLots, decision.StopLossInPips, decision.TakeProfitInPips);
            }
        }

        protected override void OnStop()
        {
            Timer.Stop();
            Logger.Info("--- Bot Stopped ---");
        }
    }
}
