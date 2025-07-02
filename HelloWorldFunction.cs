using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Kinodev.Functions
{
    public class HelloWorldFunction
    {
        private readonly ILogger _logger;

        public HelloWorldFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<HelloWorldFunction>();
        }

        [Function("HelloWorld")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Hello World");

            return response;
        }
    }
}
