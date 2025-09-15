using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

using api.Controllers;
using api.Interfaces;   
using api.Models;

namespace api.tests
{
    public class StoriesControllerTests
    {
        private readonly Mock<IStoriesService> _mockStoriesService;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _mockStoriesService = new Mock<IStoriesService>();

            _controller = new StoriesController(_mockStoriesService.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetStories_ReturnsOkResult_WithPagedData()
        {
            var category = "top";
            var page = 1;
            var pageSize = 10;

            var fakeStories = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto> { new StoryDto { Id = 1, Title = "Test Story" } },
                Page = page,
                PageSize = pageSize,
                TotalCount = 1,
                HasMore = false
            };

            _mockStoriesService
                .Setup(s => s.GetStoriesAsync(category, page, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeStories);
            
            var result = await _controller.GetStories(category, page, pageSize);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsType<PagedResult<StoryDto>>(okResult.Value);
            Assert.Equal(1, returnedData.TotalCount);
            Assert.Equal("Test Story", returnedData.Items.First().Title);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithFilteredData()
        {
            var category = "new";
            var query = "test";
            var page = 1;
            var pageSize = 10;

            var fakeSearchResult = new PagedResult<StoryDto>
            {
                Items = new List<StoryDto> { new StoryDto { Id = 2, Title = "Another Test Story" } },
                Page = page,
                PageSize = pageSize,
                TotalCount = 1,
                HasMore = false
            };

            _mockStoriesService
                .Setup(s => s.SearchAsync(category, query, page, pageSize, It.IsAny<CancellationToken>()))
                .ReturnsAsync(fakeSearchResult);

            var result = await _controller.Search(category, query, page, pageSize);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedData = Assert.IsType<PagedResult<StoryDto>>(okResult.Value);
            Assert.Equal(1, returnedData.TotalCount);
            Assert.Equal("Another Test Story", returnedData.Items.First().Title);
        }
    }
}
