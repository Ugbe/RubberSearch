using System.Text.Json;
using RubberSearch.Core.Models;
using RubberSearch.Infrastructure.Interfaces;

namespace RubberSearch.Infrastructure
{
    public class InvertedIndexRepository : IInvertedIndexRepository
    {
        private readonly string _indexPath;
        private readonly string _indexFile;

        public InvertedIndexRepository(string basePath)
        {
            _indexPath = Path.Combine(basePath, "index");
            Directory.CreateDirectory(_indexPath);
            _indexFile = Path.Combine(_indexPath, "index.json");
        }

        public async Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries)
        {
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            await File.WriteAllTextAsync(_indexFile, json);
        }

        public async Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync()
        {
            if (!File.Exists(_indexFile))
                return new Dictionary<string, IndexEntry>();

            var json = await File.ReadAllTextAsync(_indexFile);
            var entries = JsonSerializer.Deserialize<List<IndexEntry>>(json) ?? new();
            return entries.ToDictionary(e => e.Token);
        }
    }
}