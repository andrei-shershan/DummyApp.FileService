namespace DummyApp.FileService.Functions.Models;

public sealed class OrderSummaryParameters
{
    public IEnumerable<OrderItemParameters> Items { get; init; } = Array.Empty<OrderItemParameters>();
    public string Status { get; init; } = string.Empty;
    public OrderAddressParameters? Address { get; init; }
    public string? QrCodeBase64 { get; init; }
}
