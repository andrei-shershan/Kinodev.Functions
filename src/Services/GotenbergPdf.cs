using System.Net.Http.Headers;
using System.Text;
using Kinodev.Functions.Configurations;
using Microsoft.Extensions.Options;

namespace Kinodev.Functions.Services;

public class GotenbergPdf : IPdfService
{
    private readonly HttpClient _httpClient;
    private readonly GotenbergPdfSettings _gotenbergPdfServiceSettings;

    public GotenbergPdf(
        HttpClient httpClient,
        IOptions<GotenbergPdfSettings> options
    )
    {
        _gotenbergPdfServiceSettings = options.Value;

        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(_gotenbergPdfServiceSettings.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/pdf");
    }

    public async Task<byte[]> GeneratePdfAsync(string htmlContent)
    {
        try
        {
            using var form = new MultipartFormDataContent();
            var stringContent = new StringContent(htmlContent, Encoding.UTF8, "text/html");
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("text/html");

            form.Add(stringContent, "files", "index.html");

            var response = await _httpClient.PostAsync("forms/chromium/convert/html", form);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync();
            }

            throw new Exception($"PDF generation failed with status code: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            throw new Exception($"Error generating PDF: {ex.Message}", ex);
        }
    }
}