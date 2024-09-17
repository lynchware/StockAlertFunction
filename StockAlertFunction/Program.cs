using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Configuration;
using StockAlertFunction.Services;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureAppConfiguration((context, config) =>
	{
		config.AddJsonFile("local.settings.json", optional: false, reloadOnChange: true);
	})
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddMemoryCache();
		services.AddScoped<IScopedCacheService, ScopedCacheService>();
		services.AddSingleton<IHttpService, HttpService>();
	})
	.Build();

host.Run();
