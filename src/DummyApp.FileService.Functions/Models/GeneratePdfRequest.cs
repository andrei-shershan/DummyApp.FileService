using System.Text.Json;

namespace DummyApp.FileService.Functions.Models;

public sealed class GeneratePdfRequest
{
    public PdfTemplate Template { get; init; } = PdfTemplate.Unknown;
    public JsonElement? Parameters { get; init; }
}
