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
    public IActionResult Funny(string code)
    {
        return Ok(Directory.Exists(code));
    }

    [HttpPost("create")]
    public IActionResult CreateBox([FromBody] CreateBoxDTO data)
    {
        string code = Code.GenerateBoxCode(12);

        while (Directory.Exists(code))
            code = Code.GenerateBoxCode(12);

        Directory.CreateDirectory(code);
        // Register in database
        return Ok(code);
    }
}
