namespace RubberSearch.Core.Models
{
    public class Document
    {
        public string TenantId { get; set; } = string.Empty;
        public string DocId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}   