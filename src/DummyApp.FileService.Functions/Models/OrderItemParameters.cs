namespace DummyApp.FileService.Functions.Models;

public sealed class OrderItemParameters
{
    public Guid OrderId { get; init; }
    public Guid ArtworkId { get; init; }
    public int Quantity { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string ImgUrl { get; init; } = string.Empty;
    public string ThumbnailUrl { get; init; } = string.Empty;
    public int PrintSizeId { get; init; }
    public string PrintSizeName { get; init; } = string.Empty;
    public int PriceId { get; init; }
    public decimal PriceValue { get; init; }
}
