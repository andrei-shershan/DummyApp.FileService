namespace DummyApp.FileService.Functions.Models;

public sealed class GenerateQrCodeRequest
{
    public string? Text { get; set; }
    public string? FileName { get; set; }
    public int PixelsPerModule { get; set; } = 20;
}
