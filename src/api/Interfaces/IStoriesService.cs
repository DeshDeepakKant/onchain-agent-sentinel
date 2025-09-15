using api.Models;

namespace api.Interfaces;

public interface IStoriesService
{
    Task<PagedResult<StoryDto>> GetStoriesAsync(string category, int page, int pageSize, CancellationToken ct = default);
    Task<PagedResult<StoryDto>> SearchAsync(string category, string query, int page, int pageSize, CancellationToken ct = default);
}