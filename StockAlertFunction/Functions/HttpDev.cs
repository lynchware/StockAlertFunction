using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockAlertFunction.Services;

namespace HttpDev
{
	public class HttpDev
	{
		private readonly ILogger _logger;
		private readonly IConfiguration _configuration;
		private readonly IHttpService _httpService;

		public HttpDev(ILoggerFactory loggerFactory, IConfiguration configuration, IHttpService httpService)
		{
			_logger = loggerFactory.CreateLogger<HttpDev>();
			_configuration = configuration;
			_httpService = httpService;
		}

		[Function("FetchStockData")]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function,"get")] HttpRequestData req)
		{
			var incommingSymbol = req.Query["symbol"];
			//initialize stockSymbol
			string[] stockSymbols; 
			if(!string.IsNullOrEmpty(incommingSymbol))
			{
				stockSymbols = [incommingSymbol];
			}
			else
			{
				stockSymbols = _configuration.GetSection("StockSymbols").Get<string[]>() ?? [];
			}
			var stockPercentageChanges = await _httpService.GetStockPercentageChangesOverHour(stockSymbols);
			var stockNotification = stockPercentageChanges.Where(x => x.Value > 10.0m).ToDictionary(x => x.Key, x => x.Value);

			var responseMessage = req.CreateResponse(System.Net.HttpStatusCode.OK);
			await responseMessage.WriteAsJsonAsync(stockPercentageChanges);
			return responseMessage;
		}
	}
}
