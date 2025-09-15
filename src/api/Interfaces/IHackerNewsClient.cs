using api.Models;

namespace api.Interfaces;

public interface IHackerNewsClient
{
    Task<int[]> GetStoryIdsAsync(string category, CancellationToken ct);
    Task<HnApiItemDto?> GetItemAsync(int id, CancellationToken ct);
}
