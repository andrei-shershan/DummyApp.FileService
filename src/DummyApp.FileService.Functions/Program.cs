using DummyApp.FileService.Functions.Extensions;
using DummyApp.FileService.Functions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuestPDF;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

var host = new HostBuilder()
    .ConfigureAppConfiguration(config => config.AddKeyVaultFromConfiguration())
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddSingleton<IPdfService, PdfService>();
    })
    .Build();

host.Run();
