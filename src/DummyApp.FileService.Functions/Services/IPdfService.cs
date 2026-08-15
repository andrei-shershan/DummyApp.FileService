using System.Text.Json;
using DummyApp.FileService.Functions.Models;

namespace DummyApp.FileService.Functions.Services;

public interface IPdfService
{
    byte[] GeneratePdf(GeneratePdfRequest request);
}
