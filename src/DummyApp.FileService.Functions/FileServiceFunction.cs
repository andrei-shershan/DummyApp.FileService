using System.IO;
using System.Net;
using System.Text.Json;
using DummyApp.FileService.Functions.Models;
using DummyApp.FileService.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace DummyApp.FileService.Functions;

public sealed class FileServiceFunction
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly IPdfService _pdfService;
    private readonly ILogger<FileServiceFunction> _logger;

    public FileServiceFunction(IPdfService pdfService, ILogger<FileServiceFunction> logger)
    {
        _pdfService = pdfService;
        _logger = logger;
    }

    [Function("GeneratePdf")]
    public async Task<HttpResponseData> Run(
#if DEBUG
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "file/pdf")]
#else
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "file/pdf")]
#endif
        HttpRequestData req,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("GeneratePdf triggered. Method: {Method}, Url: {Url}", req.Method, req.Url);

        GeneratePdfRequest? request;
        try
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync(cancellationToken);
            request = string.IsNullOrWhiteSpace(body)
                ? new GeneratePdfRequest()
                : JsonSerializer.Deserialize<GeneratePdfRequest>(body, JsonOptions);
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Invalid JSON in generate PDF request.");
            return CreateBadRequest(req, "Invalid JSON in request body.");
        }

        if (request is null)
        {
            _logger.LogWarning("GeneratePdf request body deserialized to null.");
            return CreateBadRequest(req, "Request body is required.");
        }

        var pdfBytes = _pdfService.GenerateTestPdf(request.Url ?? "https://example.com");
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/pdf");
        response.Headers.Add("Content-Disposition", "attachment; filename=generated.pdf");
        await response.WriteBytesAsync(pdfBytes, cancellationToken);
        return response;
    }

    private static HttpResponseData CreateBadRequest(HttpRequestData req, string message)
    {
        var response = req.CreateResponse(HttpStatusCode.BadRequest);
        response.WriteString(message);
        return response;
    }
}
