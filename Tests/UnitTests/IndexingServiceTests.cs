using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using RubberSearch.Core.Services;
using RubberSearch.Core.Models;
using RubberSearch.Core.Repositories;

namespace UnitTests
{
    internal class InMemoryDocumentRepository : IDocumentRepository
    {
        // tenantId -> (docId -> Document)
        public Dictionary<string, Dictionary<string, Document>> Store { get; } = new();

        public Task SaveDocumentAsync(Document document, string tenantId)
        {
            if (!Store.TryGetValue(tenantId, out var tenantDocs))
            {
                tenantDocs = new Dictionary<string, Document>();
                Store[tenantId] = tenantDocs;
            }
            tenantDocs[document.DocId] = document;
            return Task.CompletedTask;
        }

        public Task<Document?> GetDocumentAsync(string docId, string tenantId)
        {
            if (Store.TryGetValue(tenantId, out var tenantDocs) && tenantDocs.TryGetValue(docId, out var doc))
                return Task.FromResult<Document?>(doc);
            return Task.FromResult<Document?>(null);
        }
    }

    internal class InMemoryIndexRepository : IInvertedIndexRepository
    {
        // tenantId -> token -> IndexEntry
        public Dictionary<string, Dictionary<string, IndexEntry>> IndexByTenant { get; } = new();

        public Task SaveIndexEntriesAsync(IEnumerable<IndexEntry> entries, string tenantId)
        {
            if (!IndexByTenant.TryGetValue(tenantId, out var idx))
            {
                idx = new Dictionary<string, IndexEntry>();
                IndexByTenant[tenantId] = idx;
            }

            foreach (var e in entries)
                idx[e.Token] = e;

            return Task.CompletedTask;
        }

        public Task<Dictionary<string, IndexEntry>> GetIndexEntriesAsync(string tenantId)
        {
            if (IndexByTenant.TryGetValue(tenantId, out var idx))
                return Task.FromResult(new Dictionary<string, IndexEntry>(idx));
            return Task.FromResult(new Dictionary<string, IndexEntry>());
        }

        // Implemented to satisfy interface — returns a deterministic string for tests
        public string GetIndexFile(string tenantId)
        {
            return $"{tenantId}-index.json";
        }
    }

    public class IndexingServiceTests
    {
        [Fact]
        public async Task AddDocumentAsync_PersistsDocumentAndIndex_PerTenant()
        {
            var docRepo = new InMemoryDocumentRepository();
            var idxRepo = new InMemoryIndexRepository();
            var service = new IndexingService(docRepo, idxRepo);

            var doc = new Document { DocId = "d1", Title = "Graphs", Content = "Graphs are graphs" };

            await service.AddDocumentAsync(doc, "tenantA");

            Assert.True(docRepo.Store.ContainsKey("tenantA"));
            Assert.True(docRepo.Store["tenantA"].ContainsKey("d1"));

            // index should contain tokens and postings for tenantA
            Assert.True(idxRepo.IndexByTenant.ContainsKey("tenantA"));
            var idx = idxRepo.IndexByTenant["tenantA"];
            Assert.True(idx.Count > 0, "Index for tenantA should have entries");
            Assert.Contains(idx.Values, entry => entry.Postings.ContainsKey("d1"));
        }
    }
}