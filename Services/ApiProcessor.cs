using System.Text;
using Kinodev.Functions.Configurations;
using Kinodev.Functions.Helpers;
using Microsoft.Extensions.Options;

namespace Kinodev.Functions.Services
{
    public class ApiProcessor : IApiProcessor
    {
        private readonly FunctionsServiceApiUrlsSettings _settings;

        public ApiProcessor(IOptions<FunctionsServiceApiUrlsSettings> functionsServiceApiUrlsSettings)
        {
            _settings = functionsServiceApiUrlsSettings.Value;
        }

        public async Task ProcessOrderCompletedAsync(string stringBody)
        {
            var content = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var client = HttpClientHelper.GetHttpClient(_settings.StorageServiceUrl, _settings.IgnoreSslErrors);

            var response = await client.PostAsync("api/files/process-order-completed", content);
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Failed to process order completed. Status code: {response.StatusCode}, Error: {errorContent}");
            }
        }

        public async Task ProcessOrderFileCreatedAsync(string stringBody)
        {
            var content = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var client = HttpClientHelper.GetHttpClient(_settings.DomainServiceUrl, _settings.IgnoreSslErrors);

            var response = await client.PostAsync("api/orders/process-order-file-created", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to process order file created. Status code: {response.StatusCode}");
            }
        }

        public async Task ProcessOrderFileUrlAddedAsync(string stringBody)
        {
            var content = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var client = HttpClientHelper.GetHttpClient(_settings.EmailServiceUrl, _settings.IgnoreSslErrors);

            var response = await client.PostAsync("api/emails/process-order-file-url-added", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to process order file added. Status code: {response.StatusCode}");
            }
        }

        public async Task ProcessEmailSentAsync(string stringBody)
        {
            var content = new StringContent(stringBody, Encoding.UTF8, "application/json");

            var client = HttpClientHelper.GetHttpClient(_settings.DomainServiceUrl, _settings.IgnoreSslErrors);

            var response = await client.PostAsync("api/orders/process-email-sent", content);
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to process order file url added. Status code: {response.StatusCode}");
            }
        }
    }
}