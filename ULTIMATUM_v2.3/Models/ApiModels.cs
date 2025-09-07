using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ultimatum.Models.ApiResponse
{
    // --- NewsAPI Models ---
    public class NewsArticle
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public System.DateTime PublishedAt { get; set; }
    }

    public class NewsApiResponse
    {
        public string Status { get; set; }
        public int TotalResults { get; set; }
        public List<NewsArticle> Articles { get; set; }
    }

    // --- CoinGecko Models ---
    // The CoinGecko response is a dynamic dictionary, so a specific model is not used.
    // The APIManager deserializes directly into Dictionary<string, Dictionary<string, double>>.

    // --- Alpha Vantage Models ---
    public class AlphaVantageTimeSeriesEntry
    {
        [JsonProperty("1. open")]
        public string Open { get; set; }
        [JsonProperty("2. high")]
        public string High { get; set; }
        [JsonProperty("3. low")]
        public string Low { get; set; }
        [JsonProperty("4. close")]
        public string Close { get; set; }
        [JsonProperty("5. volume")]
        public string Volume { get; set; }
    }

    public class AlphaVantageApiResponse
    {
        [JsonProperty("Meta Data")]
        public Dictionary<string, string> MetaData { get; set; }

        [JsonProperty("Time Series (Daily)")]
        public Dictionary<string, AlphaVantageTimeSeriesEntry> TimeSeriesDaily { get; set; }

        // This property will be populated if the API returns an error
        [JsonProperty("Error Message")]
        public string ErrorMessage { get; set; }
    }
}
