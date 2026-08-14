namespace DummyApp.FileService.Functions.Services;

public interface IPdfService
{
    byte[] GenerateTestPdf(string url);
}
