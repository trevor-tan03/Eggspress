using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using System.IO.Compression;
namespace backend.Controllers;

[Route("api/download")]
[ApiController]
public class DownloadController : ControllerBase
{
    private readonly IBoxRepository _boxRepository;

    public DownloadController(IBoxRepository boxRepository)
    {
        _boxRepository = boxRepository;
    }


    [HttpGet("{code}/{fileName}")]
    public async Task<IActionResult> DownloadFile(string code, string fileName)
    {
        var box = await _boxRepository.GetBox(code);
        var boxPath = Path.Combine(Directory.GetCurrentDirectory(), "Boxes", code);

        if (box == null || !Directory.Exists(boxPath))
            return NotFound($"Box '{code}' does not exist.");

        var filePath = Path.Combine(boxPath, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound($"File '{fileName}' does not exist.");

        var contentType = "application/octet-stream";
        return PhysicalFile(filePath, contentType, fileName);
    }

    [HttpGet("all/{code}")]
    public async Task<IActionResult> DownloadAllFiles(string code)
    {
        var box = await _boxRepository.GetBox(code);
        var boxPath = Path.Combine(Directory.GetCurrentDirectory(), "Boxes", code);

        if (box == null || !Directory.Exists(boxPath))
            return NotFound($"Box '{code}' does not exist.");

        var tempZipPath = Path.Combine(Path.GetTempPath(), $"{code}.zip");

        if (System.IO.File.Exists(tempZipPath))
            System.IO.File.Delete(tempZipPath);

        ZipFile.CreateFromDirectory(boxPath, tempZipPath);

        var zipBytes = await System.IO.File.ReadAllBytesAsync(tempZipPath);
        System.IO.File.Delete(tempZipPath);

        return File(zipBytes, "application/zip", $"{code}.zip");
    }
}