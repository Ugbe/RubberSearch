using RubberSearch.Core.Models;
namespace RubberSearch.Core.Repositories
{
    public interface IDocumentRepository
    {
        Task SaveDocumentAsync(Document document, string tenantId);
        Task<Document?> GetDocumentAsync(string docId, string tenantId);
    }
}
