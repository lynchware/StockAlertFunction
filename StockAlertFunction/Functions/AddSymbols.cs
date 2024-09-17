using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Azure.Functions.Worker.Http;

namespace StockAlertFunction.Functions
{
	public class AddSymbols
	{
		private readonly ILogger<AddSymbols> _logger;
		private readonly IConfiguration _configuration;

		public AddSymbols(ILogger<AddSymbols> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		[Function("AddSymbols")]
		public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			dynamic data = JsonConvert.DeserializeObject<dynamic>(requestBody);
			JArray codes = data?.ToObject<JArray>();

			if(codes is null || codes.Count < 1)
			{
				return new BadRequestObjectResult("Missing 'code' in the request body.");
			}

			// Update local.settings.json file
			var settingsFilePath = Path.Combine(Environment.CurrentDirectory, "local.settings.json");
			var json = File.ReadAllText(settingsFilePath);
			dynamic jsonObj = JsonConvert.DeserializeObject(json);

			var stockSymbols = jsonObj["StockSymbols"].ToObject<JArray>();
			var updatedStockSymbols = new JArray(stockSymbols.Merge(codes));
			jsonObj["StockSymbols"] = updatedStockSymbols;

			string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
			File.WriteAllText(settingsFilePath, output);

			_logger.LogInformation($"Added new stock symbols: {codes}");

			return new OkObjectResult($"Added new stock symbols: {codes}");
		}
	}
}
