using System;
using cAlgo.API;
using Ultimatum.Utils;

using Ultimatum.Core.Configuration;

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

        protected override void OnStart()
        {
            Logger.Initialize(this);
            Logger.Info("Bot starting...");
        }

        protected override void OnTick()
        {
            // Main trading logic will be executed on each tick
        }

        protected override void OnStop()
        {
            Logger.Info("Bot stopped.");
        }
    }
}
