using System.Text;
using System.Text.Json;
using Kinodev.Functions.Configurations;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kinodev.Functions.Services;

public class Api2PdfService : IPdfService
{
    private readonly HttpClient _httpClient;

    private readonly Api2PdfSettings _settings;

    private readonly ILogger<Api2PdfService> _logger;

    public Api2PdfService(HttpClient httpClient, IOptions<Api2PdfSettings> settings, ILogger<Api2PdfService> logger)    
    {
        _logger = logger;

        _settings = settings.Value;

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/pdf");
        _httpClient.DefaultRequestHeaders.Add("Authorization",
            $"{_settings.ApiKey}");
    }

    public async Task<byte[]> GeneratePdfAsync(string htmlContent)
    {
        var serializedContent = JsonSerializer.Serialize(new
        {
            html = htmlContent
        }, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        var request = new HttpRequestMessage(HttpMethod.Post, "chrome/pdf/html?outputBinary=true");
        request.Content = new StringContent(serializedContent, Encoding.UTF8, "text/html");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }
}