namespace Kinodev.Functions.Helpers
{
    using System.Net.Http;

    public static class HttpClientHelper
    {
        public static HttpClient GetHttpClient(string baseUrl, bool ignoreSslErrors = false)
        {
            var handler = new HttpClientHandler();
            // If ignoreSslErrors is true, set the ServerCertificateCustomValidationCallback to accept any certificate
            if (ignoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
            };

            var client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            return client;
        }

    }
}