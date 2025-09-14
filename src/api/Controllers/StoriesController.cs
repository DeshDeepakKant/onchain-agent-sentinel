using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api.Controllers;

[ApiController]
[Route("api/stories")]
public class StoriesController(StoriesService storiesService) : ControllerBase
{
    private async Task<IActionResult> GetStoriesForCategoryAsync(string category, int page, int pageSize)
    {
        var result = await storiesService.GetStoriesAsync(category, page, pageSize);
        Response.Headers["Cache-Control"] = "public, max-age=60";
        return Ok(result);
    }
    
    [HttpGet("top")]
    public async Task<IActionResult> GetTopStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("top", page, pageSize);

    [HttpGet("new")]
    public async Task<IActionResult> GetNewStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("new", page, pageSize);
        
    [HttpGet("best")]
    public async Task<IActionResult> GetBestStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("best", page, pageSize);

    [HttpGet("ask")]
    public async Task<IActionResult> GetAskStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("ask", page, pageSize);

    [HttpGet("show")]
    public async Task<IActionResult> GetShowStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("show", page, pageSize);

    [HttpGet("job")]
    public async Task<IActionResult> GetJobStories([FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        => await GetStoriesForCategoryAsync("job", page, pageSize);

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string category, [FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
    {
        var result = await storiesService.SearchAsync(category, q, page, pageSize);
        Response.Headers["Cache-Control"] = "public, max-age=60";
        return Ok(result);
    }
}