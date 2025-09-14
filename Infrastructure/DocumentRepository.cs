using System.Text.Json;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;

namespace RubberSearch.Infrastructure
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly string _basePath;

        public DocumentRepository(string basePath)
        {
            _basePath = Path.Combine(basePath, "docs");
            Directory.CreateDirectory(_basePath);
        }

        public async Task SaveDocumentAsync(Document document)
        {
            var filePath = Path.Combine(_basePath, $"{document.DocId}.json");
            var json = JsonSerializer.Serialize(document, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<Document?> GetDocumentAsync(string docId)
        {
            var filePath = Path.Combine(_basePath, $"{docId}.json");
            if (!File.Exists(filePath))
                return null;

            var json = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<Document>(json);
        }
    }
}