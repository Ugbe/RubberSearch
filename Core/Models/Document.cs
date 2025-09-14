namespace RubberSearch.Core.Models
{
    public class Document
    {
    /// <summary>
    /// Tenant identifier (reserved for future multi-tenant support).
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