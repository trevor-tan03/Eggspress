using Microsoft.AspNetCore.Mvc;
using backend.util;
using System.IO;

namespace backend.Controllers;

public record CreateBoxDTO(string? password = null);

[Route("api/box")]
[ApiController]
public class BoxController : ControllerBase
{
    public BoxController() { }

    [HttpGet("{code}")]
    public IActionResult BoxExists(string code)
    {
        if (!Directory.Exists(Path.Combine("Boxes", code)))
            return NotFound("Box does not exist.");

        return Ok();
    }

    [HttpGet("{code}/files")]
    public IActionResult GetFiles(string code)
    {
        if (!Directory.Exists(Path.Combine("Boxes", code)))
            return NotFound("Box does not exist.");

        var boxPath = Path.Combine("Boxes", code);
        var dirInfo = new DirectoryInfo(boxPath);
        var files = dirInfo.GetFiles();
        var filesDTO = ConvertToDTO.GetFilesList(files);

        return Ok(filesDTO);
    }

    [HttpPost("create")]
    public IActionResult CreateBox([FromBody] CreateBoxDTO data)
    {
        string code = Code.GenerateBoxCode(12);

        while (Directory.Exists(Path.Combine("Boxes", code)))
            code = Code.GenerateBoxCode(12);

        Directory.CreateDirectory(Path.Combine("Boxes", code));
        // Register in database
        return Ok(code);
    }

    [HttpPost("{code}/upload")]
    public async Task<IActionResult> UploadFile(string code, List<IFormFile> files)
    {
        var boxPath = Path.Combine("Boxes", code);

        if (!Directory.Exists(boxPath))
            return NotFound("Box not found.");
        else if (files.Count == 0)
            return BadRequest("No file uploaded.");

        foreach (var file in files)
        {
            var filePath = Path.Combine(boxPath, file.FileName);
            await using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);
        }

        return Ok(new { files });
    }

    [HttpDelete("{code}/delete")]
    public IActionResult DestroyBox(string code)
    {
        var boxPath = Path.Combine("Boxes", code);
        if (!Directory.Exists(boxPath))
            return NotFound("Box not found.");

        Directory.Delete(boxPath, recursive: true);
        return Ok(!Directory.Exists(boxPath));
    }
}
