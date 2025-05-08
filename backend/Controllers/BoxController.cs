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
    public async Task<IActionResult> GetBoxDetails(string code)
    {
        var box = await _boxRepository.GetBox(code);
        if (box == null)
            return NotFound($"Box with code '{code}' does not exist.");

        return Ok(box);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBox([FromBody] CreateBoxDTO data)
    {
        var (result, code) = await _boxRepository.CreateBox();
        switch (result)
        {
            case BoxOperationResult.Success:
                return Ok(code);
            default:
                return StatusCode(500, "An error occurred while creating box.");
        }
    }

    [HttpPost("{code}/upload")]
    public async Task<IActionResult> UploadFile(string code, List<IFormFile> files)
    {
        var box = await _boxRepository.GetBox(code);

        if (box == null)
            return NotFound("Box not found.");
        else if (files.Count == 0)
            return BadRequest("No files have been provided to upload.");

        var (result, uploadedFiles) = await _boxRepository.UploadFiles(code, files);
        switch (result)
        {
            case BoxOperationResult.Success:
                return Ok(new { uploadedFiles });
            case BoxOperationResult.NotFound:
                return NotFound("Box does not exist.");
            default:
                return StatusCode(500, "An error occurred while uploading files.");
        }
    }

    [HttpDelete("{code}/delete")]
    public async Task<IActionResult> DestroyBox(string code)
    {
        var boxRemoved = await _boxRepository.DeleteBox(code);
        switch (boxRemoved)
        {
            case BoxOperationResult.Success:
                return Ok($"Successfully deleted box '{code}'");
            case BoxOperationResult.NotFound:
                return NotFound("Box does not exist.");
            default:
                return StatusCode(500, "An error occurred while deleting box.");
        }
    }
}
