using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAlertFunction.Services
{
	public class HttpService : IHttpService
	{
		private IConfiguration _configuration;
		private IScopedCacheService _cacheService;

		public HttpService(IConfiguration configuration, IScopedCacheService cacheService)
		{
			_configuration = configuration;
			_cacheService = cacheService;
		}

		public async Task<IDictionary<string, decimal>> GetStockPercentageChangesOverHour(string[] stockSymbols)
		{
			var stockPercentageChanges = _cacheService.Get<IDictionary<string, decimal>>("StockPercentageChanges") ?? new Dictionary<string, decimal>();
			if(stockPercentageChanges.Count > 0)
			{
				return stockPercentageChanges;
			}
			var baseUrl = _configuration.GetRequiredSection("APIs:TimeSeriesIntraday:BaseUrl").Value;
			var apiKey = _configuration.GetRequiredSection("APIs:TimeSeriesIntraday:APIKey");

			using HttpClient client = new HttpClient();

			foreach(var stockSymbol in stockSymbols)
			{
				string url = $"{baseUrl}?symbol={stockSymbol}&interval=60min&apikey={apiKey}";

				HttpResponseMessage response = await client.GetAsync(url);

				if(response.IsSuccessStatusCode)
				{
					string data = await response.Content.ReadAsStringAsync();
					var json = JsonConvert.DeserializeObject<dynamic>(data);

					if(json != null)
					{
						var timeSeries = json["Time Series (60min)"] as JObject;

						if(timeSeries != null)
						{
							var latestEntry = timeSeries.Properties().FirstOrDefault();
							var previousEntry = timeSeries.Properties().Skip(1).FirstOrDefault();
							if(latestEntry != null && previousEntry != null)
							{
								var latestTimestamp = latestEntry.Name;
								var latestData = latestEntry.Value;
								string latestPrice = latestData["4. close"].ToString();
								Console.WriteLine($"Latest price for {stockSymbol} at {latestTimestamp}: {latestPrice}");

								var previousTimestamp = previousEntry.Name;
								var previousData = previousEntry.Value;
								string previousPrice = previousData["4. close"].ToString();
								Console.WriteLine($"Previous price for {stockSymbol} at {previousTimestamp}: {previousPrice}");

								decimal latestPriceDecimal = decimal.Parse(latestPrice);
								decimal previousPriceDecimal = decimal.Parse(previousPrice);
								decimal percentageChange = (latestPriceDecimal - previousPriceDecimal) / previousPriceDecimal * 100;

								stockPercentageChanges.Add(stockSymbol, percentageChange);
							}
						}
						else
						{
							Console.WriteLine($"Time Series data not found for {stockSymbol}.");
							continue;
						}
					}
					else
					{
						Console.WriteLine($"Error parsing JSON data for {stockSymbol}.");
					}
				}
			}
			_cacheService.Set("StockPercentageChanges", stockPercentageChanges, DateTimeOffset.Now.AddMinutes(5));
			return stockPercentageChanges;
		}
	}
}
