using System.Text.Json.Serialization;

namespace api.Models
{
    public class HnApiItemDto
    {
        public int Id { get; set; }
        public bool? Deleted { get; set; }
        public string Type { get; set; } = string.Empty;
        [JsonPropertyName("by")]
        public string? Author { get; set; }
        public long Time { get; set; }
        public string? Text { get; set; }
        public bool? Dead { get; set; }
        public int? Parent { get; set; }
        public int[]? Kids { get; set; }
        public string? Url { get; set; }
        public int? Score { get; set; }
        public string? Title { get; set; }
        [JsonPropertyName("descendants")]
        public int? CommentCount { get; set; }
    }
}