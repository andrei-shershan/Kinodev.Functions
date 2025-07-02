using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using Kinodev.Functions.Services;
using Kinodev.Functions.Models;

namespace Kinodev.Functions
{
    public class GetPdfFunction
    {
        private readonly ILogger _logger;
        private readonly IPdfService _pdfService;

        public GetPdfFunction(ILoggerFactory loggerFactory, IPdfService pdfService)
        {
            _logger = loggerFactory.CreateLogger<GetPdfFunction>();
            _pdfService = pdfService;
        }

        [Function("GetPdf")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
        {
            _logger.LogInformation("GetPdf function processing a request.");

            try
            {
                // Read and deserialize the request body
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                
                if (string.IsNullOrWhiteSpace(requestBody))
                {
                    _logger.LogWarning("Request body is empty");
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("Request body cannot be empty");
                    return badRequestResponse;
                }

                var pdfRequest = JsonSerializer.Deserialize<PdfGenerationRequest>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (pdfRequest == null || string.IsNullOrWhiteSpace(pdfRequest.Html))
                {
                    _logger.LogWarning("Invalid request: HTML content is missing or empty");
                    var badRequestResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badRequestResponse.WriteStringAsync("HTML content is required");
                    return badRequestResponse;
                }

                // Generate PDF from HTML
                byte[] pdfBytes = await _pdfService.GeneratePdfAsync(pdfRequest.Html);

                // Create response with PDF content
                var response = req.CreateResponse(HttpStatusCode.OK);
                response.Headers.Add("Content-Type", "application/pdf");
                response.Headers.Add("Content-Disposition", "attachment; filename=generated.pdf");
                
                await response.WriteBytesAsync(pdfBytes);

                _logger.LogInformation("PDF generated successfully, size: {Size} bytes", pdfBytes.Length);
                return response;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing request body");
                var errorResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await errorResponse.WriteStringAsync("Invalid JSON in request body");
                return errorResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF");
                var errorResponse = req.CreateResponse(HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync($"Error generating PDF: {ex.Message}");
                return errorResponse;
            }
        }
    }
}
