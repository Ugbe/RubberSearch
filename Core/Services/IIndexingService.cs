namespace RubberSearch.Core.Services
{
    public interface IIndexingService
    {
        Task AddDocumentAsync(Document document);
        Task<Document?> GetDocumentAsync(string docId);
    }
}