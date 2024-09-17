var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.StockAlertFunction>("stockalertfunction");

builder.Build().Run();
