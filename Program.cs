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

        services.Configure<FunctionsServiceApiUrlsSettings>(
            context.Configuration.GetSection("FunctionsServiceApiUrlsSettings"));

        // Configure HttpClientSettings from configuration section
        var pdfServiceName = context.Configuration["PdfServiceName"];
        if (string.IsNullOrEmpty(pdfServiceName))
        {
            throw new InvalidOperationException("PdfServiceName configuration is required.");
        }

        if (pdfServiceName.Equals("Gotenberg", StringComparison.OrdinalIgnoreCase))
        {
            services.AddTransient<IPdfService, GotenbergPdf>();
        }
        else if (pdfServiceName.Equals("Api2Pdf", StringComparison.OrdinalIgnoreCase))
        {
            services.AddTransient<IPdfService, Api2PdfService>();
        }

        services.AddTransient<IApiProcessor, ApiProcessor>();

        services.AddHttpClient<GotenbergPdf>();
        services.AddHttpClient<Api2PdfService>();
    })
    .Build();

host.Run();
