using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using backend.Models;

namespace backend.Controllers;


[Route("api/box")]
[ApiController]
public class BoxController : ControllerBase
{
    private readonly ILogger<BoxController> _logger;
    private readonly IBoxRepository _boxRepository;

    public BoxController(ILogger<BoxController> logger, IBoxRepository boxRepository)
    {
        _logger = logger;
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
    [RequestSizeLimit(1_000_000_000)]
    public async Task<IActionResult> CreateBox([FromForm] string? password, [FromForm] List<IFormFile> files)
    {
        var (createResult, box) = await _boxRepository.CreateBox(password);

        if (createResult == BoxOperationResult.Error || box == null)
            return StatusCode(500, "An error occurred while creating box.");

        if (files.Count == 0)
            return BadRequest("No files have been provided to upload.");

        var (uploadResult, uploadedFiles) = await _boxRepository.UploadFiles(box.Code, files);

        return uploadResult switch
        {
            BoxOperationResult.Success => Ok(new { box.Code, uploadedFiles }),
            BoxOperationResult.NotFound => NotFound("Box does not exist."),
            _ => StatusCode(500, "Error uploading files.")
        };
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
