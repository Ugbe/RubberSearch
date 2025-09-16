namespace RubberSearch.Core.Models
{
    public class Document
    {
    /// <summary>
    /// Tenant identifier, as resolved from the API Key. Remains here despite the doc already being stored in the appropriately named folder in case of any changes to the structure in the future.
    /// </summary>
        public string TenantId { get; set; } = string.Empty;
    /// <summary>
    /// Unique document identifier within the tenant namespace.
    /// </summary>
        public string DocId { get; set; } = string.Empty;
    /// <summary>
    /// Human-readable title of the document.
    /// </summary>
        public string Title { get; set; } = string.Empty;
    /// <summary>
    /// Full text content used for indexing and search.
    /// </summary>
        public string Content { get; set; } = string.Empty;
    /// <summary>
    /// Canonical URL for the document, used in search results.
    /// </summary>
        public string Url { get; set; } = string.Empty;
    }
}   