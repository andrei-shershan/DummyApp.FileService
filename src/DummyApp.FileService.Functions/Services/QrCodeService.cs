using QRCoder;

namespace DummyApp.FileService.Functions.Services;

public sealed class QrCodeService : IQrCodeService
{
    public byte[] GenerateQrCodePng(string text, int pixelsPerModule = 20)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var pngQrCode = new PngByteQRCode(qrData);
        return pngQrCode.GetGraphic(pixelsPerModule);
    }
}
