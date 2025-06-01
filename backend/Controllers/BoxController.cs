using Microsoft.AspNetCore.Mvc;
using backend.Repositories;
using Microsoft.AspNetCore.Identity;
using backend.Models;
using Microsoft.AspNetCore.RateLimiting;
using backend.util;
using backend.Filters;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using System.Web;
using backend.Services;

namespace backend.Controllers;

public class BoxAuthDTO
{
    public string Password { get; set; } = default!;
}


[Route("api/box")]
[ApiController]
public class BoxController : ControllerBase
{
    private readonly ILogger<BoxController> _logger;
    private readonly IBoxRepository _boxRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IUploadService _uploadService;

    public BoxController(ILogger<BoxController> logger, IBoxRepository boxRepository, IFileRepository fileRepository, IUploadService uploadService)
    {
        _logger = logger;
        _boxRepository = boxRepository;
        _fileRepository = fileRepository;
        _uploadService = uploadService;
    }

    [HttpGet("{code}")]
    [BoxAuth("code")]
    //[EnableRateLimiting("lenient")]
    public async Task<IActionResult> GetBoxDetails(string code)
    {
        var box = await _boxRepository.GetBox(code);

        if (box == null)
            return NotFound($"Box with code '{code}' does not exist.");

        var boxFiles = await _boxRepository.GetFiles(code) ?? [];
        var boxDTO = ConvertToDTO.Box(box, boxFiles);
        return Ok(boxDTO);
    }

    [HttpPost("{code}/auth")]
    //[EnableRateLimiting("lenient")]
    public async Task<IActionResult> AuthorizeBoxAccess(string code, [FromBody] BoxAuthDTO boxAuth)
    {
        var box = await _boxRepository.GetBox(code);

        if (box == null)
            return NotFound($"Box with code '{code}' does not exist.");

        if (box.Password != null)
        {
            var hasher = new PasswordHasher<Box>();
            var passwordCorrect = hasher.VerifyHashedPassword(box, box.Password, boxAuth.Password);

            if (passwordCorrect == PasswordVerificationResult.Failed)
                return Unauthorized("Incorrect box password");
        }

        var timeRemaining = box.ExpiresAt - DateTime.UtcNow;
        var secsTillExpiry = timeRemaining.TotalSeconds;

        HttpContext.Response.Cookies.Append($"box_auth_{code}", "true", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
            MaxAge = TimeSpan.FromSeconds(secsTillExpiry),
        });

        return Ok("Authenticated.");
    }

    [HttpPost("create")]
    //[EnableRateLimiting("lenient")]
    public async Task<IActionResult> CreateBox([FromForm] string? password)
    {
        var (createResult, box) = await _boxRepository.CreateBox(password ?? null);

        if (createResult == BoxOperationResult.Error || box == null)
            return StatusCode(500, "An error occurred while creating box.");

        return Ok(box.Code);
    }

    [HttpPost("{code}/upload/chunk")]
    [BoxAuth("code")]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> StreamChunk(string code)
    {
        try
        {
            var box = await _boxRepository.GetBox(code);
            if (box is null)
                return NotFound("Box not found.");

            var model = await UploadRequestModel.FromFormAsync(Request);
            if (model is null)
                return BadRequest("Invalid or missing form data");

            if (model.FileName.Length > 100)
                return BadRequest("File name must not exceed 100 characters.");

            if (model.ChunkNumber < 0 || model.ChunkNumber >= model.TotalChunks)
                return BadRequest("Invalid chunk number");

            var tempFolder = Path.Combine(Path.GetTempPath(), "uploads", model.FileId);
            Directory.CreateDirectory(tempFolder);

            // Map file to random file name
            if (model.ChunkNumber == 0)
            {
                var extension = Path.GetExtension(model.FileName);
                var randomName = Guid.NewGuid().ToString("N") + extension;
                await _fileRepository.AddFile(code, model.FileId, model.FileName, randomName, model.TotalChunks);
            }

            var allowed = await _uploadService.TryEnforceBoxSizeLimitAsync(code, model.FileId, model.Chunk.Length);
            if (!allowed)
                return BadRequest("Box size exceeded 5 GB.");

            var chunkPath = Path.Combine(tempFolder, $"{model.ChunkNumber}.part");
            await _uploadService.SaveChunkAsync(model.Chunk, chunkPath);

            var combined = await _uploadService.CombineIfComplete(code, model.FileId, model.TotalChunks, tempFolder);

            return Ok(new { success = true, chunksReceived = model.ChunkNumber + 1, combined });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Upload failed: {ex.Message}");
        }
    }

    [HttpGet("{code}/upload/status/{fileId}")]
    public IActionResult GetUploadStatus(string code, string fileId)
    {
        fileId = HttpUtility.HtmlDecode(fileId);
        var tempFolder = Path.Combine(Path.GetTempPath(), "uploads", fileId);

        // File already done uploading or never uploaded
        if (!Directory.Exists(tempFolder))
            return Ok(new { uploadedChunks = new List<int>() });

        var chunks = Directory.GetFiles(tempFolder)
            .Select(path => int.Parse(Path.GetFileNameWithoutExtension(path)))
            .ToList();

        return Ok(chunks);
    }

    [HttpDelete("{code}/delete")]
    [BoxAuth("code")]
    //[EnableRateLimiting("strict")]
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
