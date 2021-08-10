using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DofusMarket.Bot.Models;
using Microsoft.Extensions.Logging;

namespace DofusMarket.Bot.Services
{
    internal class DofusMetrics : IAsyncDisposable
    {
        private readonly ILogger _logger;
        private const int MaxBufferSize = 100;

        private readonly HttpClient _httpClient;
        private readonly List<ItemPrice> _bufferedItemPrices;

        public DofusMetrics(string apiUrl, string username, string password, ILogger logger)
        {
            _logger = logger;
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
            _httpClient.DefaultRequestHeaders.Add("Authorization",
                "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            _bufferedItemPrices = new List<ItemPrice>(MaxBufferSize);
        }

        public void WriteItemPrice(ItemPrice itemPrice)
        {
            _bufferedItemPrices.Add(itemPrice);
            if (_bufferedItemPrices.Count >= MaxBufferSize)
            {
                _ = FlushAsync();
            }
        }

        private async Task FlushAsync()
        {
            // Must use _bufferedItemPrices before yielding to avoid any concurrent access on the list.
            string json = JsonSerializer.Serialize(_bufferedItemPrices);
            _bufferedItemPrices.Clear();

            try
            {
                var res = await _httpClient.PostAsync("item-prices",
                    new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json));
                res.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending metrics");
            }
        }

        public async ValueTask DisposeAsync()
        {
            _httpClient.Dispose();
            await FlushAsync();
        }
    }
}
