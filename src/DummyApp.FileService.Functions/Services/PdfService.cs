using System.IO;
using System.Text.Json;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;
using DummyApp.FileService.Functions.Models;

namespace DummyApp.FileService.Functions.Services;

public sealed class PdfService : IPdfService
{
    private readonly IQrCodeService _qrCodeService;

    public PdfService(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    public byte[] GeneratePdf(GeneratePdfRequest request)
    {
        if (request.Parameters is null)
        {
            throw new ArgumentException("Template parameters are required.", nameof(request));
        }

        return request.Template switch
        {
            PdfTemplate.OrderSummary => GenerateOrderSummaryPdf(request.Parameters.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(request.Template), "Unsupported PDF template.")
        };
    }

    private static OrderSummaryParameters DeserializeOrderSummaryParameters(JsonElement parameters)
    {
        return JsonSerializer.Deserialize<OrderSummaryParameters>(parameters.GetRawText(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? throw new JsonException("Unable to deserialize OrderSummaryParameters.");
    }

    private byte[] GenerateOrderSummaryPdf(JsonElement parameters)
    {
        var orderSummary = DeserializeOrderSummaryParameters(parameters);
        var qrCodeBase64 = orderSummary.QrCodeBase64;
        if (string.IsNullOrWhiteSpace(qrCodeBase64))
        {
            throw new JsonException("QrCodeBase64 is required for OrderSummary parameters.");
        }

        var qrBytes = GetQrCodeBytes(qrCodeBase64);

        using var ms = new MemoryStream();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Text("Order Summary").FontSize(22).Bold().SemiBold();

                page.Content().Column(column =>
                {
                    column.Item().PaddingBottom(15).Row(row =>
                    {
                        row.ConstantItem(220).Column(qr =>
                        {
                            qr.Item().Text("QR Code").FontSize(12).Bold();
                            qr.Item().PaddingTop(5).Width(200).Image(qrBytes);
                        });

                        row.RelativeItem().Column(details =>
                        {
                            details.Item().Text("Order Details").FontSize(12).Bold();
                            details.Item().Text($"Status: {orderSummary.Status}").FontSize(11).SemiBold();

                            if (orderSummary.Address is not null)
                            {
                                details.Item().PaddingTop(10).Text("Delivery Address").FontSize(12).Bold();
                                details.Item().Text(orderSummary.Address.FirstName + " " + orderSummary.Address.LastName);
                                details.Item().Text(orderSummary.Address.Email);
                                details.Item().Text(orderSummary.Address.Phone);
                                details.Item().Text($"{orderSummary.Address.Street} {orderSummary.Address.HouseNumber}");
                                details.Item().Text($"{orderSummary.Address.PostalCode} {orderSummary.Address.City}");
                                details.Item().Text(orderSummary.Address.Country);
                            }
                        });
                    });

                    column.Item().PaddingBottom(5).Text("Order Items").FontSize(12).Bold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Item").WrapAnywhere();
                            header.Cell().Element(CellStyle).Text("Description").WrapAnywhere();
                            header.Cell().Element(CellStyle).Text("Quantity").WrapAnywhere();
                            header.Cell().Element(CellStyle).Text("Size").WrapAnywhere();
                            header.Cell().Element(CellStyle).Text("Price").WrapAnywhere();
                        });

                        foreach (var item in orderSummary.Items)
                        {
                            table.Cell().Element(CellStyle).Text(item.Name).WrapAnywhere();
                            table.Cell().Element(CellStyle).Text(item.Description).WrapAnywhere();
                            table.Cell().Element(CellStyle).Text(item.Quantity.ToString()).WrapAnywhere();
                            table.Cell().Element(CellStyle).Text(item.PrintSizeName).WrapAnywhere();
                            table.Cell().Element(CellStyle).Text(item.PriceValue.ToString("F2")).WrapAnywhere();
                        }

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.Border(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
                        }
                    });
                });
            });
        });

        document.GeneratePdf(ms);
        return ms.ToArray();
    }

    private static byte[] GetQrCodeBytes(string qrCodeBase64)
    {
        const string dataPrefix = "data:image/png;base64,";
        if (qrCodeBase64.StartsWith(dataPrefix, StringComparison.OrdinalIgnoreCase))
        {
            qrCodeBase64 = qrCodeBase64[dataPrefix.Length..];
        }

        return Convert.FromBase64String(qrCodeBase64);
    }
}
