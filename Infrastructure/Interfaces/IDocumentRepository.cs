namespace RubberSearch.Infrastructure.Interfaces
{
    public interface IDocumentRepository
    {
        Task SaveDocumentAsync(Document document);
        Task<Document?> GetDocumentAsync(string docId);
    }
}