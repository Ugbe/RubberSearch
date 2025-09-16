using RubberSearch.Core.Models;

namespace RubberSearch.Core.Services
{
    /// <summary>
    /// Provides indexing operations for documents.
    /// </summary>
    public interface IIndexingService
    {
        /// <summary>
        /// Add or update a document in the index and persist it to storage.
        /// </summary>
        Task AddDocumentAsync(Document document, string tenantId);

        /// <summary>
        /// Retrieve a document by id from storage.
        /// </summary>
        Task<Document?> GetDocumentAsync(string docId, string tenantId);
    }
}