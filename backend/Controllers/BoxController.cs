using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using System.Threading.Tasks;

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
    public async Task<IActionResult> BoxExists(string code)
    {
        if (!await _boxRepository.BoxExists(code))
            return NotFound("Box does not exist.");

        return Ok($"Box with code '{code}' found.");
    }

    [HttpGet("{code}/files")]
    public async Task<IActionResult> GetFiles(string code)
    {
        if (!await _boxRepository.BoxExists(code))
            return NotFound("Box does not exist.");

        var files = await _boxRepository.GetFiles(code);
        return Ok(files);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBox([FromBody] CreateBoxDTO data)
    {
        var code = await _boxRepository.CreateBox();
        return Ok(code);
    }

    [HttpPost("{code}/upload")]
    public async Task<IActionResult> UploadFile(string code, List<IFormFile> files)
    {
        var boxExists = await _boxRepository.BoxExists(code);

        if (!boxExists)
            return NotFound("Box not found.");
        else if (files.Count == 0)
            return BadRequest("No file uploaded.");

        var uploadedFiles = await _boxRepository.UploadFiles(code, files);

        return Ok(new { uploadedFiles });
    }

    [HttpDelete("{code}/delete")]
    public async Task<IActionResult> DestroyBox(string code)
    {
        var boxExists = await _boxRepository.BoxExists(code);
        if (!boxExists)
            return NotFound("Box not found.");

        var boxRemoved = await _boxRepository.DeleteBox(code);
        if (!boxRemoved)
            return StatusCode(500, "An error occurred while deleting box.");

        return Ok($"Successfully deleted box '{code}'");
    }
}
