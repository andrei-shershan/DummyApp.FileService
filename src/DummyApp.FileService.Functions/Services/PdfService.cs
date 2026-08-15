using System.IO;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using QuestPDF.Helpers;

namespace DummyApp.FileService.Functions.Services;

public sealed class PdfService : IPdfService
{
    private readonly IQrCodeService _qrCodeService;

    public PdfService(IQrCodeService qrCodeService)
    {
        _qrCodeService = qrCodeService;
    }

    public byte[] GenerateTestPdf(string url)
    {
        var qrBytes = _qrCodeService.GenerateQrCodePng(url);

        using var ms = new MemoryStream();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Content().Column(column =>
                {
                    column.Item().Text("Тестовый PDF").FontSize(22).Bold();
                    column.Item().PaddingTop(10).Text("Это PDF, сгенерированный внутри FileService.");
                    column.Item().PaddingTop(15).Text("Ссылка:").SemiBold();
                    column.Item().Text(text => text.Hyperlink(url, url));

                    column.Item().PaddingTop(15).Text("Таблица:").SemiBold();
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(1);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Название");
                            header.Cell().Element(CellStyle).Text("Кол-во");
                        });

                        table.Cell().Element(CellStyle).Text("Продукт A");
                        table.Cell().Element(CellStyle).Text("10");

                        table.Cell().Element(CellStyle).Text("Продукт B");
                        table.Cell().Element(CellStyle).Text("5");

                        table.Cell().Element(CellStyle).Text("Продукт C");
                        table.Cell().Element(CellStyle).Text("3");

                        static IContainer CellStyle(IContainer container)
                        {
                            return container.PaddingVertical(5).PaddingHorizontal(5);
                        }
                    });

                    column.Item().PaddingTop(20).Text("QR-код:").SemiBold();
                    column.Item().Image(qrBytes).FitWidth();
                });
            });
        });

        document.GeneratePdf(ms);
        return ms.ToArray();
    }

}
