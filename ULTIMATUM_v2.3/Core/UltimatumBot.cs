using System;
using cAlgo.API;
using Ultimatum.Utils;
using Ultimatum.Core.Configuration;
using Ultimatum.Engines;
using Ultimatum.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ultimatum.Core
{
    [Robot("ULTIMATUM v2.3", TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class UltimatumBot : Robot
    {
        #region Parameters

        // ... (Parameters are unchanged)

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

            if (EnableML) _mlEngine.TrainModel(new List<double[]>(), new int[0]);
            if (EnableClusterAnalysis) _clusterEngine.UpdateModel(new List<double[]>());
            if (EnableRealTimeData) Timer.Start(TimeSpan.FromHours(1), FetchExternalData);

            Logger.Info("All services and engines initialized successfully.");
        }

        private async void FetchExternalData()
        {
            try
            {
                Logger.Info("--- Periodically fetching external API data ---");
                if (EnableNewsFilter && !string.IsNullOrEmpty(NewsAPIKey))
                {
                    var news = await _apiManager.GetNews(NewsAPIKey);
                    if (news?.Articles != null && news.Articles.Any())
                        Logger.Info($"Latest news: {news.Articles.First().Title}");
                }
                var cryptoData = await _apiManager.GetCryptoData("bitcoin,ethereum");
                if (cryptoData != null && cryptoData.ContainsKey("bitcoin"))
                    Logger.Info($"BTC Price: ${cryptoData["bitcoin"]["usd"]}");

                var indexData = await _apiManager.GetIndexData(AlphaVantageAPIKey, "SPY");
                if (indexData?.TimeSeriesDaily != null && indexData.TimeSeriesDaily.Any())
                    Logger.Info($"S&P 500 latest close: {indexData.TimeSeriesDaily.First().Value.Close}");
            }
            catch (Exception ex)
            {
                Logger.Error("An error occurred during the scheduled API data fetch.", ex);
            }
        }

        protected override void OnTick()
        {
            if (EnableClusterAnalysis && _clusterEngine.IsModelReady) _clusterEngine.AnalyzeCurrentRegime();
            _marketStateEngine.UpdateState();
            var ictPattern = _ictEngine.FindLastPattern();

            if (EnableML && _mlEngine.IsTrained)
            {
                int barIndex = Bars.Count - 2;
                if (barIndex < 0) return;
                var features = _mlEngine.GenerateFeatures(barIndex);
                var prediction = _mlEngine.Predict(features);
                if (prediction.HasPrediction && prediction.Confidence > MLConfidenceThreshold)
                    Logger.Info($"ML Prediction: {prediction.Direction} with {prediction.Confidence:P1} confidence.");
            }

            if (ictPattern != null)
            {
                Logger.Info($"ICT Pattern found: {ictPattern.PatternType} ({ictPattern.Direction}) at {ictPattern.PriceLevel}");
                if (EnableTrading)
                {
                    // TODO: Combine all signals for a final decision.
                }
            }
        }

        protected override void OnStop()
        {
            Timer.Stop();
            Logger.Info("--- Bot Stopped ---");
        }
    }
}
