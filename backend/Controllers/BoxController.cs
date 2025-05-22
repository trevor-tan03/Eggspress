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

    public BoxController(ILogger<BoxController> logger, IBoxRepository boxRepository)
    {
        _logger = logger;
        _boxRepository = boxRepository;
    }

    [HttpGet("{code}")]
    [BoxAuth("code")]
    [EnableRateLimiting("lenient")]
    public async Task<IActionResult> GetBoxDetails(string code)
    {
        var box = await _boxRepository.GetBox(code);

        if (box == null)
            return NotFound($"Box with code '{code}' does not exist.");

        var boxFiles = _boxRepository.GetFiles(code) ?? [];
        var boxDTO = ConvertToDTO.Box(box, boxFiles);
        return Ok(boxDTO);
    }

    [HttpPost("{code}/auth")]
    [EnableRateLimiting("lenient")]
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
        var minsTillExpiry = Math.Floor(timeRemaining.TotalMinutes);

        HttpContext.Response.Cookies.Append($"box_auth_{code}", "true", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
            MaxAge = TimeSpan.FromMinutes(minsTillExpiry),
        });

        return Ok("Authenticated.");
    }

    [HttpPost("create")]
    [EnableRateLimiting("lenient")]
    public async Task<IActionResult> CreateBox([FromForm] string? password)
    {
        var (createResult, box) = await _boxRepository.CreateBox(password ?? null);

        if (createResult == BoxOperationResult.Error || box == null)
            return StatusCode(500, "An error occurred while creating box.");

        return Ok(box.Code);
    }

    [HttpPost("{code}/upload/chunk")]
    [DisableFormValueModelBinding]
    [EnableRateLimiting("lenient")]
    public async Task<IActionResult> StreamChunk(string code)
    {
        try
        {
            var request = HttpContext.Request;
            if (!request.HasFormContentType)
                return BadRequest("Expected multipart form data.");

            var form = await request.ReadFormAsync();

            var chunk = form.Files["file"];
            var chunkNumber = int.Parse(form["chunkNumber"]!);
            var totalChunks = int.Parse(form["totalChunks"]!);
            var fileId = form["fileId"].ToString();
            var fileName = form["fileName"].ToString();

            if (chunk == null || chunk.Length == 0)
                return BadRequest("No chunk uploaded.");

            // Temp folder for assembling chunks
            var tempFolder = Path.Combine(Path.GetTempPath(), "uploads", fileId);
            Directory.CreateDirectory(tempFolder);

            // Save chunk with sequential number
            var chunkPath = Path.Combine(tempFolder, $"{chunkNumber}.part");
            await using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }

            // If all chunks are uploaded, combine them
            if (Directory.GetFiles(tempFolder).Length == totalChunks)
            {
                var boxPath = _boxRepository.GetBoxPath(code);
                var filePath = Path.Combine(boxPath, fileName);
                await Chunks.Combine(tempFolder, filePath);
                Directory.Delete(tempFolder, recursive: true); // Cleanup
            }

            return Ok(new { success = true, chunksReceived = chunkNumber + 1 });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Upload failed: {ex.Message}");
        }
    }

    [HttpDelete("{code}/delete")]
    [BoxAuth("code")]
    [EnableRateLimiting("strict")]
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
