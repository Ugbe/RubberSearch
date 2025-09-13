namespace RubberSearch.Api.Models
{
    public class SearchResult
    {
        public string DocId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Snippet { get; set; } = string.Empty;
        public double Score { get; set; }
        public string Url { get; set; } = string.Empty;
    }
}