using Microsoft.AspNetCore.Mvc;
using api.Interfaces;

namespace api.Controllers;

[ApiController]
[Route("api/stories")]
public class StoriesController : ControllerBase
{
    private readonly IStoriesService _storiesService;

    public StoriesController(IStoriesService storiesService)
    {
        _storiesService = storiesService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStories(
        [FromQuery] string category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30)
    {
        var result = await _storiesService.GetStoriesAsync(category, page, pageSize);
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
        var result = await _storiesService.SearchAsync(category, q, page, pageSize);
        Response.Headers["Cache-Control"] = "public, max-age=60";
        return Ok(result);
    }
}