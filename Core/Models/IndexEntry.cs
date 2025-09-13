namespace RubberSearch.Core.Models
{
    public class IndexEntry
    {
        public string Token { get; set; } = string.Empty;
        public HashSet<string> DocumentIds { get; set; } = new();

    }
}