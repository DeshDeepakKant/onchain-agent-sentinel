using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("api/stories")]
public class StoriesController(StoriesService storiesService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetStories(
        [FromQuery] string category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var result = await storiesService.GetStoriesAsync(category, page, pageSize);
        Response.Headers["Cache-Control"] = "public, max-age=60";
        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string category,
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var result = await storiesService.SearchAsync(category, q, page, pageSize);
        Response.Headers["Cache-Control"] = "public, max-age=60";
        return Ok(result);
    }
}