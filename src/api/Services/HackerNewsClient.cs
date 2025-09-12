using api.Models;

namespace api.Services;

public class HackerNewsClient(HttpClient httpClient)
{
    public async Task<int[]> GetStoryIdsAsync(string category, CancellationToken ct)
    {
        var endpoint = $"{category.ToLowerInvariant()}stories.json";
        return await httpClient.GetFromJsonAsync<int[]>(endpoint, ct) ?? [];
    }

    public async Task<HnApiItemDto?> GetItemAsync(int id, CancellationToken ct)
    {
        try
        {
            return await httpClient.GetFromJsonAsync<HnApiItemDto>($"item/{id}.json", ct);
        }
        catch (HttpRequestException)
        {
            return null;
        }
    }
}