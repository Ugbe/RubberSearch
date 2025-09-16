using System.Text.Json;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;

namespace RubberSearch.Infrastructure
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly string _dataPath;

        public DocumentRepository(string basePath)
        {
            _dataPath = basePath;
            Directory.CreateDirectory(_dataPath);
        }

        public async Task SaveDocumentAsync(Document document, string tenantId)
        {
            var docsPath = Path.Combine(_dataPath, tenantId, "docs");
            Directory.CreateDirectory(docsPath);
            var filePath = Path.Combine(docsPath, $"{document.DocId}.json");
            var json = JsonSerializer.Serialize(document, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Document?> GetDocumentAsync(string docId, string tenantId)
        {
            var filePath = Path.Combine(_dataPath, tenantId, "docs", $"{docId}.json");
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Document>(json);
        }
    }
}