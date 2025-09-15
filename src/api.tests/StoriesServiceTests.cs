using Moq;
using Microsoft.Extensions.Caching.Memory;
using api.Interfaces;
using api.Models;
using api.Services;

namespace api.tests
{
    public class StoriesServiceTests
    {
        private readonly IMemoryCache _cache;
        private readonly Mock<IHackerNewsClient> _client;
        private readonly StoriesService _svc;

        public StoriesServiceTests()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _client = new Mock<IHackerNewsClient>(); 
            _svc = new StoriesService(_cache, _client.Object);
        }

        [Fact]
        public async Task GetStoriesAsync_CachesResult_AfterFirstCall()
        {
            var category = "top";

            _client.Setup(c => c.GetStoryIdsAsync(category, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new[] { 1, 2 });
            _client.Setup(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 1, Title = "Job Post", Type = "job" });
            _client.Setup(c => c.GetItemAsync(2, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 2, Title = "Story", Type = "story" });

            var first  = await _svc.GetStoriesAsync(category, page: 1, pageSize: 30);
            var second = await _svc.GetStoriesAsync(category, page: 1, pageSize: 30);

            _client.Verify(c => c.GetStoryIdsAsync(category, It.IsAny<CancellationToken>()), Times.Once);

            Assert.Equal(2, first.TotalCount);
            Assert.Equal(2, second.TotalCount);
            Assert.Contains(first.Items, s => s.Title == "Story");
        }

        [Fact]
        public async Task SearchAsync_FiltersByTitleOrAuthor_AndPaginates()
        {
            var category = "new";
            _client.Setup(c => c.GetStoryIdsAsync(category, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new[] { 1, 2, 3, 4 });

            _client.Setup(c => c.GetItemAsync(1, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 1, Title = "Angular Tips",     Type = "story", Author = "alice" });
            _client.Setup(c => c.GetItemAsync(2, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 2, Title = "C# Patterns",      Type = "story", Author = "garth" });
            _client.Setup(c => c.GetItemAsync(3, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 3, Title = "GARTH builds HN", Type = "story", Author = "bob" });
            _client.Setup(c => c.GetItemAsync(4, It.IsAny<CancellationToken>()))
                   .ReturnsAsync(new HnApiItemDto { Id = 4, Title = "RxJS Deep Dive",   Type = "story", Author = "carol" });

            var page1 = await _svc.SearchAsync(category, "garth", page: 1, pageSize: 1);
            var page2 = await _svc.SearchAsync(category, "garth", page: 2, pageSize: 1);

            Assert.Equal(2, page1.TotalCount);  
            Assert.True(page1.HasMore);       
            Assert.Single(page1.Items);
            Assert.Single(page2.Items);
            Assert.NotEqual(page1.Items.First().Id, page2.Items.First().Id);
        }
    }
}
