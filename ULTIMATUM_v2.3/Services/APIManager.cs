using System;
using System.Net.Http;
using System.Threading.Tasks;
using Ultimatum.Models.ApiResponse;
using Ultimatum.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Ultimatum.Services
{
    public class APIManager
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public APIManager()
        {
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("UltimatumTradingBot/2.3");
        }

        public async Task<NewsApiResponse> GetNews(string apiKey, string query = "forex")
        {
            var requestUrl = $"https://newsapi.org/v2/everything?q={Uri.EscapeDataString(query)}&apiKey={apiKey}&sortBy=publishedAt&pageSize=20";
            try
            {
                Logger.Info($"Fetching news for query: '{query}'");
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to fetch news from NewsAPI. Status: {response.StatusCode}. Reason: {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<NewsApiResponse>(jsonContent);
            }
            catch (Exception ex)
            {
                Logger.Error("An exception occurred while fetching news from NewsAPI.", ex);
                return null;
            }
        }

        public async Task<Dictionary<string, Dictionary<string, double>>> GetCryptoData(string coinIds = "bitcoin,ethereum")
        {
            var requestUrl = $"https://api.coingecko.com/api/v3/simple/price?ids={coinIds}&vs_currencies=usd&include_24hr_change=true";
            try
            {
                Logger.Info($"Fetching crypto data for: {coinIds}");
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to fetch crypto data from CoinGecko. Status: {response.StatusCode}. Reason: {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                var priceData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, double>>>(jsonContent);
                Logger.Info($"Successfully fetched price data for {priceData?.Count ?? 0} coins from CoinGecko.");
                return priceData;
            }
            catch (Exception ex)
            {
                Logger.Error("An exception occurred while fetching crypto data from CoinGecko.", ex);
                return null;
            }
        }

        /// <summary>
        /// Fetches time series data for a given symbol from Alpha Vantage.
        /// </summary>
        /// <param name="apiKey">Your Alpha Vantage API key.</param>
        /// <param name="symbol">The stock or index symbol (e.g., "SPY").</param>
        /// <returns>An AlphaVantageApiResponse object or null on failure.</returns>
        public async Task<AlphaVantageApiResponse> GetIndexData(string apiKey, string symbol = "SPY")
        {
            var requestUrl = $"https://www.alphavantage.co/query?function=TIME_SERIES_DAILY&symbol={symbol}&apikey={apiKey}";
            try
            {
                Logger.Info($"Fetching index data for symbol: {symbol}");
                var response = await _httpClient.GetAsync(requestUrl);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Failed to fetch index data from Alpha Vantage. Status: {response.StatusCode}. Reason: {await response.Content.ReadAsStringAsync()}");
                    return null;
                }
                var jsonContent = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<AlphaVantageApiResponse>(jsonContent);

                if (!string.IsNullOrEmpty(data?.ErrorMessage))
                {
                    Logger.Error($"Alpha Vantage API returned an error: {data.ErrorMessage}");
                    return null;
                }

                Logger.Info($"Successfully fetched time series data for {data?.MetaData?["2. Symbol"]}.");
                return data;
            }
            catch (Exception ex)
            {
                Logger.Error($"An exception occurred while fetching index data from Alpha Vantage for symbol {symbol}.", ex);
                return null;
            }
        }
    }
}
