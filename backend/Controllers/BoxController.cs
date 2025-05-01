using Microsoft.AspNetCore.Mvc;
using backend.util;

namespace backend.Controllers;

public record CreateBoxDTO(string? password = null);

[Route("api/box")]
[ApiController]
public class BoxController : ControllerBase
{
    public BoxController() { }

    [HttpGet]
    public IActionResult Funny()
    {
        return Ok("LOL");
    }

    [HttpPost("create")]
    public IActionResult CreateBox([FromBody] CreateBoxDTO data)
    {
        string code = Code.GenerateBoxCode(12);
        return Ok(code);
    }
}
