namespace api.Models
{
    public class StoryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public int Score { get; set; }
        public string? Author { get; set; }
        public long Time { get; set; }
        public int CommentCount { get; set; }
        public string Type { get; set; } = "story";
        public string? Text { get; set; }
    }
}