namespace DummyApp.FileService.Functions.Services;

public interface IQrCodeService
{
    byte[] GenerateQrCodePng(string text, int pixelsPerModule = 20);
}
