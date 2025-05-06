using Microsoft.AspNetCore.Mvc;
using backend.Repositories;

namespace backend.Controllers;

public record CreateBoxDTO(string? password = null);

[Route("api/box")]
[ApiController]
public class BoxController : ControllerBase
{
    private readonly IBoxRepository _boxRepository;
    public BoxController(IBoxRepository boxRepository)
    {
        _boxRepository = boxRepository;
    }

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

        var files = _boxRepository.GetFiles(code);
        return Ok(files);
    }

    [HttpPost("create")]
    public IActionResult CreateBox([FromBody] CreateBoxDTO data)
    {
        var code = _boxRepository.CreateBox();
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

        var uploadedFiles = await _boxRepository.UploadFiles(code, files);

        return Ok(new { uploadedFiles });
    }

    [HttpDelete("{code}/delete")]
    public IActionResult DestroyBox(string code)
    {
        var boxPath = Path.Combine("Boxes", code);
        if (!Directory.Exists(boxPath))
            return NotFound("Box not found.");

        var boxRemoved = _boxRepository.DeleteBox(code);
        if (!boxRemoved)
            return StatusCode(500, "An error occurred while deleting box.");

        return Ok($"Successfully deleted box '{code}'");
    }
}
