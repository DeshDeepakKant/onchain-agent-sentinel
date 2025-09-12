using api.Models;
using Microsoft.Extensions.Caching.Memory;

namespace api.Services;

public class StoriesService(IMemoryCache cache, HackerNewsClient client)
{
    private static readonly MemoryCacheEntryOptions CacheOptions = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
    };

    public async Task<PagedResult<StoryDto>> GetStoriesAsync(string category, int page, int pageSize)
    {
        var cacheKey = $"stories_{category}";

        if (!cache.TryGetValue(cacheKey, out List<StoryDto>? stories))
        {
            var storyIds = await client.GetStoryIdsAsync(category, CancellationToken.None);
            stories = await HydrateStoriesAsync(storyIds.Take(200), CancellationToken.None); 

            cache.Set(cacheKey, stories, CacheOptions);
        }

        return Paginate(stories ?? [], page, pageSize);
    }

    public async Task<PagedResult<StoryDto>> SearchAsync(string query, int page, int pageSize)
    {
        var storiesToSearch = await GetStoriesForSearchAsync();
        
        if (string.IsNullOrWhiteSpace(query))
        {
            return Paginate(storiesToSearch, page, pageSize);
        }

        var lowerQuery = query.Trim().ToLowerInvariant();
        
        var filtered = storiesToSearch.Where(s =>
            (s.Title?.ToLowerInvariant().Contains(lowerQuery) ?? false) ||
            (s.Author?.ToLowerInvariant().Contains(lowerQuery) ?? false)
        ).ToList();
        
        return Paginate(filtered, page, pageSize);
    }
    
    private async Task<List<StoryDto>> GetStoriesForSearchAsync()
    {
        const string cacheKey = "stories_top";
        if (!cache.TryGetValue(cacheKey, out List<StoryDto>? stories))
        {
            var storyIds = await client.GetStoryIdsAsync("top", CancellationToken.None);
            stories = await HydrateStoriesAsync(storyIds.Take(200), CancellationToken.None);
            cache.Set(cacheKey, stories, CacheOptions);
        }
        return stories ?? [];
    }

    private async Task<List<StoryDto>> HydrateStoriesAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var tasks = new List<Task<HnApiItemDto?>>();
        var semaphore = new SemaphoreSlim(20); 

        foreach (var id in ids)
        {
            await semaphore.WaitAsync(ct);
            tasks.Add(Task.Run(async () =>
            {
                try { return await client.GetItemAsync(id, ct); }
                finally { semaphore.Release(); }
            }, ct));
        }

        var results = await Task.WhenAll(tasks);
        
        return results
            .Where(item => item is { Dead: not true, Deleted: not true } 
                       && (item!.Type == "story" || item.Type == "job"))
            .Select(item => new StoryDto
            {
                Id = item!.Id,
                Title = item.Title,
                Url = item.Url,
                Score = item.Score ?? 0,
                Author = item.Author,
                Time = item.Time,
                CommentCount = item.CommentCount ?? 0,
                Type = item.Type!,
                Text = item.Text
            })
            .ToList();
    }
    
    private static PagedResult<T> Paginate<T>(IReadOnlyCollection<T> source, int page, int pageSize)
    {
        var totalCount = source.Count;
        var items = source.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        
        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            HasMore = (page * pageSize) < totalCount
        };
    }
}