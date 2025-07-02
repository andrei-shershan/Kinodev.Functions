using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Kinodev.Functions.Services;
using Kinodev.Functions.Configurations;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure GotenbergPdfSettings from configuration section
        services.Configure<GotenbergPdfSettings>(
            context.Configuration.GetSection("GotenbergPdfSettings"));

        services.Configure<Api2PdfSettings>(
            context.Configuration.GetSection("Api2PdfSettings"));

        var pdfServiceName = context.Configuration["PdfServiceName"];
        System.Console.WriteLine($"Using PDF Service: {pdfServiceName}");
        if (string.IsNullOrEmpty(pdfServiceName))
        {
            throw new InvalidOperationException("PdfServiceName configuration is required.");
        }

        if(pdfServiceName.Equals("Gotenberg", StringComparison.OrdinalIgnoreCase))
        {
            services.AddTransient<IPdfService, GotenbergPdf>();
        }
        else if(pdfServiceName.Equals("Api2Pdf", StringComparison.OrdinalIgnoreCase))
        {
            services.AddTransient<IPdfService, Api2PdfService>();
        }

        services.AddHttpClient<GotenbergPdf>();
        services.AddHttpClient<Api2PdfService>();
    })
    .Build();

host.Run();
