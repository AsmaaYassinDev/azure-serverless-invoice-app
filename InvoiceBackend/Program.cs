using InvoiceBackend;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        //services.AddApplicationInsightsTelemetryWorkerService();
        //services.ConfigureFunctionsApplicationInsights();

        // Read the connection string from local.settings.json safely
        string cosmosConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnectionString")
            ?? throw new InvalidOperationException("CosmosDBConnectionString is missing in configuration.");

        // Register the CosmosClient as a Singleton
        services.AddSingleton(sp => new CosmosClient(cosmosConnectionString));

        // Register your application services
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPdfGenerator, IronPdfGenerator>();
    })
    .Build();

host.Run();