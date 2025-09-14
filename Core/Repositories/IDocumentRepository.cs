using RubberSearch.Core.Models;
namespace RubberSearch.Core.Repositories
{
    public interface IDocumentRepository
    {
        Task SaveDocumentAsync(Document document);
        Task<Document?> GetDocumentAsync(string docId);
    }
}
