using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using System.IO.Compression;
using Microsoft.AspNetCore.RateLimiting;
namespace backend.Controllers;

[Route("api/download")]
[ApiController]
public class DownloadController : ControllerBase
{
    private readonly IBoxRepository _boxRepository;
    private readonly IFileRepository _fileRepository;

    public DownloadController(IBoxRepository boxRepository, IFileRepository fileRepository)
    {
        _boxRepository = boxRepository;
        _fileRepository = fileRepository;
    }


    [HttpGet("{code}/{fileName}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> DownloadFile(string code, string fileName)
    {
        var box = await _boxRepository.GetBox(code);
        var boxPath = _boxRepository.GetBoxPath(code);

        if (box == null || !Directory.Exists(boxPath))
            return NotFound($"Box '{code}' does not exist.");

        var randomFileName = await _fileRepository.GetRandomFileName(code, fileName);
        if (randomFileName == null)
            return NotFound($"File '{fileName}' does not exist.");

        var filePath = Path.Combine(boxPath, randomFileName);

        if (!System.IO.File.Exists(filePath))
            return NotFound($"File '{fileName}' does not exist.");

        var contentType = "application/octet-stream";
        return PhysicalFile(filePath, contentType, fileName);
    }

    [HttpGet("all/{code}")]
    [EnableRateLimiting("strict")]
    public async Task<IActionResult> DownloadAllFiles(string code)
    {
        var box = _boxRepository.GetBoxPath(code);
        var boxPath = _boxRepository.GetBoxPath(code);

        if (box == null || !Directory.Exists(boxPath))
            return NotFound($"Box '{code}' does not exist.");

        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);

        try
        {
            var files = Directory.GetFiles(boxPath);
            foreach (string sourceFilePath in files)
            {
                if (sourceFilePath == null)
                    continue;

                var randomFileName = Path.GetFileName(sourceFilePath);
                var originalFileName = await _fileRepository.GetOriginalFileName(code, randomFileName);

                if (originalFileName == null)
                    continue;

                var destFilePath = Path.Combine(tempDirectory, originalFileName);
                System.IO.File.Copy(sourceFilePath, destFilePath);
            }

            var tempZipPath = Path.Combine(Path.GetTempPath(), $"{code}.zip");
            if (System.IO.File.Exists(tempZipPath))
                System.IO.File.Delete(tempZipPath);

            ZipFile.CreateFromDirectory(tempDirectory, tempZipPath);

            var zipBytes = await System.IO.File.ReadAllBytesAsync(tempZipPath);
            System.IO.File.Delete(tempZipPath);

            return File(zipBytes, "application/zip", $"{code}.zip");
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
                Directory.Delete(tempDirectory, recursive: true);

            var tempZipPath = Path.Combine(Path.GetTempPath(), $"{code}.zip");
            if (System.IO.File.Exists(tempZipPath))
                System.IO.File.Delete(tempZipPath);
        }
    }
}