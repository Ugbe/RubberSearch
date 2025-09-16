using System.Text.Json;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;

namespace RubberSearch.Infrastructure
{
    public class InvertedIndexRepository : IInvertedIndexRepository
    {
        private readonly string _dataPath;

        public InvertedIndexRepository(string basePath)
        {
            _dataPath = basePath;
            Directory.CreateDirectory(_dataPath);
        }

        public string GetIndexFile(string tenantId)
        {
            var indexDir = Path.Combine(_dataPath, tenantId, "index");
            Directory.CreateDirectory(indexDir);
            return Path.Combine(indexDir, "index.json");
        }

        public async Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries, string tenantId)
        {
            var file = GetIndexFile(tenantId);
            var json = JsonSerializer.Serialize(entries, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            var tmp = file + ".tmp";
            await File.WriteAllTextAsync(tmp, json);
            File.Move(tmp, file, overwrite: true);
        }

        public async Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync(string tenantId)
        {
            var file = GetIndexFile(tenantId);
            if (!File.Exists(file)) return new Dictionary<string, IndexEntry>();
            var json = await File.ReadAllTextAsync(file);
            var entries = JsonSerializer.Deserialize<List<IndexEntry>>(json) ?? new List<IndexEntry>();
            return entries.ToDictionary(e => e.Token);
        }
    }
}